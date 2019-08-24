using BlazorMobile.Common.Components;
using BlazorMobile.Common.Helpers;
using BlazorMobile.Common.Interop;
using BlazorMobile.Common.Serialization;
using BlazorMobile.Web.Services;
using Microsoft.JSInterop;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace BlazorMobile.Common.Services
{
    public static class BlazorToXamarinDispatcher
    {
        [JSInvokable]
        public static Task Send(MethodProxy methodProxy)
        {
            string csharpProxy = BridgeSerializer.Serialize(methodProxy);
            BlazorMobileComponent.GetJSRuntime().InvokeAsync<bool>("contextBridgeSend", csharpProxy);
            return Task.CompletedTask;
        }

        [JSInvokable]
        public static bool Receive(string methodProxyJson, bool socketSuccess)
        {
            if (string.IsNullOrEmpty(methodProxyJson))
                return false;

            try
            {
                MethodProxy resultProxy = BridgeSerializer.Deserialize<MethodProxy>(ref methodProxyJson);
                return BlazorCommonDispatcher.Receive(resultProxy, socketSuccess);
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteException(ex);
                return false;
            }
        }
    }
}
