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

        private static Dictionary<string, MethodInfo> _assemblyCache = new Dictionary<string, MethodInfo>();

        private static MethodInfo GetJSInvokableMethod(MessageProxy proxy)
        {
            MethodInfo method = null;

            try
            {
                string key = proxy.InteropAssembly + proxy.InteropMethod;

                if (_assemblyCache.ContainsKey(key))
                {
                    method = _assemblyCache[key];
                }
                else
                {
                    var methods = AppDomain.CurrentDomain.GetAssemblies()
                        .Where(assembly => assembly.GetName().Name == proxy.InteropAssembly)
                        .Select(x => x.GetTypes())
                        .SelectMany(x => x)
                        .Where(c => c.GetMethod(proxy.InteropMethod, BindingFlags.Public | BindingFlags.Static) != null)
                        .Select(c => c.GetMethod(proxy.InteropMethod, BindingFlags.Public | BindingFlags.Static));

                    method = methods.FirstOrDefault(p => p.GetCustomAttribute<JSInvokableAttribute>() != null);

                    if (method == null)
                    {
                        ConsoleHelper.WriteError("CallJSInvokableMethod: Target method was not found");
                        return null;
                    }

                    _assemblyCache.Add(key, method);
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteException(ex);
            }
            
            return method;
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
                    var invokableMethod = GetJSInvokableMethod(resultProxy);
                    invokableMethod?.Invoke(null, new object[] { resultProxy.InteropParameters });
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
