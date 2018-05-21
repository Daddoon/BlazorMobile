// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Mono.Cecil;
using Mono.Cecil.Cil;
using ReactiveUI.Fody;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.ExceptionServices;
using System.Threading;
using SR = Daddoon.Blazor.Xam.Common.Resources.Strings;

namespace System.Reflection
{
    // Helper class to handle the IL EMIT for the generation of proxies.
    // Much of this code was taken directly from the Silverlight proxy generation.
    // Differences between this and the Silverlight version are:
    //  1. This version is based on DispatchProxy from NET Native and CoreCLR, not RealProxy in Silverlight ServiceModel.
    //     There are several notable differences between them.
    //  2. Both DispatchProxy and RealProxy permit the caller to ask for a proxy specifying a pair of types:
    //     the interface type to implement, and a base type.  But they behave slightly differently:
    //       - RealProxy generates a proxy type that derives from Object and *implements" all the base type's
    //         interfaces plus all the interface type's interfaces.
    //       - DispatchProxy generates a proxy type that *derives* from the base type and implements all
    //         the interface type's interfaces.  This is true for both the CLR version in NET Native and this
    //         version for CoreCLR.
    //  3. DispatchProxy and RealProxy use different type hierarchies for the generated proxies:
    //       - RealProxy type hierarchy is:
    //             proxyType : proxyBaseType : object
    //         Presumably the 'proxyBaseType' in the middle is to allow it to implement the base type's interfaces
    //         explicitly, preventing collision for same name methods on the base and interface types.
    //       - DispatchProxy hierarchy is:
    //             proxyType : baseType (where baseType : DispatchProxy)
    //         The generated DispatchProxy proxy type does not need to generate implementation methods
    //         for the base type's interfaces, because the base type already must have implemented them.
    //  4. RealProxy required a proxy instance to hold a backpointer to the RealProxy instance to mirror
    //     the .Net Remoting design that required the proxy and RealProxy to be separate instances.
    //     But the DispatchProxy design encourages the proxy type to *be* an DispatchProxy.  Therefore,
    //     the proxy's 'this' becomes the equivalent of RealProxy's backpointer to RealProxy, so we were
    //     able to remove an extraneous field and ctor arg from the DispatchProxy proxies.
    //
    internal static class DispatchProxyGenerator
    {
        // Generated proxies have a private Action field that all generated methods
        // invoke.  It is the first field in the class and the first ctor parameter.
        private const int InvokeActionFieldAndCtorParameterIndex = 0;

        // Proxies are requested for a pair of types: base type and interface type.
        // The generated proxy will subclass the given base type and implement the interface type.
        // We maintain a cache keyed by 'base type' containing a dictionary keyed by interface type,
        // containing the generated proxy type for that pair.   There are likely to be few (maybe only 1)
        // base type in use for many interface types.
        // Note: this differs from Silverlight's RealProxy implementation which keys strictly off the
        // interface type.  But this does not allow the same interface type to be used with more than a
        // single base type.  The implementation here permits multiple interface types to be used with
        // multiple base types, and the generated proxy types will be unique.
        // This cache of generated types grows unbounded, one element per unique T/ProxyT pair.
        // This approach is used to prevent regenerating identical proxy types for identical T/Proxy pairs,
        // which would ultimately be a more expensive leak.
        // Proxy instances are not cached.  Their lifetime is entirely owned by the caller of DispatchProxy.Create.
        private static readonly Dictionary<Type, Dictionary<Type, Type>> s_baseTypeAndInterfaceToGeneratedProxyType = new Dictionary<Type, Dictionary<Type, Type>>();
        private static readonly ProxyAssembly s_proxyAssembly = new ProxyAssembly();
        private static MethodDefinition s_dispatchProxyInvokeMethod = null;
        //private static readonly MethodInfo s_dispatchProxyInvokeMethod = typeof(DispatchProxy).GetTypeInfo().GetDeclaredMethod("Invoke");

        // Returns a new instance of a proxy the derives from 'baseType' and implements 'interfaceType'
        internal static object CreateProxyInstance(Type baseType, Type interfaceType)
        {
            Debug.Assert(baseType != null);
            Debug.Assert(interfaceType != null);

            Type proxiedType = GetProxyType(baseType, interfaceType);
            return Activator.CreateInstance(proxiedType, (Action<object[]>)DispatchProxyGenerator.Invoke);
        }

        private static Type GetProxyType(Type baseType, Type interfaceType)
        {
            lock (s_baseTypeAndInterfaceToGeneratedProxyType)
            {
                Dictionary<Type, Type> interfaceToProxy = null;
                if (!s_baseTypeAndInterfaceToGeneratedProxyType.TryGetValue(baseType, out interfaceToProxy))
                {
                    interfaceToProxy = new Dictionary<Type, Type>();
                    s_baseTypeAndInterfaceToGeneratedProxyType[baseType] = interfaceToProxy;
                }

                Type generatedProxy = null;
                if (!interfaceToProxy.TryGetValue(interfaceType, out generatedProxy))
                {
                    generatedProxy = GenerateProxyType(baseType, interfaceType);
                    interfaceToProxy[interfaceType] = generatedProxy;
                }

                return generatedProxy;
            }
        }

        // Unconditionally generates a new proxy type derived from 'baseType' and implements 'interfaceType'
        private static Type GenerateProxyType(Type baseType, Type interfaceType)
        {
            // Parameter validation is deferred until the point we need to create the proxy.
            // This prevents unnecessary overhead revalidating cached proxy types.
            TypeInfo baseTypeInfo = baseType.GetTypeInfo();

            // The interface type must be an interface, not a class
            if (!interfaceType.GetTypeInfo().IsInterface)
            {
                // "T" is the generic parameter seen via the public contract
                throw new ArgumentException(string.Format(SR.InterfaceType_Must_Be_Interface, interfaceType.FullName), "T");
            }

            // The base type cannot be sealed because the proxy needs to subclass it.
            if (baseTypeInfo.IsSealed)
            {
                // "TProxy" is the generic parameter seen via the public contract
                throw new ArgumentException(string.Format(SR.BaseType_Cannot_Be_Sealed, baseTypeInfo.FullName), "TProxy");
            }

            // The base type cannot be abstract
            if (baseTypeInfo.IsAbstract)
            {
                throw new ArgumentException(string.Format(SR.BaseType_Cannot_Be_Abstract, baseType.FullName), "TProxy");
            }

            // The base type must have a public default ctor
            if (!baseTypeInfo.DeclaredConstructors.Any(c => c.IsPublic && c.GetParameters().Length == 0))
            {
                throw new ArgumentException(string.Format(SR.BaseType_Must_Have_Default_Ctor, baseType.FullName), "TProxy");
            }

            // Create a type that derives from 'baseType' provided by caller
            ProxyBuilder pb = s_proxyAssembly.CreateProxy("generatedProxy", baseType);

            foreach (Type t in interfaceType.GetTypeInfo().ImplementedInterfaces)
                pb.AddInterfaceImpl(t);

            pb.AddInterfaceImpl(interfaceType);

            Type generatedProxyType = pb.CreateType();
            return generatedProxyType;
        }

