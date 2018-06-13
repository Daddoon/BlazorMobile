using Daddoon.Blazor.Xam.Common.Components;
using Daddoon.Blazor.Xam.Common.Helpers;
using Daddoon.Blazor.Xam.Common.Services;
using Microsoft.AspNetCore.Blazor.Browser.Interop;
using Microsoft.AspNetCore.Blazor.Browser.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

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
        internal static void Init(BrowserRenderer br, string domElementSelector, Action onFinish)
        {
            if (br == null)
            {
                throw new NullReferenceException();
            }

            br.AddComponent<BlazorXamarinExtensionScript>(domElementSelector);

            xamService = new BlazorXamarinDeviceService();

            InternalHelper.SetTimeout(async () => {
                //Safety for detect if RuntimePlatform is a Mobile app or a Browser
                //First detect Browser context eligible for Xamarin, then returning the proper Xamarin RuntimePlatform
                try
                {
                    if (RegisteredFunction.Invoke<bool>("BlazorXamarinRuntimeCheck"))
                    {
                        string resultRuntimePlatform = await xamService.GetRuntimePlatform();
                        RuntimePlatform = resultRuntimePlatform;
                    }
                    else
                    {
                        RuntimePlatform = Browser;
                    }
                }
                catch (Exception ex)
                {
                    RuntimePlatform = Browser;
                }

                onFinish?.Invoke();
            }, 100);
        }
    }
}
