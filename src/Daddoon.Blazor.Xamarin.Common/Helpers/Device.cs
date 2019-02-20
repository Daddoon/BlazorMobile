using Daddoon.Blazor.Xam.Common.Components;
using Daddoon.Blazor.Xam.Common.Helpers;
using Daddoon.Blazor.Xam.Common.Services;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Daddoon.Blazor.Xam.Common
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
                Console.WriteLine("DEBUG 1");
                try
                {
                    if (await JSRuntime.Current.InvokeAsync<bool>("BlazorXamarinRuntimeCheck"))
                    {
                        Console.WriteLine("DEBUG 2");
                        string resultRuntimePlatform = await xamService.GetRuntimePlatform();
                        RuntimePlatform = resultRuntimePlatform;
                        Console.WriteLine("DEBUG 3");
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
