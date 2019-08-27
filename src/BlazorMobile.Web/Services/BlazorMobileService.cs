using BlazorMobile.Interop;
using System;
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
    }
}
