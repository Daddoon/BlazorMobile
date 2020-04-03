using BlazorMobile.Common.Components;
using BlazorMobile.Common.Helpers;
using BlazorMobile.Common.Interop;
using BlazorMobile.Interop;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BlazorMobile.ElectronNET")]
namespace BlazorMobile.Common.Services
{
    public class BlazorMobileOnFinishEventArgs : EventArgs
    {
        public bool Success { get; set; }
    }

    public static class BlazorMobileService
    {
        private static bool _isInit = false;

        internal static int _serverSideClientPort = -1;

        internal static string _serverSideClientIP = null;

        internal static string GetContextBridgeURI()
        {
            if (!IsEnableClientToDeviceRemoteDebuggingEnabled())
                return string.Empty;

            return "ws://" + _serverSideClientIP + ":" + _serverSideClientPort + ContextBridgeHelper._contextBridgeRelativeURI;
        }

        internal static bool IsEnableClientToDeviceRemoteDebuggingEnabled()
        {
            return _serverSideClientIP != null && _serverSideClientPort != -1;
        }

        /// <summary>
        /// Add a display:none attribute to the selected element with the corresponding id attribute.
        /// This is mainly used for hidding the extra placeholder used during BlazorMobile loading, after it has finished
        /// </summary>
        /// <param name="elementId"></param>
        public static void HideElementById(string elementId)
        {
            var runtime = BlazorMobileComponent.GetJSRuntime();
            if (runtime == null)
            {
                Console.WriteLine("Cannot call HideElementById, JSRuntime interop is not yet ready");
                return;
            }

            try
            {
                BlazorMobileComponent.GetJSRuntime().InvokeVoidAsync("BlazorXamarin.HideElementById", elementId);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Enable server-side web application to connect to a remote allowed native client (iOS, Android) running on a BlazorMobile Xamarin.Forms application
        /// in order to simulate the Blazor client-side device communication, through web call
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public static void EnableClientToDeviceRemoteDebugging(string ip, int port)
        {
            _serverSideClientIP = ip;
            _serverSideClientPort = port;
        }

        public delegate void BlazorMobileEventHandler(object source, BlazorMobileOnFinishEventArgs args);

        /// <summary>
        /// Subscribe to this event to be notified when BlazorMobile has finished loading
        /// </summary>
        public static event BlazorMobileEventHandler OnBlazorMobileLoaded;

        public static bool IsBlazorMobileLoaded { get; set; }

        internal static void SendOnBlazorMobileLoaded(object source, bool success)
        {
            IsBlazorMobileLoaded = true;

            OnBlazorMobileLoaded?.Invoke(source,
                new BlazorMobileOnFinishEventArgs()
                {
                    Success = success
                });
        }

        #region Messaging Center

        private static Dictionary<string, Dictionary<Type, List<Delegate>>> _delegateHandler = new Dictionary<string, Dictionary<Type, List<Delegate>>>();

        private static void AddDelegateEntryTypeIfNotExist(string messageName, Type TArgsType)
        {
            if (string.IsNullOrEmpty(messageName))
            {
                throw new NullReferenceException($"{nameof(messageName)} cannot be null");
            }

            if (!_delegateHandler.ContainsKey(messageName))
            {
                _delegateHandler.Add(messageName, new Dictionary<Type, List<Delegate>>());
            }

            if (!_delegateHandler[messageName].ContainsKey(TArgsType))
            {
                _delegateHandler[messageName].Add(TArgsType, new List<Delegate>());
            }
        }

        private static void AddDelegateEntry<TArgs>(string messageName, Action<TArgs> handler)
        {
            Type argsType = typeof(TArgs);
            AddDelegateEntryTypeIfNotExist(messageName, argsType);

            //Should be ok, see https://docs.microsoft.com/en-us/dotnet/api/system.delegate.op_equality?view=netstandard-2.0 for more info
            //as Action inherit from Delegate, this must be true too: https://docs.microsoft.com/en-us/dotnet/api/system.action?view=netstandard-2.0
            if (!_delegateHandler[messageName][argsType].Contains(handler))
            {
                _delegateHandler[messageName][argsType].Add(handler);
            }
        }

        private static void RemoveDelegateEntry<TArgs>(string messageName, Action<TArgs> handler)
        {
            Type argsType = typeof(TArgs);
            AddDelegateEntryTypeIfNotExist(messageName, argsType);

            //Should be ok, see https://docs.microsoft.com/en-us/dotnet/api/system.delegate.op_equality?view=netstandard-2.0 for more info
            //as Action inherit from Delegate, this must be true too: https://docs.microsoft.com/en-us/dotnet/api/system.action?view=netstandard-2.0
            if (_delegateHandler[messageName][argsType].Contains(handler))
            {
                _delegateHandler[messageName][argsType].Remove(handler);
            }
        }

        private static List<Delegate> GetEligibleDelegates(string messageName, Type TArgsType)
        {
            AddDelegateEntryTypeIfNotExist(messageName, TArgsType);

            return _delegateHandler[messageName][TArgsType];
        }

        /// <summary>
        /// Subscribe to a specific message name sent from native side with PostMessage, and forward the event to the specified delegate if received
        /// </summary>
        /// <param name="messageName">The message name to subscribe to</param>
        /// <param name="handler">The delegate action that must be executed at message reception</param>
        public static void MessageSubscribe<TArgs>(string messageName, Action<TArgs> handler)
        {
            AddDelegateEntry(messageName, handler);
        }

        /// <summary>
        /// Unsubscribe to a specific message name sent from native side with PostMessage
        /// </summary>
        /// <param name="messageName">The message name to unsubscribe to</param>
        /// <param name="handler">The delegate action that must be unsubscribed</param>
        public static void MessageUnsubscribe<TArgs>(string messageName, Action<TArgs> handler)
        {
            RemoveDelegateEntry(messageName, handler);
        }

        /// <summary>
        /// Allow to post a message to any delegate action registered through MessageSubscribe.
        /// This method behavior is similar to the IBlazorWebView.PostMessage method on the native side,
        /// except that you send message from within your Blazor app instead sending it from native side.
        /// </summary>
        /// <typeparam name="TArgs">The paramter expected type</typeparam>
        /// <param name="messageName">The message name to target</param>
        /// <param name="value">The value to send in the message</param>
        public static void PostMessage<TArgs>(string messageName, TArgs value)
        {
            SendMessageToSubscribers(messageName, typeof(TArgs), new object[] { value });
        }

        internal static void SendMessageToSubscribers(string messageName, Type ArgsType, object[] payload)
        {
            AddDelegateEntryTypeIfNotExist(messageName, ArgsType);

            foreach (Delegate action in GetEligibleDelegates(messageName, ArgsType))
            {
                //We are in a try catch as we want to continue to propagate if the user event crash for any reason (invalid code or disposed object...
                try
                {
                    action.DynamicInvoke(payload);
                }
                catch (Exception ex)
                {
                    ConsoleHelper.WriteException(ex);
                }
            }
        }

        #endregion Messaging Center

        #region CallJSInvokableMethod

        private static Dictionary<string, MethodInfo> _assemblyCache = new Dictionary<string, MethodInfo>();

        private static MethodInfo GetJSInvokableMethod(string assemblyName, string methodName)
        {
            MethodInfo method = null;

            try
            {
                string key = assemblyName + methodName;

                if (_assemblyCache.ContainsKey(key))
                {
                    method = _assemblyCache[key];
                }
                else
                {
                    var methods = AppDomain.CurrentDomain.GetAssemblies()
                        .Where(assembly => assembly.GetName().Name == assemblyName)
                        .Select(x => x.GetTypes())
                        .SelectMany(x => x)
                        .Where(c => c.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static) != null)
                        .Select(c => c.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static));

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

        internal static void SendMessageToJSInvokableMethod(MessageProxy proxy)
        {
            SendMessageToJSInvokableMethod(proxy.InteropAssembly, proxy.InteropMethod, proxy.InteropParameters);
        }

        internal static void SendMessageToJSInvokableMethod(string assembly, string method, object[] args)
        {
            var invokableMethod = GetJSInvokableMethod(assembly, method);

            invokableMethod?.Invoke(null, args.Length <= 0 ? null : args);
        }

        #endregion CallJSInvokableMethod
    }
}
