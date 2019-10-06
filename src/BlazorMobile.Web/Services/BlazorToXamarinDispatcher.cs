using BlazorMobile.Common.Components;
using BlazorMobile.Common.Helpers;
using BlazorMobile.Common.Interop;
using BlazorMobile.Common.Serialization;
using BlazorMobile.Web.Services;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
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
        internal static Task Send(MethodProxy methodProxy)
        {
            string csharpProxy = BridgeSerializer.Serialize(methodProxy);
            BlazorMobileComponent.GetJSRuntime().InvokeAsync<bool>("contextBridgeSend", csharpProxy);
            return Task.CompletedTask;
        }

        //NOTE: Javascript should be able to call this method even with obsolete
        //This is what we want. We only want to prevent a direct use from a .NET assembly
        //at build time. Btw, this does not prevent anyone to trick the system with a
        //C# -> Javascript -> C# call. Actually, this is only for readability
        [JSInvokable]
        [Obsolete("Internal use only", true)]
        public static bool Receive(string methodProxyJson, bool socketSuccess)
        {
            //Unlikely to happen. It would mean that we don't even know who was the caller
            if (string.IsNullOrEmpty(methodProxyJson))
                return false;

            if (methodProxyJson.IndexOf(nameof(MessageProxy.MessageProxyToken)) > -1)
            {
                MessageProxyReceiver(methodProxyJson, socketSuccess);
            }
            else
            {
                MethodProxyReceiver(methodProxyJson, socketSuccess);
            }

            return true;
        }

        private static bool TryGetDefaultFallbackMethodProxy(string methodProxyJson, Exception forwardedException, out MethodProxy defaultProxy)
        {
            defaultProxy = null;

            try
            {
                //We will only fetch required data for defaulting
                JObject methodProxyObject = JObject.Parse(methodProxyJson);
                if (!methodProxyObject.TryGetValue(nameof(MethodProxy.TaskIdentity), out JToken TaskIdentityToken))
                {
                    return false;
                }

                Guid TaskIdentity = Guid.Parse(TaskIdentityToken.Value<string>());
                defaultProxy = new MethodProxy()
                {
                    TaskIdentity = TaskIdentity,
                    TaskSuccess = false,
                    ExceptionDescriptor = new ExceptionDescriptor($"An error occured during data deserialization on Blazor side while receiving data from native side. Check that your used types are serializable and present on both Blazor & native side. See InnerException for more details.")
                    {
                        InnerException = forwardedException
                    }
                };

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool MethodProxyReceiver(string methodProxyJson, bool socketSuccess)
        {
            MethodProxy resultProxy = null;

            try
            {
                resultProxy = BridgeSerializer.Deserialize<MethodProxy>(ref methodProxyJson);
            }
            catch (Exception ex)
            {
                //Actually if we have an exception here that can mean that the user data has seem class not deserializable
                //between native and Blazor side. As we are probably in a waiting Task scenario, we must try to forward
                //the exception to the waiting task by fetching it's TaskId.

                //It will be easier to debug then
                if (TryGetDefaultFallbackMethodProxy(methodProxyJson, ex, out MethodProxy defaultProxy))
                {
                    return BlazorCommonDispatcher.Receive(defaultProxy, socketSuccess);
                }

                //Otherwise returning false. The task may be blocked in a waiting state forever
                //TODO: We should implement some kind of CancellationToken in the future on interop method signatures
                //forcing the business code to continue
                return false;
            }

            return BlazorCommonDispatcher.Receive(resultProxy, socketSuccess);
        }

        private static bool MessageProxyReceiver(string methodProxyJson, bool socketSuccess)
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
