using Daddoon.Blazor.Xam.Common.Interop;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Daddoon.Blazor.Xam.Common.Services
{
    public static class DependencyService
    {
        static DependencyService()
        {
            if (ProxyInstances == null)
                ProxyInstances = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Assuming Dictionary Interface Type, ProxyClass instance (even if fake)
        /// </summary>
        private static Dictionary<Type, object> ProxyInstances { get; set; }

        public static T Get<T>() where T : class
        {
            Type type = typeof(T);

            if (!type.IsInterface)
                return null;

            //If already called once, return the cached instance
            if (ProxyInstances.ContainsKey(type))
                return (T)ProxyInstances[type];

            return DispatchProxy.Create<T, ClassProxy>();
        }
    }
}
