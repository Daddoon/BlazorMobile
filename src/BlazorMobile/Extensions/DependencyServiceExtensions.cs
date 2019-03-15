using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

public static class DependencyServiceExtension
{
    /// <summary>
    /// Get Xamarin service from type instead of generic parameter
    /// </summary>
    /// <param name="type"></param>
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
