using BlazorMobile.Common.Helpers;
using Microsoft.AspNetCore.Components.Builder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMobile.Common.Services
{
    public static class BlazorWebViewService
    {
        private static bool _isInit = false;

        internal const string ComponentEndpointConventionBuilder = "ComponentEndpointConventionBuilder";

        internal const string ComponentEndpointConventionBuilderExtensionsGetType = "Microsoft.AspNetCore.Builder.ComponentEndpointConventionBuilderExtensions, Microsoft.AspNetCore.Components.Server";


        internal static string _serverSideClientIP = null;

        internal const string _contextBridgeRelativeURI = "/contextBridge";

        internal static string GetContextBridgeRelativeURI()
        {
            if (!ServerSideToClientRemoteDebuggingEnabled())
                return string.Empty;

            return "ws://" + _serverSideClientIP + _contextBridgeRelativeURI;
        }

        internal static bool ServerSideToClientRemoteDebuggingEnabled()
        {
            return _serverSideClientIP != null;
        }

        /// <summary>
        /// Enable server-side web application to connect to a remote allowed native client (iOS, Android) running on a BlazorMobile Xamarin.Forms application
        /// in order to simulate the Blazor client-side device communication, through web call
        /// </summary>
        /// <param name="ip"></param>
        public static void EnableServerSideToClientRemoteDebugging(string ip)
        {
            _serverSideClientIP = ip;
        }

        /// <summary>
        /// Initialize BlazorWebViewService from Blazor client-side app
        /// </summary>
        /// <param name="app"></param>
        /// <param name="domElementSelector"></param>
        /// <param name="onFinish"></param>
        public static void Init(IComponentsApplicationBuilder app, string domElementSelector, Action<bool> onFinish = null)
        {
            if (!_isInit)
            {
                Device.Init(app, domElementSelector, onFinish);
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
        public static void Init(object app, string domElementSelector, Action<bool> onFinish = null)
        {
            if (app == null)
            {
                onFinish?.Invoke(false);
                throw new NullReferenceException($"{nameof(IComponentsApplicationBuilder)} object is null");
            }

            if (app is IComponentsApplicationBuilder)
            {
                Init((IComponentsApplicationBuilder)app, domElementSelector, onFinish);
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
                        Device.InitServer(app, domElementSelector, onFinish);
                        _isInit = true;
                        return;
                    }
                }
                else
                {
                    //If here, that mean the given app object is invalid
                    onFinish?.Invoke(false);
                    throw new InvalidCastException($"{nameof(app)} object does not have an expected type of {ComponentEndpointConventionBuilder} or {nameof(IComponentsApplicationBuilder)}");
                }
            }
        }
    }
}
