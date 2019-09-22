using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace BlazorMobile.Extensions
{
    public static class DependencyServiceExtension
    {
        /// <summary>
        /// This DependencyService extension allow invoking the Get method with a Type given in parameter
        /// instead of using a known generic parameter, as there is no method overload allowing to search
        /// an instance of a service from a Type object.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="DependencyFetchTarget"></param>
        /// <returns></returns>
        public static object Get(Type type, DependencyFetchTarget DependencyFetchTarget = DependencyFetchTarget.GlobalInstance)
        {
            MethodInfo baseMethod = null;
            MethodInfo getMethod = null;

            getMethod = SymbolExtensions.GetMethodInfo(() => DependencyService.Get<object>(DependencyFetchTarget));
            baseMethod = getMethod.GetGenericMethodDefinition();

            MethodInfo genericMethod = baseMethod.MakeGenericMethod(new Type[] { type });
            return genericMethod.Invoke(null, new object[] { DependencyFetchTarget });
        }
    }
}