        private static TypeReference[] GetTypeReferences(Type[] types)
        {
            var modDef = s_proxyAssembly.GetModuleDefinition();
            TypeReference[] references = new TypeReference[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                references[i] = modDef.ImportReference(types[i]);
            }

            return references;
        }


        // All generated proxy methods call this static helper method to dispatch.
        // Its job is to unpack the arguments and the 'this' instance and to dispatch directly
        // to the (abstract) DispatchProxy.Invoke() method.
        private static void Invoke(object[] args)
        {
            PackedArgs packed = new PackedArgs(args);

            MethodDefinition method = s_proxyAssembly.ResolveMethodToken(packed.DeclaringType, packed.MethodToken);
            if (method.IsGenericInstance)
            {
                method.MakeGenericMethod(GetTypeReferences(packed.GenericTypes));
                //method = ((MethodInfo)method).MakeGenericMethod(packed.GenericTypes);
            }

            //MethodBase method = s_proxyAssembly.ResolveMethodToken(packed.DeclaringType, packed.MethodToken);
            //if (method.IsGenericMethodDefinition)
            //    method = ((MethodInfo)method).MakeGenericMethod(packed.GenericTypes);

            // Call (protected method) DispatchProxy.Invoke()
            try
            {
                if (s_dispatchProxyInvokeMethod == null)
                {
                    var modDef = s_proxyAssembly.GetModuleDefinition();
                    s_dispatchProxyInvokeMethod = modDef.ImportReference(typeof(DispatchProxy)).Resolve().Methods.FirstOrDefault(p => p.Name == "Invoke");
                }

                Debug.Assert(s_dispatchProxyInvokeMethod != null);

                //TODO!

                //object returnValue = s_dispatchProxyInvokeMethod.Invoke(packed.DispatchProxy,
                //                                                       new object[] { method, packed.Args });
                //packed.ReturnValue = returnValue;
            }
            catch (TargetInvocationException tie)
            {
                ExceptionDispatchInfo.Capture(tie.InnerException).Throw();
            }

            //PackedArgs packed = new PackedArgs(args);
            //MethodBase method = s_proxyAssembly.ResolveMethodToken(packed.DeclaringType, packed.MethodToken);
            //if (method.IsGenericMethodDefinition)
            //    method = ((MethodInfo)method).MakeGenericMethod(packed.GenericTypes);

            // Call (protected method) DispatchProxy.Invoke()
            //try
            //{
            //    Debug.Assert(s_dispatchProxyInvokeMethod != null);
            //    object returnValue = s_dispatchProxyInvokeMethod.Invoke(packed.DispatchProxy,
            //                                                           new object[] { method, packed.Args });
            //    packed.ReturnValue = returnValue;
            //}
            //catch (TargetInvocationException tie)
            //{
            //    ExceptionDispatchInfo.Capture(tie.InnerException).Throw();
            //}
        }

        private class PackedArgs
        {
            internal const int DispatchProxyPosition = 0;
            internal const int DeclaringTypePosition = 1;
            internal const int MethodTokenPosition = 2;
            internal const int ArgsPosition = 3;
            internal const int GenericTypesPosition = 4;
            internal const int ReturnValuePosition = 5;

            internal static readonly Type[] PackedTypes = new Type[] { typeof(object), typeof(Type), typeof(int), typeof(object[]), typeof(Type[]), typeof(object) };

            private object[] _args;
            internal PackedArgs() : this(new object[PackedTypes.Length]) { }
            internal PackedArgs(object[] args) { _args = args; }

            internal DispatchProxy DispatchProxy { get { return (DispatchProxy)_args[DispatchProxyPosition]; } }
            internal Type DeclaringType { get { return (Type)_args[DeclaringTypePosition]; } }
            internal int MethodToken { get { return (int)_args[MethodTokenPosition]; } }
            internal object[] Args { get { return (object[])_args[ArgsPosition]; } }
            internal Type[] GenericTypes { get { return (Type[])_args[GenericTypesPosition]; } }
            internal object ReturnValue { /*get { return args[ReturnValuePosition]; }*/ set { _args[ReturnValuePosition] = value; } }
        }

        private class ProxyAssembly
        {
            private AssemblyDefinition _ab;
            private ModuleDefinition _mb;
            private int _typeId = 0;

            // Maintain a MethodBase-->int, int-->MethodBase mapping to permit generated code
            // to pass methods by token
            private Dictionary<MethodDefinition, int> _methodToToken = new Dictionary<MethodDefinition, int>();
            private List<MethodDefinition> _methodsByToken = new List<MethodDefinition>();
            private HashSet<string> _ignoresAccessAssemblyNames = new HashSet<string>();
            private ConstructorInfo _ignoresAccessChecksToAttributeConstructor;

            public ProxyAssembly()
            {
                _ab = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition("ProxyBuilder", new Version()), "testmod", ModuleKind.Dll);
                _mb = _ab.MainModule;

                //_ab = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("ProxyBuilder"), AssemblyBuilderAccess.Run);
                //_mb = _ab.DefineDynamicModule("testmod");
            }

            // Gets or creates the ConstructorInfo for the IgnoresAccessChecksAttribute.
            // This attribute is both defined and referenced in the dynamic assembly to
            // allow access to internal types in other assemblies.
            internal ConstructorInfo IgnoresAccessChecksAttributeConstructor
            {
                get
                {
                    //if (_ignoresAccessChecksToAttributeConstructor == null)
                    //{
                    //    _ignoresAccessChecksToAttributeConstructor = IgnoreAccessChecksToAttributeBuilder.AddToModule(_mb);
                    //}

                    return _ignoresAccessChecksToAttributeConstructor;
                }
            }
            public ProxyBuilder CreateProxy(string name, Type proxyBaseType)
            {
                //Read proxy Type in Cecil
                var proxyBaseTypeReference = _mb.ImportReference(proxyBaseType);

                int nextId = Interlocked.Increment(ref _typeId);
                
                TypeDefinition tb = new TypeDefinition(null, name + "_" + nextId, Mono.Cecil.TypeAttributes.Public, proxyBaseTypeReference);
                _mb.Types.Add(tb);
                //TypeBuilder tb = _mb.DefineType(name + "_" + nextId, TypeAttributes.Public, proxyBaseType);
                return new ProxyBuilder(this, tb, proxyBaseType, _mb);
            }

            public ModuleDefinition GetModuleDefinition()
            {
                return _mb;
            }

