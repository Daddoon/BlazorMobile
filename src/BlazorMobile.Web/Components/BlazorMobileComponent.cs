using BlazorMobile.Common.Services;
using BlazorMobile.Interop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMobile.Common.Components
{
    public class BlazorMobileComponent : ComponentBase
    {
        [Inject]
        IUriHelper _injectedUriHelper { get; set; }

        private static IUriHelper UriHelper { get; set; }

        [Inject]
        IJSRuntime Runtime { get; set; }

        private static IJSRuntime JSRuntime { get; set; }

        private static BlazorXamarinDeviceService xamService = null;

        private async Task InitXamService()
        {
            xamService = new BlazorXamarinDeviceService();
            bool success;

            try
            {
                if (Runtime != null && await Runtime.InvokeAsync<bool>("BlazorXamarinRuntimeCheck"))
                {
                    string resultRuntimePlatform = await xamService.GetRuntimePlatform();
                    Device.RuntimePlatform = resultRuntimePlatform;
                }
                else
                {
                    //If service return false or if JSRuntime is not yet ready (on server side scenario), we return the Browser default value
                    Device.RuntimePlatform = Device.Browser;
                }

                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                Device.RuntimePlatform = Device.Browser;
                success = false;
            }
            Device._onFinishCallback?.Invoke(success);
        }

        protected override void OnInitialized()
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

        private bool _isInitialized = false;

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

            //Add server side remote to client remote debugging
            builder.OpenElement(0, "script");
            builder.AddContent(1, ContextBridgeHelper.GetInjectableJavascript(false).Replace("%_contextBridgeURI%", BlazorService.GetContextBridgeURI()));
            builder.CloseElement();

            _isInitialized = true;
        }


        private bool _isFirstRender = true;
        protected override async Task OnAfterRenderAsync()
        {
            if (_isFirstRender)
            {
                await InitXamService();
                _isFirstRender = false;
            }

            await base.OnAfterRenderAsync();

            return;
        }
    }
}
