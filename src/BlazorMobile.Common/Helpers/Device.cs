using BlazorMobile.Common.Components;
using BlazorMobile.Common.Helpers;
using BlazorMobile.Common.Services;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace BlazorMobile.Common
{

    //Mimic Xamarin.Forms Device static class
    public static class Device
    {
        public const string iOS = "iOS";
        public const string Android = "Android";
        public const string WinPhone = "WinPhone";
        public const string UWP = "UWP";
        public const string Browser = "Browser";
        public const string Unknown = "Unknown";
        public static string RuntimePlatform { get; private set; } = Unknown;

        private static BlazorXamarinDeviceService xamService = null;
        internal static void Init(IComponentsApplicationBuilder app, string domElementSelector, Action<bool> onFinish)
        {
            bool success = false;

            if (app == null)
            {
                onFinish?.Invoke(success);
                throw new NullReferenceException($"{nameof(IComponentsApplicationBuilder)} object is null");
            }

            app.AddComponent<BlazorXamarinExtensionScript>(domElementSelector);

            InternalHelper.SetTimeout(async () =>
            {
                xamService = new BlazorXamarinDeviceService();
                try
                {
                    if (await JSRuntime.Current.InvokeAsync<bool>("BlazorXamarinRuntimeCheck"))
                    {
                        string resultRuntimePlatform = await xamService.GetRuntimePlatform();
                        RuntimePlatform = resultRuntimePlatform;
                    }
                    else
                    {
                        RuntimePlatform = Browser;
                    }

                    success = true;
                }
                catch (Exception ex)
                {
                    RuntimePlatform = Browser;
                    throw;
                }
                onFinish?.Invoke(success);
            }, 10);
        }
    }
}