            //// Generates an instance of the IgnoresAccessChecksToAttribute to
            //// identify the given assembly as one which contains internal types
            //// the dynamic assembly will need to reference.
            //internal void GenerateInstanceOfIgnoresAccessChecksToAttribute(string assemblyName)
            //{
            //    // Add this assembly level attribute:
            //    // [assembly: System.Runtime.CompilerServices.IgnoresAccessChecksToAttribute(assemblyName)]
            //    ConstructorInfo attributeConstructor = IgnoresAccessChecksAttributeConstructor;
            //    CustomAttributeBuilder customAttributeBuilder =
            //        new CustomAttributeBuilder(attributeConstructor, new object[] { assemblyName });
            //    _ab.SetCustomAttribute(customAttributeBuilder);
            //}

            // Ensures the type we will reference from the dynamic assembly
            // is visible.  Non-public types need to emit an attribute that
            // allows access from the dynamic assembly.
            internal void EnsureTypeIsVisible(Type type)
            {
                TypeInfo typeInfo = type.GetTypeInfo();
                if (!typeInfo.IsVisible)
                {
                    string assemblyName = typeInfo.Assembly.GetName().Name;
                    if (!_ignoresAccessAssemblyNames.Contains(assemblyName))
                    {
                        //As we don't have System.Reflection.Emit, avoiding this case for Mono.Cecil
                        throw new PlatformNotSupportedException("Cannot emit attribute visibility for internal types. Please use a public type");


                        //GenerateInstanceOfIgnoresAccessChecksToAttribute(assemblyName);
                        //_ignoresAccessAssemblyNames.Add(assemblyName);
                    }
                }
            }

            internal void GetTokenForMethod(MethodDefinition method, out TypeDefinition type, out int token)
            {
                type = method.DeclaringType;
                token = 0;
                if (!_methodToToken.TryGetValue(method, out token))
                {
                    _methodsByToken.Add(method);
                    token = _methodsByToken.Count - 1;
                    _methodToToken[method] = token;
                }
            }

            internal MethodDefinition ResolveMethodToken(Type type, int token)
            {
                Debug.Assert(token >= 0 && token < _methodsByToken.Count);
                return _methodsByToken[token];
            }
        }

        private class ProxyBuilder
        {
            private static MethodDefinition s_delegateInvoke = null;
            //private static readonly MethodInfo s_delegateInvoke = typeof(Action<object[]>).GetTypeInfo().GetDeclaredMethod("Invoke");

            private ProxyAssembly _assembly;
            private TypeDefinition _tb;
            private static ModuleDefinition _mb;
            private Type _proxyBaseType;
            private TypeDefinition _proxyBaseDefinition;
            private List<FieldDefinition> _fields;

            internal ProxyBuilder(ProxyAssembly assembly, TypeDefinition tb, Type proxyBaseType, ModuleDefinition mb)
            {
                _assembly = assembly;
                _tb = tb;
                _mb = mb;
                _proxyBaseType = proxyBaseType;
                _proxyBaseDefinition = _mb.ImportReference(_proxyBaseType).Resolve();

                _fields = new List<FieldDefinition>();

                var actionObjectDelegate = _mb.ImportReference(typeof(Action<object[]>));
                s_delegateInvoke = actionObjectDelegate.Resolve().Methods.FirstOrDefault(p => p.Name == "Invoke");

                FieldDefinition fdef = new FieldDefinition("invoke", Mono.Cecil.FieldAttributes.Private, actionObjectDelegate);
                tb.Fields.Add(fdef);
                //_fields.Add(tb.DefineField("invoke", typeof(Action<object[]>), FieldAttributes.Private));
            }

            private void AddConstructor(TypeDefinition type, TypeDefinition[] args)
            {
                var baseCtor = _proxyBaseDefinition.Methods.SingleOrDefault(p => p.IsPublic && p.Parameters.Count == 0);

                var methodAttributes = Mono.Cecil.MethodAttributes.Public;
                var method = new MethodDefinition(".ctor", methodAttributes, _mb.ImportReference(typeof(void)));
                method.CallingConvention = MethodCallingConvention.ThisCall;
                method.Body.Instructions.Add(Instruction.Create(Mono.Cecil.Cil.OpCodes.Ldarg_0));
                method.Body.Instructions.Add(Instruction.Create(Mono.Cecil.Cil.OpCodes.Call, baseCtor));

                for (int i = 0; i < args.Length; i++)
                {
                    method.Body.Instructions.Add(Instruction.Create(Mono.Cecil.Cil.OpCodes.Ldarg_0));
                    method.Body.Instructions.Add(Instruction.Create(Mono.Cecil.Cil.OpCodes.Ldarg, i + 1));
                    method.Body.Instructions.Add(Instruction.Create(Mono.Cecil.Cil.OpCodes.Stfld, _fields[i]));
                }

                method.Body.Instructions.Add(Instruction.Create(Mono.Cecil.Cil.OpCodes.Ret));

                foreach (var arg in args)
                {
                    method.Parameters.Add(new ParameterDefinition(arg.Resolve()));
                }

                type.Methods.Add(method);
            }

            private void Complete()
            {
                TypeDefinition[] args = new TypeDefinition[_fields.Count];
                for (int i = 0; i < args.Length; i++)
                {
                    args[i] = _fields[i].FieldType.Resolve();
                }

                AddConstructor(_tb, args);


                //ConstructorBuilder cb = _tb.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, args);
                //ILGenerator il = cb.GetILGenerator();

                // chained ctor call
                //ConstructorInfo baseCtor = _proxyBaseType.GetTypeInfo().DeclaredConstructors.SingleOrDefault(c => c.IsPublic && c.GetParameters().Length == 0);
                //Debug.Assert(baseCtor != null);

                //il.Emit(OpCodes.Ldarg_0);
                //il.Emit(OpCodes.Call, baseCtor);

                // store all the fields
                //for (int i = 0; i < args.Length; i++)
                //{
                //    il.Emit(OpCodes.Ldarg_0);
                //    il.Emit(OpCodes.Ldarg, i + 1);
                //    il.Emit(OpCodes.Stfld, _fields[i]);
                //}

                //il.Emit(OpCodes.Ret);
            }

            internal Type CreateType()
            {
                this.Complete();

                return Type.GetType($@"{_tb.FullName}, {_tb.Module.Assembly.FullName}");

                //return _tb.CreateTypeInfo().AsType();
            }

