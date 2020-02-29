using BlazorMobile.Common.Components;
using BlazorMobile.Common.Helpers;
using BlazorMobile.Common.Interop;
using BlazorMobile.Common.Serialization;
using BlazorMobile.Web.Helpers.WebSocketWrapper;
using BlazorMobile.Web.Services;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorMobile.Common.Services
{
    public static class BlazorToXamarinDispatcher
    {
        #region WebSocket I/O

        private static WebSocketWrapper _client = null;

        private static void OnConnectedHandler(WebSocketWrapper connection)
        {

        }

        private static void OnDisconnectHandler(WebSocketWrapper connection)
        {
            _client = null;
        }

        private static void OnMessageHandler(string message, WebSocketWrapper connection)
        {
            Receive(message, true);
        }

        private static async Task<WebSocketWrapper> GetConnection()
        {
            try
            {
                if (_client == null)
                {
                    //Instanciating a new client
                    _client = WebSocketWrapper.Create(BlazorMobileService.GetContextBridgeURI());

                    _client.OnConnect(OnConnectedHandler);
                    _client.OnDisconnect(OnDisconnectHandler);
                    _client.OnMessage(OnMessageHandler);

                    await _client.Connect();
                }
                else if (_client.State == WebSocketState.Connecting)
                {
                    //Give additional time to connect if connecting from first call
                    await Task.Delay(200);
                }

                return _client;
            }
            catch (Exception ex)
            {
                _client = null;

                ConsoleHelper.WriteException(ex);
                throw;
            }
        }

        #endregion WebSocket I/O

        internal static async Task Send(MethodProxy methodProxy)
        {
            string csharpProxy = BridgeSerializer.Serialize(methodProxy);

            try
            {
                var client = await GetConnection();
                await client.SendMessage(csharpProxy);
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteException(ex);

                Task.Run(() =>
                {
                    Receive(csharpProxy, false);
                });
            }

            return;
        }

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
