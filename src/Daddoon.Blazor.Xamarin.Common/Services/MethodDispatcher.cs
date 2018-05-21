using Daddoon.Blazor.Xam.Common.Interop;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Daddoon.Blazor.Xam.Common.Attributes;
using Daddoon.Blazor.Xam.Common.Serialization;

namespace Daddoon.Blazor.Xam.Common.Services
{
    public static class MethodDispatcher
    {
        /// <summary>
        /// Fake type just to return void value wrapped around a simple Task type
        /// </summary>
        internal class IgnoredType
        {

        }

        #region VOID CALL

        public static Task CallVoidMethod(MethodBase method)
        {
            return InternalCallMethod<IgnoredType>(method, null, null);
        }

        public static Task CallVoidMethod(MethodBase method, params object[] args)
        {
            return InternalCallMethod<IgnoredType>(method, null, args);
        }

        #region SYNTACTIC SUGAR HELPER

        //Just a Generic argument signature for easier signature call

        public static Task CallVoidMethod<TGenericArg>(MethodBase method, params object[] args)
        {
            return InternalCallMethod<IgnoredType>(
                method,
                new Type[] { typeof(TGenericArg) }
                , args);
        }

        public static Task CallVoidMethod<TGenericArg1, TGenericArg2>(MethodBase method, params object[] args)
        {
            return InternalCallMethod<IgnoredType>(
                method,
                new Type[] { typeof(TGenericArg1), typeof(TGenericArg2) }
                , args);
        }

        public static Task CallVoidMethod<TGenericArg1, TGenericArg2, TGenericArg3>(MethodBase method, params object[] args)
        {
            return InternalCallMethod<IgnoredType>(
                method,
                new Type[] { typeof(TGenericArg1), typeof(TGenericArg2), typeof(TGenericArg3) }
                , args);
        }

        public static Task CallVoidMethod<TGenericArg1, TGenericArg2, TGenericArg3, TGenericArg4>(MethodBase method, params object[] args)
        {
            return InternalCallMethod<IgnoredType>(
                method,
                new Type[] { typeof(TGenericArg1), typeof(TGenericArg2), typeof(TGenericArg4) }
                , args);
        }

        #endregion

        public static Task CallVoidMethod(MethodBase method, Type[] genericParameters, params object[] args)
        {
            return InternalCallMethod<IgnoredType>(method, genericParameters, args);
        }

        #endregion


        #region WITH RETURN VALUE

        public static Task<TReturnType> CallMethod<TReturnType>(MethodBase method)
        {
            return InternalCallMethod<TReturnType>(method, null, null);
        }

        public static Task<TReturnType> CallMethod<TReturnType>(MethodBase method, params object[] args)
        {
            return InternalCallMethod<TReturnType>(method, null, args);
        }

        #region SYNTACTIC SUGAR HELPER

        //Just a Generic argument signature for easier signature call

        public static Task<TReturnType> CallMethod<TReturnType, TGenericArg>(MethodBase method, params object[] args)
        {
            return InternalCallMethod<TReturnType>(
                method,
                new Type[] { typeof(TGenericArg) }
                , args);
        }

        public static Task<TReturnType> CallMethod<TReturnType, TGenericArg1, TGenericArg2>(MethodBase method, params object[] args)
        {
            return InternalCallMethod<TReturnType>(
                method,
                new Type[] { typeof(TGenericArg1), typeof(TGenericArg2) }
                , args);
        }

        public static Task<TReturnType> CallMethod<TReturnType, TGenericArg1, TGenericArg2, TGenericArg3>(MethodBase method, params object[] args)
        {
            return InternalCallMethod<TReturnType>(
                method,
                new Type[] { typeof(TGenericArg1), typeof(TGenericArg2), typeof(TGenericArg3) }
                , args);
        }

        public static Task<TReturnType> CallMethod<TReturnType, TGenericArg1, TGenericArg2, TGenericArg3, TGenericArg4>(MethodBase method, params object[] args)
        {
            return InternalCallMethod<TReturnType>(
                method,
                new Type[] { typeof(TGenericArg1), typeof(TGenericArg2), typeof(TGenericArg4) }
                , args);
        }

        #endregion

        public static Task<TReturnType> CallMethod<TReturnType>(MethodBase method, Type[] genericParameters, params object[] args)
        {
            return InternalCallMethod<TReturnType>(method, genericParameters, args);
        }

        private static Task<TReturnType> InternalCallMethod<TReturnType>(MethodBase methodBase, Type[] genericParameters, object[] args)
        {
            MethodInfo method = (MethodInfo)methodBase;

            if (genericParameters == null)
            {
                genericParameters = new Type[0];
            }

            if (args == null)
            {
                args = new object[0];
            }

            #region Invokation

            MethodProxy methodProxy = new MethodProxy();

            var iface = methodBase.DeclaringType.GetInterfaces().FirstOrDefault(p => p.GetCustomAttribute<ProxyInterfaceAttribute>() != null);
            if (iface == null)
            {
                throw new ArgumentException("Given class method does not contain any possible ProxyInterface. Be sure to add [ProxyInterface] attribute on top of your interface definition");
            }

            //As we are using a DispatchProxy, the targetMethod should be the interface MethodInfo (and not a class MethodInfo)
            int index = MethodProxyHelper.GetInterfaceMethodIndex(methodBase.DeclaringType, method, iface);

            if (index == -1)
                return default;

            methodProxy.MethodIndex = index;
            methodProxy.GenericTypes = genericParameters.Select(p => new TypeProxy(p)).ToArray();

            //On this case, the DeclaringType should be the Interface type
            methodProxy.InterfaceType = new TypeProxy(iface);

            //We will let the serializer do the thing for RunTime values
            methodProxy.Parameters = args;

            var jsonMethodProxy = BridgeSerializer.Serialize(methodProxy);

            switch (ContextHelper.GetExecutingContext())
            {
                case Models.ExecutingContext.Blazor:
                    BlazorToXamarinDispatcher.Send(jsonMethodProxy);
                    return Task.FromResult<TReturnType>(default);
                default:
                    throw new NotImplementedException();
            }

            #endregion

            //TODO: Proper call
            return null;
        }

        #endregion
    }
}
