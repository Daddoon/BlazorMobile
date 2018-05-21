using Daddoon.Blazor.Xam.Common.Interop;
using Daddoon.Blazor.Xam.Common.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace Daddoon.Blazor.Xam.Interop
{
    public static class ContextBridge
    {
        public static void Send()
        {

        }

        public static void Receive(string methodProxyJson)
        {
            MethodProxy methodProxy = BridgeSerializer.Deserialize<MethodProxy>(methodProxyJson);

            Type iface = methodProxy.InterfaceType.ResolvedType();
            object concreteService = DependencyServiceExtension.Get(iface);

            MethodInfo baseMethod = MethodProxyHelper.GetClassMethodInfo(concreteService.GetType(), iface, methodProxy.MethodIndex);

            baseMethod.Invoke(concreteService, methodProxy.Parameters);
        }
    }
}
