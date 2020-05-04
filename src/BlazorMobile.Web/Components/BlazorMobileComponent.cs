using BlazorMobile.Common.Helpers;
using BlazorMobile.Common.Services;
using BlazorMobile.Interop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.JSInterop;
using Microsoft.JSInterop.WebAssembly;
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
                //ElectronNET implementation does not rely on any JS Interop, nor Xamarin/Mono context EXCEPT if 'useWasm' is enabled
                if ((ContextHelper.IsBlazorMobile() || (ContextHelper.IsElectronNET() && ContextHelper.IsUsingWASM())) && await Runtime.InvokeAsync<bool>("BlazorXamarin.RuntimeCheck")
                    || ContextHelper.IsElectronNET())
                {
                    //This must be called before GetRuntimePlatform, as GetRuntimePlatform is considered as a BlazorAppLaunched event
                    //We are here workarounding a race condition when using ElectronNET. Using Electron here.
                    if (ContextHelper.IsElectronNET())
                    {
                        string currentURL = await Runtime.InvokeAsync<string>("BlazorXamarin.ElectronGetCurrentURL");
                        string UserDataPath = await Runtime.InvokeAsync<string>("BlazorXamarin.ElectronGetUserDataPath");

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

                        xamService.WriteLine($"HybridSupport.IsElectronActive returned: {isElectronActive.ToString()}");

                        if (ContextHelper.IsUsingWASM())
                        {
                            xamService.WriteLine("INFO: Using WASM engine on ElectronNET as 'useWasm' was set to true in 'UseBlazorMobileWithElectronNET' method");
                        }

                        await xamService.StartupMetadataForElectron(currentURL, UserDataPath);
                    }

                    BlazorDevice.RuntimePlatform = await xamService.GetRuntimePlatform();
                }
                else
                {
                    //If service return false we return the Browser default value
                    BlazorDevice.RuntimePlatform = BlazorDevice.Browser;
                }

                BlazorMobileSuccess = true;
            }
            catch (Exception ex)
            {
                xamService.WriteLine(ex.Message);

                BlazorDevice.RuntimePlatform = BlazorDevice.Browser;
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

        internal bool _isWebAssembly = true;

        internal bool IsWebAssembly()
        {
            return _isWebAssembly;
        }

        private async Task SetRemoteDebugEndpoint()
        {
            string uri = BlazorMobileService.GetContextBridgeURI();
            await Runtime.InvokeVoidAsync("BlazorXamarin.SetDebugRemoteEndpoint", uri);
        }

        private async Task<bool> JSRuntimeHasElectronFeature()
        {
            return await Runtime.InvokeAsync<bool>("BlazorXamarin.JSRuntimeHasElectronFeature");
        }

        private bool IsWASMJSRuntime(IJSRuntime runtime)
        {
            if (runtime is WebAssemblyJSRuntime)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SetCurrentRuntime(IJSRuntime runtime)
        {
            string blazorVersion;

            if (IsWASMJSRuntime(runtime))
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
        }

        internal void OnFinishEvent()
        {
            BlazorMobileService.SendOnBlazorMobileLoaded(this, BlazorMobileSuccess);
        }

        private async Task ValidateRequirements()
        {
            //If IsElectronNET is already to true, this mean we already know that we are using ElectronNET
            //So we don't have to do additional JS check before initializing code
            if (ContextHelper.IsElectronNET())
            {
                return;
            }

            bool isElectron = false;
            bool isWASM = false;

            //At this point, we can be in a kind of race condition on BlazorMobile if using ElectronNET + WASM mode
            //as this specific behavior will be detected only in the InitXamService method.
            //Also, as memory is not synchronized yet in this method between .NET Core and WASM, we don't know yet if
            //we are on ElectronNET or BlazorMobile.

            //HACK: We will try to discover ElectronNET specific features in JS in order to at least flag the IsElectronNET
            //method to true
            if (await JSRuntimeHasElectronFeature())
            {
                isElectron = true;
            }

            if (IsWASMJSRuntime(Runtime))
            {
                isWASM = true;
            }

            if (isElectron)
            {
                //Notify theses values only in Electron scenario
                Console.WriteLine("INFO: Electron detected from Javascript:" + isElectron);
                Console.WriteLine("INFO: Electron running with WASM:" + isWASM);
            }

            ContextHelper.SetElectronNETUsage(isElectron, isWASM);
        }

        private bool _FirstInit = true;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (_FirstInit)
            {
                _FirstInit = false;

                //Adding remote endpoint for debugging
                await SetRemoteDebugEndpoint();

                JSRuntime = Runtime;

                SetCurrentRuntime(JSRuntime);

                await ValidateRequirements();
                await InitXamService();

                OnFinishEvent();
            }
        }
    }
}
