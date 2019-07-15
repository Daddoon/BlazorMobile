using BlazorMobile.Common.Services;
using BlazorMobile.Interop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorMobile.Common.Components
{
    public class BlazorXamarinExtensionScript : ComponentBase
    {
        [Inject]
        IUriHelper _injectedUriHelper { get; set; }

        private static IUriHelper UriHelper { get; set; }

        [Inject]
        IJSRuntime Runtime { get; set; }

        private static IJSRuntime JSRuntime { get; set; }

        protected override void OnInit()
        {
            JSRuntime = Runtime;
            UriHelper = _injectedUriHelper;
        }

        /// <summary>
        /// Return the current IJSRuntime initialized with the BlazorMobile plugin
        /// </summary>
        /// <returns></returns>
        public static IJSRuntime GetJSRuntime()
        {
            return JSRuntime;
        }

        /// <summary>
        /// Return the current IUriHelper initialized with the BlazorMobile plugin
        /// </summary>
        /// <returns></returns>
        public static IUriHelper GetUriHelper()
        {
            return UriHelper;
        }

        private static bool _isInitialized = false;

        internal static bool IsWebAssembly = true;

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            //This component must be rendered once!
            if (_isInitialized)
                return;

            builder.OpenElement(0, "script");
            builder.AddContent(1, @"
    window.BlazorXamarinRuntimeCheck = function () {
        if (window.contextBridge == null || window.contextBridge == undefined)
        {
            return false;
        }
    
        return true;
    };
");
            builder.CloseElement();

            if (IsWebAssembly == false && BlazorWebViewService.ServerSideToClientRemoteDebuggingEnabled())
            {
                //Add server side remote to client remote debugging
                builder.OpenElement(0, "script");
                builder.AddContent(1, ContextBridgeHelper.GetInjectableJavascript(false).Replace("%_contextBridgeURI%", BlazorWebViewService.GetContextBridgeRelativeURI()));
                builder.CloseElement();
            }

            _isInitialized = true;
        }
    }
}
