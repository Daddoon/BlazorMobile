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

        [JSInvokable]
        public static bool ReceiveFromXamarin(string methodProxyJson, bool socketSuccess)
        {
            if (string.IsNullOrEmpty(methodProxyJson))
                return false;

            try
            {
                MessageProxy resultProxy = BridgeSerializer.Deserialize<MessageProxy>(ref methodProxyJson);

                //Using JSInvokable API
                if (resultProxy.IsJSInvokable)
                {
                    //A little hacky, but actually as this is a second rountrip to javascript,
                    //but ensure that the method called is JSInvokable, by the Blazor API.
                    BlazorMobileComponent.GetJSRuntime().InvokeAsync<bool>("contextBridgeSendClient", resultProxy.InteropAssembly, resultProxy.InteropMethod, resultProxy.InteropParameters);
                }
                else
                {
                    //Using delegate Messaging API
                    BlazorMobileService.SendMessageToSubscribers(resultProxy.InteropMethod, resultProxy.InteropParameters);
                }

                return true;

            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteException(ex);
                return false;
            }
        }
    }
}
