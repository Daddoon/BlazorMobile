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
