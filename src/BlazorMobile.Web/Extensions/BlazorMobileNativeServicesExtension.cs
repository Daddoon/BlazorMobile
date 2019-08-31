using BlazorMobile.Proxy.Interop.Abstract;
using BlazorMobile.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BlazorMobileNativeServiceExtensions
    {
        public static void AddBlazorMobileNativeServices<TClientApp>(this IServiceCollection services)
        {
            var foundTypes = Assembly.GetAssembly(typeof(TClientApp)).GetTypes();

            var proxyClassList = foundTypes.Where(p => p.IsPublic
            && !p.IsAbstract
            && p.IsSubclassOf(typeof(BlazorMobileProxyClass)));

            foreach (var proxyClass in proxyClassList)
            {
                //Only Proxy class with the correct interface attribute should be registered.
                //Regarding the BlazorMobileProxyClass implementation, this should be all btw

                var interfaceType = proxyClass.GetInterfaces().FirstOrDefault(p => p.GetCustomAttribute<BlazorMobile.Common.Attributes.ProxyInterfaceAttribute>() != null);

                //No eligible interface found
                if (interfaceType == null)
                    continue;

                //We can safely register the class and the interface
                services.AddSingleton(interfaceType, proxyClass);
            }
        }
    }
}