            internal void AddInterfaceImpl(Type iface)
            {
                // If necessary, generate an attribute to permit visibility
                // to internal types.
                _assembly.EnsureTypeIsVisible(iface);

                var ifaceRef = _mb.ImportReference(iface);
                var ifaceDef = ifaceRef.Resolve();

                _tb.Interfaces.Add(new InterfaceImplementation(ifaceDef));

                //_tb.AddInterfaceImplementation(iface);


                // AccessorMethods -> Metadata mappings.
                var propertyMap = new Dictionary<MethodDefinition, PropertyAccessorInfo>(MethodInfoEqualityComparer.Instance);
                //var propertyMap = new Dictionary<MethodInfo, PropertyAccessorInfo>(MethodInfoEqualityComparer.Instance);

                foreach (var pi in ifaceDef.Properties)
                {
                    var ai = new PropertyAccessorInfo(pi.GetMethod, pi.SetMethod);
                    if (pi.GetMethod != null)
                        propertyMap[pi.GetMethod] = ai;
                    if (pi.SetMethod != null)
                        propertyMap[pi.SetMethod] = ai;
                }

                //foreach (PropertyInfo pi in iface.GetRuntimeProperties())
                //{
                //    var ai = new PropertyAccessorInfo(pi.GetMethod, pi.SetMethod);
                //    if (pi.GetMethod != null)
                //        propertyMap[pi.GetMethod] = ai;
                //    if (pi.SetMethod != null)
                //        propertyMap[pi.SetMethod] = ai;
                //}

                var eventMap = new Dictionary<MethodDefinition, EventAccessorInfo>(MethodInfoEqualityComparer.Instance);
                foreach (var ei in ifaceDef.Events)
                {
                    var ai = new EventAccessorInfo(ei.AddMethod, ei.RemoveMethod, ei.InvokeMethod);
                    if (ei.AddMethod != null)
                        eventMap[ei.AddMethod] = ai;
                    if (ei.RemoveMethod != null)
                        eventMap[ei.RemoveMethod] = ai;
                    if (ei.InvokeMethod != null)
                        eventMap[ei.InvokeMethod] = ai;
                }


                //var eventMap = new Dictionary<MethodInfo, EventAccessorInfo>(MethodInfoEqualityComparer.Instance);
                //foreach (EventInfo ei in iface.GetRuntimeEvents())
                //{
                //    var ai = new EventAccessorInfo(ei.AddMethod, ei.RemoveMethod, ei.RaiseMethod);
                //    if (ei.AddMethod != null)
                //        eventMap[ei.AddMethod] = ai;
                //    if (ei.RemoveMethod != null)
                //        eventMap[ei.RemoveMethod] = ai;
                //    if (ei.RaiseMethod != null)
                //        eventMap[ei.RaiseMethod] = ai;
                //}

                foreach (var mi in ifaceDef.Methods)
                {
                    MethodDefinition mdb = AddMethodImpl(mi);
                    PropertyAccessorInfo associatedProperty;
                    if (propertyMap.TryGetValue(mi, out associatedProperty))
                    {
                        if (MethodInfoEqualityComparer.Instance.Equals(associatedProperty.InterfaceGetMethod, mi))
                            associatedProperty.GetMethodBuilder = mdb;
                        else
                            associatedProperty.SetMethodBuilder = mdb;
                    }

                    EventAccessorInfo associatedEvent;
                    if (eventMap.TryGetValue(mi, out associatedEvent))
                    {
                        if (MethodInfoEqualityComparer.Instance.Equals(associatedEvent.InterfaceAddMethod, mi))
                            associatedEvent.AddMethodBuilder = mdb;
                        else if (MethodInfoEqualityComparer.Instance.Equals(associatedEvent.InterfaceRemoveMethod, mi))
                            associatedEvent.RemoveMethodBuilder = mdb;
                        else
                            associatedEvent.RaiseMethodBuilder = mdb;
                    }
                }

                //foreach (MethodInfo mi in iface.GetRuntimeMethods())
                //{
                //    MethodBuilder mdb = AddMethodImpl(mi);
                //    PropertyAccessorInfo associatedProperty;
                //    if (propertyMap.TryGetValue(mi, out associatedProperty))
                //    {
                //        if (MethodInfoEqualityComparer.Instance.Equals(associatedProperty.InterfaceGetMethod, mi))
                //            associatedProperty.GetMethodBuilder = mdb;
                //        else
                //            associatedProperty.SetMethodBuilder = mdb;
                //    }

                //    EventAccessorInfo associatedEvent;
                //    if (eventMap.TryGetValue(mi, out associatedEvent))
                //    {
                //        if (MethodInfoEqualityComparer.Instance.Equals(associatedEvent.InterfaceAddMethod, mi))
                //            associatedEvent.AddMethodBuilder = mdb;
                //        else if (MethodInfoEqualityComparer.Instance.Equals(associatedEvent.InterfaceRemoveMethod, mi))
                //            associatedEvent.RemoveMethodBuilder = mdb;
                //        else
                //            associatedEvent.RaiseMethodBuilder = mdb;
                //    }
                //}

                foreach (var pi in ifaceDef.Properties)
                {
                    PropertyAccessorInfo ai = propertyMap[pi.GetMethod ?? pi.SetMethod];

                    PropertyDefinition propDef = new PropertyDefinition(pi.Name, pi.Attributes, pi.PropertyType);
                    foreach (var propParam in pi.Parameters)
                    {
                        propDef.Parameters.Add(propParam);
                    }
                    _tb.Properties.Add(propDef);

                    //PropertyBuilder pb = _tb.DefineProperty(pi.Name, pi.Attributes, pi.PropertyType, pi.GetIndexParameters().Select(p => p.ParameterType).ToArray());

                    if (ai.GetMethodBuilder != null)
                        propDef.GetMethod = ai.GetMethodBuilder;
                    if (ai.SetMethodBuilder != null)
                        propDef.SetMethod = ai.SetMethodBuilder;
                }

                foreach (var ei in ifaceDef.Events)
                {
                    EventAccessorInfo ai = eventMap[ei.AddMethod ?? ei.RemoveMethod];

                    EventDefinition ed = new EventDefinition(ei.Name, ei.Attributes, ei.EventType);
                    _tb.Events.Add(ed);
                    //EventBuilder eb = _tb.DefineEvent(ei.Name, ei.Attributes, ei.EventHandlerType);


                    if (ai.AddMethodBuilder != null)
                        ed.AddMethod = ai.AddMethodBuilder;
                    if (ai.RemoveMethodBuilder != null)
                        ed.RemoveMethod = ai.RemoveMethodBuilder;
                    if (ai.RaiseMethodBuilder != null)
                        ed.InvokeMethod = ai.RaiseMethodBuilder;
                }
            }

