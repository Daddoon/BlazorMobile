using BlazorMobile.Common.Helpers;
using BlazorMobile.Interop;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BlazorMobile.ElectronNET")]
namespace BlazorMobile.Common.Services
{
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

        internal static bool IsInitCalled()
        {
            return _isInit;
        }

        /// <summary>
        /// Set the delegate method to be executed when BlazorMobile finished loading
        /// </summary>
        /// <param name="onFinish"></param>
        public static void Init(Action<bool> onFinish = null)
        {
            if (!_isInit)
            {
                BlazorDevice.Init(onFinish);
                _isInit = true;
            }
        }

        #region Messaging Center

        private static Dictionary<string, List<Action<object[]>>> _delegateHandler = new Dictionary<string, List<Action<object[]>>>();

        /// <summary>
        /// Subscribe to a specific message name sent from native side with PostMessage, and forward the event to the specified delegate if received
        /// </summary>
        /// <param name="messageName">The message name to subscribe to</param>
        /// <param name="handler">The delegate action that must be executed at message reception</param>
        public static void MessageSubscribe(string messageName, Action<object[]> handler)
        {
            if (string.IsNullOrEmpty(messageName))
            {
                throw new NullReferenceException($"{nameof(messageName)} cannot be null");
            }

            if (!_delegateHandler.ContainsKey(messageName))
            {
                _delegateHandler.Add(messageName, new List<Action<object[]>>());
            }

            //Should be ok, see https://docs.microsoft.com/en-us/dotnet/api/system.delegate.op_equality?view=netstandard-2.0 for more info
            //as Action inherit from Delegate, this must be true too: https://docs.microsoft.com/en-us/dotnet/api/system.action?view=netstandard-2.0
            if (!_delegateHandler[messageName].Contains(handler))
            {
                _delegateHandler[messageName].Add(handler);
            }
        }

        /// <summary>
        /// Unsubscribe to a specific message name sent from native side with PostMessage
        /// </summary>
        /// <param name="messageName">The message name to unsubscribe to</param>
        /// <param name="handler">The delegate action that must be unsubscribed</param>
        public static void MessageUnsubscribe(string messageName, Action<object[]> handler)
        {
            if (string.IsNullOrEmpty(messageName))
            {
                throw new NullReferenceException($"{nameof(messageName)} cannot be null");
            }

            if (!_delegateHandler.ContainsKey(messageName))
            {
                _delegateHandler.Add(messageName, new List<Action<object[]>>());
            }

            //Should be ok, see https://docs.microsoft.com/en-us/dotnet/api/system.delegate.op_equality?view=netstandard-2.0 for more info
            //as Action inherit from Delegate, this must be true too: https://docs.microsoft.com/en-us/dotnet/api/system.action?view=netstandard-2.0
            if (_delegateHandler[messageName].Contains(handler))
            {
                _delegateHandler[messageName].Remove(handler);
            }
        }

        internal static void SendMessageToSubscribers(string messageName, object[] payload)
        {
            if (string.IsNullOrEmpty(messageName))
            {
                throw new NullReferenceException($"{nameof(messageName)} cannot be null");
            }

            if (!_delegateHandler.ContainsKey(messageName))
            {
                _delegateHandler.Add(messageName, new List<Action<object[]>>());
            }

            foreach (Action<object[]> action in _delegateHandler[messageName])
            {
                //We are in a try catch as we want to continue to propagate if the user event crash for any reason (invalid code or disposed object...
                try
                {
                    action(payload);
                }
                catch (Exception ex)
                {
                    ConsoleHelper.WriteException(ex);
                }
            }
        }

        #endregion Messaging Center
    }
}
