using BlazorMobile.Common.Helpers;
using BlazorMobile.Interop;
using Microsoft.AspNetCore.Components.Builder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMobile.Common.Services
{
    public static class BlazorService
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
        /// Initialize BlazorService from Blazor client-side app
        /// </summary>
        /// <param name="app"></param>
        /// <param name="domElementSelector"></param>
        /// <param name="onFinish"></param>
        public static void Init(IComponentsApplicationBuilder app, Action<bool> onFinish = null)
        {
            if (!_isInit)
            {
                Device.Init(app, onFinish);
                _isInit = true;
            }
        }

        /// <summary>
        /// Initialize BlazorWebviewService from a Blazor server-side app or client-side app.
        /// The app object is looking for a IComponentsApplicationBuilder interface or a ComponentEndpointConventionBuilder class.
        /// If neither type are found, this will result as an error
        /// </summary>
        /// <param name="app"></param>
        /// <param name="domElementSelector"></param>
        /// <param name="onFinish"></param>
        public static void Init(object app, Action<bool> onFinish = null)
        {
            if (app == null)
            {
                throw new NullReferenceException($"{nameof(IComponentsApplicationBuilder)} object is null");
            }

            if (app is IComponentsApplicationBuilder)
            {
                Init((IComponentsApplicationBuilder)app, onFinish);
                return;
            }
            else
            {
                //NOTE: As we don't want and can't integrate .NETCore assemblies namespace from this assembly,
                //we check the class type by name
                if (app.GetType().Name == ComponentEndpointConventionBuilder)
                {
                    if (!_isInit)
                    {
                        Device.InitServer(app, onFinish);
                        _isInit = true;
                        return;
                    }
                }
                else
                {
                    //If here, that mean the given app object is invalid
                    throw new InvalidCastException($"{nameof(app)} object does not have an expected type of {ComponentEndpointConventionBuilder} or {nameof(IComponentsApplicationBuilder)}");
                }
            }
        }
    }
}