            private MethodDefinition AddMethodImpl(MethodDefinition mid)
            {
                //ParameterInfo[] parameters = mi.GetParameters();
                var parameters = mid.Parameters.ToArray();
                TypeDefinition[] paramTypes = ParamTypes(parameters, false);

                MethodDefinition mdb = new MethodDefinition(mid.Name, Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.Virtual, mid.ReturnType);

                foreach (var paramType in paramTypes)
                {
                    mdb.Parameters.Add(new ParameterDefinition(paramType));
                }
                
                _tb.Methods.Add(mdb);

                //MethodBuilder mdb = _tb.DefineMethod(mi.Name, MethodAttributes.Public | MethodAttributes.Virtual, mi.ReturnType, paramTypes);
                if (mid.HasGenericParameters)
                {
                    var ts = mid.GenericParameters.ToArray();
                    string[] ss = new string[ts.Length];
                    for (int i = 0; i < ts.Length; i++)
                    {
                        ss[i] = ts[i].Name;
                    }

                    
                    for (int i = 0; i < ts.Length; i++)
                    {
                        var gp = new GenericParameter(ts[i].Name, mdb);
                        gp.Attributes =  ts[i].Attributes;
                        mdb.GenericParameters.Add(gp);
                    }

                    //GenericTypeParameterBuilder[] genericParameters = mdb.DefineGenericParameters(ss);
                    //for (int i = 0; i < genericParameters.Length; i++)
                    //{
                    //    genericParameters[i].SetGenericParameterAttributes(ts[i].GetTypeInfo().GenericParameterAttributes);
                    //}
                }

                //Workaround Mono.Cecil for GenericParameter https://stackoverflow.com/a/9440212
                //mdb.ReturnType = mid.ReturnType;

                var il = mdb.Body.GetILProcessor();

                //ILGenerator il = mdb.GetILGenerator();

                ParametersArray args = new ParametersArray(il, paramTypes);

                // object[] args = new object[paramCount];
                il.Emit(Mono.Cecil.Cil.OpCodes.Nop);
                GenericArray<object> argsArr = new GenericArray<object>(il, ParamTypes(parameters, true).Length);

                for (int i = 0; i < parameters.Length; i++)
                {
                    // args[i] = argi;
                    if (!parameters[i].IsOut)
                    {
                        argsArr.BeginSet(i);
                        args.Get(i);
                        argsArr.EndSet(parameters[i].ParameterType.Resolve());
                    }
                }

                // object[] packed = new object[PackedArgs.PackedTypes.Length];
                GenericArray<object> packedArr = new GenericArray<object>(il, PackedArgs.PackedTypes.Length);

                // packed[PackedArgs.DispatchProxyPosition] = this;
                packedArr.BeginSet(PackedArgs.DispatchProxyPosition);
                il.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
                packedArr.EndSet(_mb.ImportReference(typeof(DispatchProxy)).Resolve());

                // packed[PackedArgs.DeclaringTypePosition] = typeof(iface);

                var RunTimeTypeHandlerType = _mb.ImportReference(typeof(RuntimeTypeHandle));
                MethodDefinition Type_GetTypeFromHandle = _mb.ImportReference(typeof(Type)).Resolve().Methods.Where(p => p.Name == "GetTypeFromHandle" && p.Parameters.Count == 1 && p.Parameters[0].ParameterType == RunTimeTypeHandlerType).FirstOrDefault();
                //MethodInfo Type_GetTypeFromHandle = typeof(Type).GetRuntimeMethod("GetTypeFromHandle", new Type[] { typeof(RuntimeTypeHandle) });

                int methodToken;
                TypeDefinition declaringType;
                _assembly.GetTokenForMethod(mid, out declaringType, out methodToken);
                packedArr.BeginSet(PackedArgs.DeclaringTypePosition);
                il.Emit(Mono.Cecil.Cil.OpCodes.Ldtoken, declaringType);
                il.Emit(Mono.Cecil.Cil.OpCodes.Call, Type_GetTypeFromHandle);
                packedArr.EndSet(_mb.ImportReference(typeof(object)).Resolve());

                // packed[PackedArgs.MethodTokenPosition] = iface method token;
                packedArr.BeginSet(PackedArgs.MethodTokenPosition);
                il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, methodToken);
                packedArr.EndSet(_mb.ImportReference(typeof(Int32)).Resolve());

                // packed[PackedArgs.ArgsPosition] = args;
                packedArr.BeginSet(PackedArgs.ArgsPosition);
                argsArr.Load();
                packedArr.EndSet(_mb.ImportReference(typeof(object[])).Resolve());

                // packed[PackedArgs.GenericTypesPosition] = mi.GetGenericArguments();
                if (mid.HasGenericParameters)
                {
                    packedArr.BeginSet(PackedArgs.GenericTypesPosition);
                    var genericTypes = mid.GenericParameters.ToArray();
                    GenericArray<Type> typeArr = new GenericArray<Type>(il, genericTypes.Length);
                    for (int i = 0; i < genericTypes.Length; ++i)
                    {
                        typeArr.BeginSet(i);
                        il.Emit(Mono.Cecil.Cil.OpCodes.Ldtoken, genericTypes[i]);
                        il.Emit(Mono.Cecil.Cil.OpCodes.Call, Type_GetTypeFromHandle);
                        typeArr.EndSet(_mb.ImportReference(typeof(Type)).Resolve());
                    }
                    typeArr.Load();
                    packedArr.EndSet(_mb.ImportReference(typeof(Type[])).Resolve());
                }

