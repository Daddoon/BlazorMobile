using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;

[assembly: InternalsVisibleTo("BlazorMobile")]
namespace BlazorMobile.Common.Interop
{
    internal static class MethodProxyHelper
    {
        private static MethodInfo ParseMethodInfo(MethodInfo method)
        {
            if (method == null)
                return null;

            if (method.IsGenericMethod && !method.IsGenericMethodDefinition)
            {
                //We should get the generic method definition in order to find the index on the Interface Map
                method = method.GetGenericMethodDefinition();
            }

            return method;
        }

        #region Class to Interface

        /// <summary>
        /// Get the corresponding interface MethodInfo from a class MethodInfo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static int GetInterfaceMethodIndex<TClass, TInterface>(MethodInfo classMethod)
            where TClass : class
            where TInterface : class
        {
            var type = typeof(TClass);

            if (type.IsInterface)
                return -1;

            var interfaceType = typeof(TInterface);

            return GetInterfaceMethodIndex(type, classMethod, interfaceType);
        }

        /// <summary>
        /// Get the corresponding interface MethodInfo from a class MethodInfo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static int GetInterfaceMethodIndex(Type targetType, MethodInfo classMethod, Type interfaceType)
        {
            classMethod = ParseMethodInfo(classMethod);

            if (targetType == null || targetType.IsInterface || classMethod == null)
                return -1;

            var map = targetType.GetInterfaceMap(interfaceType);
            var index = Array.IndexOf(map.TargetMethods, classMethod);

            return index;
        }

        /// <summary>
        /// Get the corresponding interface MethodInfo from a class MethodInfo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static MethodInfo GetInterfaceMethodInfo<TClass, TInterface>(MethodInfo classMethod)
            where TClass : class
            where TInterface : class
        {
            var type = typeof(TClass);

            if (type.IsInterface)
                return null;

            var interfaceType = typeof(TInterface);

            return GetInterfaceMethodInfo(type, classMethod, interfaceType);
        }

        /// <summary>
        /// Get the corresponding interface MethodInfo from a class MethodInfo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static MethodInfo GetInterfaceMethodInfo<TClass, TInterface>(int classMethodIndex)
        {
            var type = typeof(TClass);

            if (type.IsInterface)
                return null;

            var interfaceType = typeof(TInterface);

            return GetInterfaceMethodInfo(type, classMethodIndex, interfaceType);
        }

        /// <summary>
        /// Get the corresponding interface MethodInfo from a class MethodInfo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static MethodInfo GetInterfaceMethodInfo(Type targetType, MethodInfo classMethod, Type interfaceType)
        {
            if (targetType == null || targetType.IsInterface || classMethod == null)
                return null;

            var index = GetInterfaceMethodIndex(targetType, classMethod, interfaceType);
            if (index == -1)
                return null;

            var map = targetType.GetInterfaceMap(interfaceType);
            return map.InterfaceMethods[index];
        }

        /// <summary>
        /// Get the corresponding interface MethodInfo from a class MethodInfo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static MethodInfo GetInterfaceMethodInfo(Type targetType, int classMethodIndex, Type interfaceType)
        {
            if (targetType == null || targetType.IsInterface || classMethodIndex == -1)
                return null;

            var map = targetType.GetInterfaceMap(interfaceType);
            return map.InterfaceMethods[classMethodIndex];
        }

        #endregion

        #region Interface to Class

        /// <summary>
        /// Get the corresponding class MethodInfo from an interface MethodInfo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static int GetClassMethodIndex<T>(MethodInfo interfaceMethod) where T : class
        {
            var type = typeof(T);

            if (type.IsInterface)
                return -1;

            return GetClassMethodIndex(type, interfaceMethod);
        }

        /// <summary>
        /// Get the corresponding class MethodInfo from an interface MethodInfo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static int GetClassMethodIndex(Type targetType, MethodInfo interfaceMethod)
        {
            if (targetType == null || targetType.IsInterface || interfaceMethod == null)
                return -1;

            var map = targetType.GetInterfaceMap(interfaceMethod.DeclaringType);
            var index = Array.IndexOf(map.InterfaceMethods, interfaceMethod);

            return index;
        }

        /// <summary>
        /// Get the corresponding class MethodInfo from an interface MethodInfo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static MethodInfo GetClassMethodInfo<T>(MethodInfo interfaceMethod) where T : class
        {
            var type = typeof(T);

            if (type.IsInterface)
                return null;

            return GetClassMethodInfo(type, interfaceMethod);
        }

        /// <summary>
        /// Get the corresponding class MethodInfo from an interface MethodInfo index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static MethodInfo GetClassMethodInfo<TClass, TInterface>(int interfaceMethodIndex)
            where TClass : class
            where TInterface : class
        {
            Type type = typeof(TClass);
            Type interfaceType = typeof(TInterface);

            return GetClassMethodInfo(type, interfaceType, interfaceMethodIndex);
        }

