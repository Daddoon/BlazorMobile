using BlazorMobile.Common.Components;
using BlazorMobile.Common.Helpers;
using BlazorMobile.Common.Interop;
using BlazorMobile.Common.Serialization;
using BlazorMobile.Web.Services;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
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
                    BlazorMobileService.SendMessageToJSInvokableMethod(resultProxy);
                }
                else
                {
                    if (resultProxy.InteropParameters == null)
                        resultProxy.InteropParameters = new object[0];

                    //This is more about atomicity as we don't want to do extensive test
                    //We just only want to prevent to pass an empty / null value arguments that would maybe not be identifiable on its type after serialization / deserialization
                    Type ArgsType = resultProxy.InteropArgsType.ResolvedType();

                    //Using delegate Messaging API
                    BlazorMobileService.SendMessageToSubscribers(resultProxy.InteropMethod, ArgsType, resultProxy.InteropParameters);
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