                // Call static DispatchProxyHelper.Invoke(object[])
                il.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
                il.Emit(Mono.Cecil.Cil.OpCodes.Ldfld, _fields[InvokeActionFieldAndCtorParameterIndex]); // delegate
                packedArr.Load();
                il.Emit(Mono.Cecil.Cil.OpCodes.Call, s_delegateInvoke);

                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].ParameterType.IsByReference)
                    {
                        args.BeginSet(i);
                        argsArr.Get(i);
                        args.EndSet(i, _mb.ImportReference(typeof(object)).Resolve());
                    }
                }

                if (mid.ReturnType != _mb.ImportReference(typeof(void)))
                {
                    packedArr.Get(PackedArgs.ReturnValuePosition);
                    Convert(il, _mb.ImportReference(typeof(object)).Resolve(), mid.ReturnType.Resolve(), false);
                }

                il.Emit(Mono.Cecil.Cil.OpCodes.Ret);

                mdb.Overrides.Add(mid);
                //_tb.DefineMethodOverride(mdb, mi);
                return mdb;
            }

            private static TypeDefinition[] ParamTypes(ParameterDefinition[] parms, bool noByRef)
            {
                TypeReference[] types = new TypeReference[parms.Length];
                for (int i = 0; i < parms.Length; i++)
                {
                    types[i] = parms[i].ParameterType;
                    if (noByRef && types[i].IsByReference)
                        types[i] = types[i].GetElementType();
                }

                TypeDefinition[] typeDef = new TypeDefinition[types.Length];
                for (int i = 0; i < types.Length; i++)
                {
                    typeDef[i] = types[i].Resolve();
                }

                return typeDef;
            }

            private static Dictionary<Type, TypeDefinition> TypeCodeCache = null;

            // TypeCode does not exist in ProjectK or ProjectN.
            // This lookup method was copied from PortableLibraryThunks\Internal\PortableLibraryThunks\System\TypeThunks.cs
            // but returns the integer value equivalent to its TypeCode enum.
            private static int GetTypeCode(TypeDefinition type)
            {
                if (TypeCodeCache == null)
                {
                    TypeCodeCache = new Dictionary<Type, TypeDefinition>()
                    {
                        {  typeof(Boolean), _mb.ImportReference(typeof(Boolean)).Resolve() },
                        {  typeof(Char), _mb.ImportReference(typeof(Char)).Resolve() },
                        {  typeof(SByte), _mb.ImportReference(typeof(SByte)).Resolve() },
                        {  typeof(Byte), _mb.ImportReference(typeof(Byte)).Resolve() },
                        {  typeof(Int16), _mb.ImportReference(typeof(Int16)).Resolve() },
                        {  typeof(UInt16), _mb.ImportReference(typeof(UInt16)).Resolve() },
                        {  typeof(Int32), _mb.ImportReference(typeof(Int32)).Resolve() },
                        {  typeof(UInt32), _mb.ImportReference(typeof(UInt32)).Resolve() },
                        {  typeof(Int64), _mb.ImportReference(typeof(Int64)).Resolve() },
                        {  typeof(UInt64), _mb.ImportReference(typeof(UInt64)).Resolve() },
                        {  typeof(Single), _mb.ImportReference(typeof(Single)).Resolve() },
                        {  typeof(Double), _mb.ImportReference(typeof(Double)).Resolve() },
                        {  typeof(Decimal), _mb.ImportReference(typeof(Decimal)).Resolve() },
                        {  typeof(DateTime), _mb.ImportReference(typeof(DateTime)).Resolve() },
                        {  typeof(String), _mb.ImportReference(typeof(String)).Resolve() },
                    };
                }

                if (type == null)
                    return 0;   // TypeCode.Empty;

                if (type == TypeCodeCache[typeof(Boolean)])
                    return 3;   // TypeCode.Boolean;

                if (type == TypeCodeCache[typeof(Char)])
                    return 4;   // TypeCode.Char;

                if (type == TypeCodeCache[typeof(SByte)])
                    return 5;   // TypeCode.SByte;

                if (type == TypeCodeCache[typeof(Byte)])
                    return 6;   // TypeCode.Byte;

                if (type == TypeCodeCache[typeof(Int16)])
                    return 7;   // TypeCode.Int16;

                if (type == TypeCodeCache[typeof(UInt16)])
                    return 8;   // TypeCode.UInt16;

                if (type == TypeCodeCache[typeof(Int32)])
                    return 9;   // TypeCode.Int32;

                if (type == TypeCodeCache[typeof(UInt32)])
                    return 10;  // TypeCode.UInt32;

                if (type == TypeCodeCache[typeof(Int64)])
                    return 11;  // TypeCode.Int64;

                if (type == TypeCodeCache[typeof(UInt64)])
                    return 12;  // TypeCode.UInt64;

                if (type == TypeCodeCache[typeof(Single)])
                    return 13;  // TypeCode.Single;

                if (type == TypeCodeCache[typeof(Double)])
                    return 14;  // TypeCode.Double;

                if (type == TypeCodeCache[typeof(Decimal)])
                    return 15;  // TypeCode.Decimal;

                if (type == TypeCodeCache[typeof(DateTime)])
                    return 16;  // TypeCode.DateTime;

                if (type == TypeCodeCache[typeof(String)])
                    return 18;  // TypeCode.String;

                //WORKAROUND: Did not find Enum.GetUnderlyingType for Enum. Returning Int32 as Type for most common cases
                if (type.IsEnum)
                    return GetTypeCode(TypeCodeCache[typeof(Int32)]);

                return 1;   // TypeCode.Object;
            }

            private static Mono.Cecil.Cil.OpCode[] s_convOpCodes = new Mono.Cecil.Cil.OpCode[] {
                Mono.Cecil.Cil.OpCodes.Nop,//Empty = 0,
                Mono.Cecil.Cil.OpCodes.Nop,//Object = 1,
                Mono.Cecil.Cil.OpCodes.Nop,//DBNull = 2,
                Mono.Cecil.Cil.OpCodes.Conv_I1,//Boolean = 3,
                Mono.Cecil.Cil.OpCodes.Conv_I2,//Char = 4,
                Mono.Cecil.Cil.OpCodes.Conv_I1,//SByte = 5,
                Mono.Cecil.Cil.OpCodes.Conv_U1,//Byte = 6,
                Mono.Cecil.Cil.OpCodes.Conv_I2,//Int16 = 7,
                Mono.Cecil.Cil.OpCodes.Conv_U2,//UInt16 = 8,
                Mono.Cecil.Cil.OpCodes.Conv_I4,//Int32 = 9,
                Mono.Cecil.Cil.OpCodes.Conv_U4,//UInt32 = 10,
                Mono.Cecil.Cil.OpCodes.Conv_I8,//Int64 = 11,
                Mono.Cecil.Cil.OpCodes.Conv_U8,//UInt64 = 12,
                Mono.Cecil.Cil.OpCodes.Conv_R4,//Single = 13,
                Mono.Cecil.Cil.OpCodes.Conv_R8,//Double = 14,
                Mono.Cecil.Cil.OpCodes.Nop,//Decimal = 15,
                Mono.Cecil.Cil.OpCodes.Nop,//DateTime = 16,
                Mono.Cecil.Cil.OpCodes.Nop,//17
                Mono.Cecil.Cil.OpCodes.Nop,//String = 18,
            };

            private static Mono.Cecil.Cil.OpCode[] s_ldindOpCodes = new Mono.Cecil.Cil.OpCode[] {
                Mono.Cecil.Cil.OpCodes.Nop,//Empty = 0,
                Mono.Cecil.Cil.OpCodes.Nop,//Object = 1,
                Mono.Cecil.Cil.OpCodes.Nop,//DBNull = 2,
                Mono.Cecil.Cil.OpCodes.Ldind_I1,//Boolean = 3,
                Mono.Cecil.Cil.OpCodes.Ldind_I2,//Char = 4,
                Mono.Cecil.Cil.OpCodes.Ldind_I1,//SByte = 5,
                Mono.Cecil.Cil.OpCodes.Ldind_U1,//Byte = 6,
                Mono.Cecil.Cil.OpCodes.Ldind_I2,//Int16 = 7,
                Mono.Cecil.Cil.OpCodes.Ldind_U2,//UInt16 = 8,
                Mono.Cecil.Cil.OpCodes.Ldind_I4,//Int32 = 9,
                Mono.Cecil.Cil.OpCodes.Ldind_U4,//UInt32 = 10,
                Mono.Cecil.Cil.OpCodes.Ldind_I8,//Int64 = 11,
                Mono.Cecil.Cil.OpCodes.Ldind_I8,//UInt64 = 12,
                Mono.Cecil.Cil.OpCodes.Ldind_R4,//Single = 13,
                Mono.Cecil.Cil.OpCodes.Ldind_R8,//Double = 14,
                Mono.Cecil.Cil.OpCodes.Nop,//Decimal = 15,
                Mono.Cecil.Cil.OpCodes.Nop,//DateTime = 16,
                Mono.Cecil.Cil.OpCodes.Nop,//17
                Mono.Cecil.Cil.OpCodes.Ldind_Ref,//String = 18,
            };

            private static Mono.Cecil.Cil.OpCode[] s_stindOpCodes = new Mono.Cecil.Cil.OpCode[] {
                Mono.Cecil.Cil.OpCodes.Nop,//Empty = 0,
                Mono.Cecil.Cil.OpCodes.Nop,//Object = 1,
                Mono.Cecil.Cil.OpCodes.Nop,//DBNull = 2,
                Mono.Cecil.Cil.OpCodes.Stind_I1,//Boolean = 3,
                Mono.Cecil.Cil.OpCodes.Stind_I2,//Char = 4,
                Mono.Cecil.Cil.OpCodes.Stind_I1,//SByte = 5,
                Mono.Cecil.Cil.OpCodes.Stind_I1,//Byte = 6,
                Mono.Cecil.Cil.OpCodes.Stind_I2,//Int16 = 7,
                Mono.Cecil.Cil.OpCodes.Stind_I2,//UInt16 = 8,
                Mono.Cecil.Cil.OpCodes.Stind_I4,//Int32 = 9,
                Mono.Cecil.Cil.OpCodes.Stind_I4,//UInt32 = 10,
                Mono.Cecil.Cil.OpCodes.Stind_I8,//Int64 = 11,
                Mono.Cecil.Cil.OpCodes.Stind_I8,//UInt64 = 12,
                Mono.Cecil.Cil.OpCodes.Stind_R4,//Single = 13,
                Mono.Cecil.Cil.OpCodes.Stind_R8,//Double = 14,
                Mono.Cecil.Cil.OpCodes.Nop,//Decimal = 15,
                Mono.Cecil.Cil.OpCodes.Nop,//DateTime = 16,
                Mono.Cecil.Cil.OpCodes.Nop,//17
                Mono.Cecil.Cil.OpCodes.Stind_Ref,//String = 18,
            };

            private static void Convert(ILProcessor il, TypeDefinition source, TypeDefinition target, bool isAddress)
            {
                Debug.Assert(!target.IsByReference);
                if (target == source)
                    return;

                var sourceTypeInfo = source;
                var targetTypeInfo = target;

                if (source.IsByReference)
                {
                    Debug.Assert(!isAddress);
                    TypeDefinition argType = source.GetElementType().Resolve();
                    Ldind(il, argType);
                    Convert(il, argType, target, isAddress);
                    return;
                }
                if (targetTypeInfo.IsValueType)
                {
                    if (sourceTypeInfo.IsValueType)
                    {
                        Mono.Cecil.Cil.OpCode opCode = s_convOpCodes[GetTypeCode(target)];
                        Debug.Assert(!opCode.Equals(Mono.Cecil.Cil.OpCodes.Nop));
                        il.Emit(opCode);
                    }
                    else
                    {
                        Debug.Assert(sourceTypeInfo.IsAssignableFrom(targetTypeInfo));
                        il.Emit(Mono.Cecil.Cil.OpCodes.Unbox, target);
                        if (!isAddress)
                            Ldind(il, target);
                    }
                }
                else if (targetTypeInfo.IsAssignableFrom(sourceTypeInfo))
                {
                    if (sourceTypeInfo.IsValueType || source.IsGenericParameter)
                    {
                        if (isAddress)
                            Ldind(il, source);
                        il.Emit(Mono.Cecil.Cil.OpCodes.Box, _mb.ImportReference(source));
                    }
                }
                else
                {
                    Debug.Assert(sourceTypeInfo.IsAssignableFrom(targetTypeInfo) || targetTypeInfo.IsInterface || sourceTypeInfo.IsInterface);
                    if (target.IsGenericParameter)
                    {
                        il.Emit(Mono.Cecil.Cil.OpCodes.Unbox_Any, _mb.ImportReference(target));
                    }
                    else
                    {
                        il.Emit(Mono.Cecil.Cil.OpCodes.Castclass, _mb.ImportReference(target));
                    }
                }
            }

            private static void Ldind(ILProcessor il, TypeDefinition type)
            {
                Mono.Cecil.Cil.OpCode opCode = s_ldindOpCodes[GetTypeCode(type)];
                if (!opCode.Equals(Mono.Cecil.Cil.OpCodes.Nop))
                {
                    il.Emit(opCode);
                }
                else
                {
                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldobj, _mb.ImportReference(type));
                }
            }

            private static void Stind(ILProcessor il, TypeDefinition type)
            {
                Mono.Cecil.Cil.OpCode opCode = s_stindOpCodes[GetTypeCode(type)];
                if (!opCode.Equals(Mono.Cecil.Cil.OpCodes.Nop))
                {
                    il.Emit(opCode);
                }
                else
                {
                    il.Emit(Mono.Cecil.Cil.OpCodes.Stobj, _mb.ImportReference(type));
                }
            }

            private class ParametersArray
            {
                private ILProcessor _il;
                private TypeDefinition[] _paramTypes;
                internal ParametersArray(ILProcessor il, TypeDefinition[] paramTypes)
                {
                    _il = il;
                    _paramTypes = paramTypes;
                }

                internal void Get(int i)
                {
                    _il.Emit(Mono.Cecil.Cil.OpCodes.Ldarg, i + 1);
                }

                internal void BeginSet(int i)
                {
                    _il.Emit(Mono.Cecil.Cil.OpCodes.Ldarg, i + 1);
                }

                internal void EndSet(int i, TypeDefinition stackType)
                {
                    Debug.Assert(_paramTypes[i].IsByReference);
                    TypeReference argType = _paramTypes[i].GetElementType();
                    TypeDefinition argTypeDef = argType.Resolve();
                    Convert(_il, stackType, argTypeDef, false);
                    Stind(_il, argTypeDef);
                }
            }

            private class GenericArray<T>
            {
                private ILProcessor _il;
                private VariableDefinition _lb;
                internal GenericArray(ILProcessor il, int len)
                {
                    _il = il;
                    var genericTRef = _mb.ImportReference(typeof(T));
                    _lb = new VariableDefinition(genericTRef);
                    il.Body.Variables.Add(_lb);
                    //_lb = il.DeclareLocaltypeof(T[]);

                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, len);
                    il.Emit(Mono.Cecil.Cil.OpCodes.Newarr, genericTRef);
                    il.Emit(Mono.Cecil.Cil.OpCodes.Stloc, _lb);
                }

                internal void Load()
                {
                    _il.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, _lb);
                }

                internal void Get(int i)
                {
                    _il.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, _lb);
                    _il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, i);
                    _il.Emit(Mono.Cecil.Cil.OpCodes.Ldelem_Ref);
                }

                internal void BeginSet(int i)
                {
                    _il.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, _lb);
                    _il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, i);
                }

                internal void EndSet(TypeDefinition stackType)
                {
                    var targetType = _mb.ImportReference(typeof(T)).Resolve();

                    Convert(_il, stackType, targetType, false);
                    _il.Emit(Mono.Cecil.Cil.OpCodes.Stelem_Ref);
                }
            }

            private sealed class PropertyAccessorInfo
            {
                public MethodDefinition InterfaceGetMethod { get; }
                public MethodDefinition InterfaceSetMethod { get; }
                public MethodDefinition GetMethodBuilder { get; set; }
                public MethodDefinition SetMethodBuilder { get; set; }

                public PropertyAccessorInfo(MethodDefinition interfaceGetMethod, MethodDefinition interfaceSetMethod)
                {
                    InterfaceGetMethod = interfaceGetMethod;
                    InterfaceSetMethod = interfaceSetMethod;
                }
            }

            private sealed class EventAccessorInfo
            {
                public MethodDefinition InterfaceAddMethod { get; }
                public MethodDefinition InterfaceRemoveMethod { get; }
                public MethodDefinition InterfaceRaiseMethod { get; }
                public MethodDefinition AddMethodBuilder { get; set; }
                public MethodDefinition RemoveMethodBuilder { get; set; }
                public MethodDefinition RaiseMethodBuilder { get; set; }

                public EventAccessorInfo(MethodDefinition interfaceAddMethod, MethodDefinition interfaceRemoveMethod, MethodDefinition interfaceRaiseMethod)
                {
                    InterfaceAddMethod = interfaceAddMethod;
                    InterfaceRemoveMethod = interfaceRemoveMethod;
                    InterfaceRaiseMethod = interfaceRaiseMethod;
                }
            }

            private sealed class MethodInfoEqualityComparer : EqualityComparer<MethodDefinition>
            {
                public static readonly MethodInfoEqualityComparer Instance = new MethodInfoEqualityComparer();

                private MethodInfoEqualityComparer() { }

                public sealed override bool Equals(MethodDefinition left, MethodDefinition right)
                {
                    if (ReferenceEquals(left, right))
                        return true;

                    if (left == null)
                        return right == null;
                    else if (right == null)
                        return false;

                    // This assembly should work in netstandard1.3,
                    // so we cannot use MemberInfo.MetadataToken here.
                    // Therefore, it compares honestly referring ECMA-335 I.8.6.1.6 Signature Matching.
                    if (!Equals(left.DeclaringType, right.DeclaringType))
                        return false;

                    if (!Equals(left.ReturnType, right.ReturnType))
                        return false;

                    if (left.CallingConvention != right.CallingConvention)
                        return false;

                    if (left.IsStatic != right.IsStatic)
                        return false;

                    if (left.Name != right.Name)
                        return false;

                    GenericParameter[] leftGenericParameters = new GenericParameter[0];
                    GenericParameter[] rightGenericParameters = new GenericParameter[0];

                    if (left.HasGenericParameters)
                        leftGenericParameters = left.GenericParameters.ToArray();

                    if (right.HasGenericParameters)
                        rightGenericParameters = right.GenericParameters.ToArray();

                    if (leftGenericParameters.Length != rightGenericParameters.Length)
                        return false;

                    for (int i = 0; i < leftGenericParameters.Length; i++)
                    {
                        if (!Equals(leftGenericParameters[i], rightGenericParameters[i]))
                            return false;
                    }

                    ParameterDefinition[] leftParameters = left.Parameters.ToArray();
                    ParameterDefinition[] rightParameters = right.Parameters.ToArray();
                    if (leftParameters.Length != rightParameters.Length)
                        return false;

                    for (int i = 0; i < leftParameters.Length; i++)
                    {
                        if (!Equals(leftParameters[i].ParameterType, rightParameters[i].ParameterType))
                            return false;
                    }

                    return true;
                }

                public sealed override int GetHashCode(MethodDefinition obj)
                {
                    if (obj == null)
                        return 0;

                    int hashCode = obj.DeclaringType.GetHashCode();
                    hashCode ^= obj.Name.GetHashCode();
                    foreach (var parameter in obj.Parameters)
                    {
                        hashCode ^= parameter.ParameterType.GetHashCode();
                    }

                    return hashCode;
                }
            }

            //private sealed class MethodInfoEqualityComparer : EqualityComparer<MethodInfo>
            //{
            //    public static readonly MethodInfoEqualityComparer Instance = new MethodInfoEqualityComparer();

            //    private MethodInfoEqualityComparer() { }

            //    public sealed override bool Equals(MethodInfo left, MethodInfo right)
            //    {
            //        if (ReferenceEquals(left, right))
            //            return true;

            //        if (left == null)
            //            return right == null;
            //        else if (right == null)
            //            return false;

            //        // This assembly should work in netstandard1.3,
            //        // so we cannot use MemberInfo.MetadataToken here.
            //        // Therefore, it compares honestly referring ECMA-335 I.8.6.1.6 Signature Matching.
            //        if (!Equals(left.DeclaringType, right.DeclaringType))
            //            return false;

            //        if (!Equals(left.ReturnType, right.ReturnType))
            //            return false;

            //        if (left.CallingConvention != right.CallingConvention)
            //            return false;

            //        if (left.IsStatic != right.IsStatic)
            //            return false;

            //        if (left.Name != right.Name)
            //            return false;

            //        Type[] leftGenericParameters = left.GetGenericArguments();
            //        Type[] rightGenericParameters = right.GetGenericArguments();
            //        if (leftGenericParameters.Length != rightGenericParameters.Length)
            //            return false;

            //        for (int i = 0; i < leftGenericParameters.Length; i++)
            //        {
            //            if (!Equals(leftGenericParameters[i], rightGenericParameters[i]))
            //                return false;
            //        }

            //        ParameterInfo[] leftParameters = left.GetParameters();
            //        ParameterInfo[] rightParameters = right.GetParameters();
            //        if (leftParameters.Length != rightParameters.Length)
            //            return false;

            //        for (int i = 0; i < leftParameters.Length; i++)
            //        {
            //            if (!Equals(leftParameters[i].ParameterType, rightParameters[i].ParameterType))
            //                return false;
            //        }

            //        return true;
            //    }

            //    public sealed override int GetHashCode(MethodInfo obj)
            //    {
            //        if (obj == null)
            //            return 0;

            //        int hashCode = obj.DeclaringType.GetHashCode();
            //        hashCode ^= obj.Name.GetHashCode();
            //        foreach (ParameterInfo parameter in obj.GetParameters())
            //        {
            //            hashCode ^= parameter.ParameterType.GetHashCode();
            //        }

            //        return hashCode;
            //    }
            //}
        }
    }
}