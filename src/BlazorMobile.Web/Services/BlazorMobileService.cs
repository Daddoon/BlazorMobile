using BlazorMobile.Common.Helpers;
using BlazorMobile.Interop;
using Microsoft.AspNetCore.Components.Builder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMobile.Common.Services
{
    public static class BlazorMobileService
    {
        private static bool _isInit = false;

        internal const string ComponentEndpointConventionBuilder = "ComponentEndpointConventionBuilder";

        internal const string ComponentEndpointConventionBuilderExtensionsGetType = "Microsoft.AspNetCore.Builder.ComponentEndpointConventionBuilderExtensions, Microsoft.AspNetCore.Components.Server";

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

        /// <summary>
        /// Set the delegate method to be executed when BlazorMobile finished loading
        /// </summary>
        /// <param name="onFinish"></param>
        public static void Init(Action<bool> onFinish = null)
        {
            if (!_isInit)
            {
                Device.Init(onFinish);
                _isInit = true;
            }
        }

        /// <summary>
        /// Call of this method will notify to BlazorMobile that the runtime is actually running under ElectronNET (server-side)
        /// NOTE: Usage of blazor.server.js in your starting page is mandatory.
        /// If using BlazorMobile.Build on your Blazor base project, you should register 'server_index.html' at this app start.
        /// </summary>
        public static void UseElectronNET()
        {
            ContextHelper.SetElectronNETUsage(true);
        }
    }
}