        /// <summary>
        /// Get the corresponding class MethodInfo from an interface MethodInfo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static MethodInfo GetClassMethodInfo(Type targetType, MethodInfo interfaceMethod)
        {
            if (targetType == null || targetType.IsInterface || interfaceMethod == null)
                return null;

            var map = targetType.GetInterfaceMap(interfaceMethod.DeclaringType);

            var index = GetClassMethodIndex(targetType, interfaceMethod);

            if (index == -1)
            {
                //Method not found on interface
                return null;
            }

            return map.TargetMethods[index];
        }

        /// <summary>
        /// Get the corresponding class MethodInfo from an interface MethodInfo index
        /// WARNING: InterfeceMethod index behavior is not supported on UWP with .NET native toolchain enabled
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static MethodInfo GetClassMethodInfo(Type targetType, Type interfaceType, int interfaceMethodIndex)
        {
            if (targetType == null || targetType.IsInterface || interfaceMethodIndex == -1)
                return null;

            var map = targetType.GetInterfaceMap(interfaceType);

            return map.TargetMethods[interfaceMethodIndex];
        }

        private const string MethodNotFoundException = "Requested method was not found on native side. Check that your interfaces and class are updated on each project";

        private static MethodInfo SearchMethodInfo(Type targetType, MethodProxy methodProxy)
        {
            if (targetType == null || targetType.IsInterface)
                return null;

            var methods = targetType.GetRuntimeMethods().Where(p => p.Name == methodProxy.MethodName).ToList();
            if (methods.Count <= 0)
            {
                throw new InvalidOperationException(MethodNotFoundException);
            }
            else if (methods.Count == 1)
            {
                //The perfect usage, no ambiguity, there is only one possible result
                return methods[0];
            }
            else
            {
                //We must do some research according to the MethodProxy configuration
                //NOTE: Theses check will surely if comparing with Generic values.
                //No time to manage this yet, as end-user can rename methods and parameters if needed

                if (methodProxy.GenericTypes.Count() > 0)
                {
                    //This mean that our signature must have some generic parameters
                    methods = methods.Where(p => p.IsGenericMethod && p.GetGenericArguments().Count() == methodProxy.GenericTypes.Count()).ToList();
                }
                else
                {
                    methods = methods.Where(p => !p.IsGenericMethod).ToList();
                }


                //Retrying to parse
                if (methods.Count <= 0)
                {
                    throw new InvalidOperationException(MethodNotFoundException);
                }
                else if (methods.Count == 1)
                {
                    //An ideal usage we find one result
                    return methods[0];
                }

                //Trying to distinguish additional things, like number of parameters.
                //May fail with optional parameters in input ?
                methods = methods.Where(p => p.GetParameters().Count() == methodProxy.Parameters.Count()).ToList();

                if (methods.Count == 1)
                {
                    //An ideal usage we find one result
                    return methods[0];
                    
                }
                else
                {
                    throw new InvalidOperationException(MethodNotFoundException);
                }

                //TODO: Check per parameter type, ideally not generic one, and do something like a weight table
            }
        }

        /// <summary>
        /// Get the corresponding class MethodInfo from a MethodProxy
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="interfaceType"></param>
        /// <param name="methodProxy"></param>
        /// <returns></returns>
        public static MethodInfo GetClassMethodInfo(Type targetType, Type interfaceType, MethodProxy methodProxy)
        {
            //Even if we don't have access to Xamarin.Forms assembly here, BlazorDevice is initialized internally at BlazorMobile boot on native side
            //As this method call should probably be called on the native end first after Blazor app booted too, the values will already be set.
            switch (BlazorDevice.RuntimePlatform)
            {
                case BlazorDevice.UWP:
                    return SearchMethodInfo(targetType, methodProxy);
                default:
                    return GetClassMethodInfo(targetType, interfaceType, methodProxy.MethodIndex);
            }
        }

        #endregion

        #region Interface Only

        /// <summary>
        /// Get the index of the INTERFACE MethodInfo from a DispactherProxy class
        /// </summary>
        /// <param name="method"></param>
        /// <param name="genericProxy"></param>
        /// <returns></returns>
        public static int GetInterfaceMethodIndexFromGenericProxy(MethodInfo method, Type genericProxy)
        {
            method = ParseMethodInfo(method);

            if (method == null || method.DeclaringType.IsInterface == false)
                return -1;

            //ClassProxy does not match, we just want to self look the index on the Interface from the given
            //interface MethodInfo
            var map = genericProxy.GetInterfaceMap(method.DeclaringType);
            return Array.IndexOf(map.InterfaceMethods, method);
        }

        #endregion

        #region Other

        public static TypeProxy[] GetGenericTypesFromRuntimeMethod(MethodInfo method)
        {
            if (!method.IsGenericMethod)
                return new TypeProxy[0];

            if (method.IsGenericMethodDefinition)
            {
                throw new ArgumentException("In order to retrieve Generic Types from MethodInfo, the method should be the implemented one and not the generic definition");
            }

            var types = method.GetGenericArguments();
            List<TypeProxy> typeProxies = new List<TypeProxy>();

            foreach (var type in types)
            {
                TypeProxy typeProxy = new TypeProxy(type);
                typeProxies.Add(typeProxy);
            }

            return typeProxies.ToArray();
        }

        #endregion

    }
}
