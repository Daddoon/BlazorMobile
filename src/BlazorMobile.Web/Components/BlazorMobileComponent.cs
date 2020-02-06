using BlazorMobile.Common.Helpers;
using BlazorMobile.Common.Services;
using BlazorMobile.Interop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
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

        private readonly BlazorXamarinDeviceService xamService = new BlazorXamarinDeviceService();

        private bool BlazorMobileSuccess = false;

        private async Task InitXamService()
        {
            try
            {
                //This must be called before GetRuntimePlatform, as GetRuntimePlatform is considered as a BlazorAppLaunched event
                //We are here workarounding a race condition when using ElectronNET. Using Electron here.
                if (ContextHelper.IsElectronNET())
                {
                    string currentURL = await Runtime.InvokeAsync<string>("BlazorMobileElectron.GetCurrentURL");
                    string UserDataPath = await Runtime.InvokeAsync<string>("BlazorMobileElectron.GetUserDataPath");

                    //Before calling StartupMetadataForElectron and call the initialization event from GetRuntimePlatform
                    //we want to ensure that the 'HybridSupport.IsElectronActive' value is ready remotly, to ensure
                    //to finish to install ElectronNET Hooks

                    bool isElectronActive = false;
                    int waitingCounter = 0;

                    while (waitingCounter < 10)
                    {
                        //We will do 10 attempts. If unable to honor the result, we will continue and ignore this
                        if (isElectronActive = await xamService.IsElectronActive())
                        {
                            break;
                        }

                        waitingCounter++;
                        await Task.Delay(200);
                    }

                    ConsoleHelper.WriteLine($"HybridSupport.IsElectronActive returned: {isElectronActive.ToString()}");

                    await xamService.StartupMetadataForElectron(currentURL, UserDataPath);
                }

                BlazorDevice.RuntimePlatform = await xamService.GetRuntimePlatform();

                BlazorMobileSuccess = true;
            }
            catch (Exception ex)
            {
                xamService.WriteLine(ex.Message);

                BlazorDevice.RuntimePlatform = BlazorDevice.Browser;
                BlazorMobileSuccess = false;
            }
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

            if (ContextHelper.IsElectronNET())
            {
                //Add electronNET helpers
                builder.OpenElement(0, "script");
                builder.AddContent(1, ContextBridgeHelper.GetElectronNETJavascript());
                builder.CloseElement();
            }

            _isInitialized = true;
        }

        private void DetectCurrentRuntime(IJSRuntime runtime)
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
            BlazorDevice._onFinishCallback?.Invoke(BlazorMobileSuccess);
        }

        private bool _FirstInit = true;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (_FirstInit)
            {
                _FirstInit = false;

                DetectCurrentRuntime(Runtime);

                await InitXamService();

                OnFinishEvent();
            }
        }
    }
}
