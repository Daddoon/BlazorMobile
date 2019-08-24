using BlazorMobile.Common.Helpers;
using BlazorMobile.Common.Services;
using BlazorMobile.Interop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.JSInterop;
using Mono.WebAssembly.Interop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMobile.Common.Components
{
    public class BlazorMobileComponent : ComponentBase
    {
        [Inject]
        IJSRuntime Runtime { get; set; }

        private static IJSRuntime JSRuntime { get; set; }

        private readonly BlazorXamarinDeviceService xamService = new BlazorXamarinDeviceService();

        private bool BlazorMobileSuccess = false;

        private async Task InitXamService()
        {
            try
            {
                if ((ContextHelper.IsBlazorMobile() && await Runtime.InvokeAsync<bool>("BlazorXamarinRuntimeCheck"))
                    || (ContextHelper.IsElectronNET()))
                {
                    //ElectronNET implementation does not rely on any JS Interop, nor Xamarin/Mono context

                    string resultRuntimePlatform = await xamService.GetRuntimePlatform();
                    Device.RuntimePlatform = resultRuntimePlatform;
                }
                else
                {
                    //If service return false we return the Browser default value
                    Device.RuntimePlatform = Device.Browser;
                }

                BlazorMobileSuccess = true;
            }
            catch (Exception ex)
            {
                xamService.WriteLine(ex.Message);

                Device.RuntimePlatform = Device.Browser;
                BlazorMobileSuccess = false;
            }
        }

        /// <summary>
        /// Return the current IJSRuntime initialized with the BlazorMobile plugin
        /// </summary>
        /// <returns></returns>
        public static IJSRuntime GetJSRuntime()
        {
            return JSRuntime;
        }

        private bool _isInitialized = false;

        internal bool _isWebAssembly = true;

        internal bool IsWebAssembly()
        {
            return _isWebAssembly;
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            //This component must be rendered once!
            //If the component is disposed and then re-rendered, then something is wrong with the current project configuration
            if (_isInitialized)
                return;

            if (!ContextHelper.IsBlazorMobile())
            {
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
                builder.AddContent(1, ContextBridgeHelper.GetInjectableJavascript(false).Replace("%_contextBridgeURI%", BlazorMobileService.GetContextBridgeURI()));
                builder.CloseElement();
            }

            _isInitialized = true;
        }


        private void SetCurrentRuntime(IJSRuntime runtime)
        {
            string blazorVersion;

            if (runtime is MonoWebAssemblyJSRuntime mono)
            {
                //WASM version
                _isWebAssembly = true;
                blazorVersion = "WebAssembly (Client-side)";
            }
            else
            {
                //Server-side version
                _isWebAssembly = false;
                blazorVersion = ".NET Core (Server-side)";
            }

            xamService.WriteLine($"Detected Blazor implementation: {blazorVersion}");

            //We should notify that this configuration is incorrect as BlazorMobile would not be able to 'communicate' on assemblies outside the BlazorMobile.Web scope
            if (ContextHelper.IsElectronNET() && _isWebAssembly)
            {
                throw new PlatformNotSupportedException("ERROR: WebAssembly runtime detected while using ElectronNET with BlazorMobile. Use .NET Core (server-side) runtime by referencing blazor.server.js instead.");
            }
        }

        internal void OnFinishEvent()
        {
            Device._onFinishCallback?.Invoke(BlazorMobileSuccess);
        }

        private bool _FirstInit = true;
        protected override async Task OnAfterRenderAsync()
        {
            await base.OnAfterRenderAsync();

            if (_FirstInit)
            {
                _FirstInit = false;

                JSRuntime = Runtime;

                SetCurrentRuntime(JSRuntime);

                await InitXamService();

                OnFinishEvent();
            }
        }
    }
}
