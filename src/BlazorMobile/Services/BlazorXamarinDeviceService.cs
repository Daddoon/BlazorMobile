using BlazorMobile.Common.Helpers;
using BlazorMobile.Common.Interfaces;
using BlazorMobile.Common.Services;
using BlazorMobile.Components;
using BlazorMobile.Helper;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BlazorMobile.Services
{
    internal static class OperatingSystem
    {
        public static bool IsWindows() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static bool IsMacOS() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public static bool IsLinux() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }

    public class BlazorXamarinDeviceService : IBlazorXamarinDeviceService
    {
        private static bool _init = false;

        internal static void InitRuntimePlatform()
        {
            if (_init)
            {
                return;
            }

            string device = BlazorMobile.Common.BlazorDevice.Unknown;

            if (ContextHelper.IsBlazorMobile())
            {
                device = Device.RuntimePlatform;
            }
            else if (ContextHelper.IsElectronNET())
            {
                if (OperatingSystem.IsWindows())
                {
                    device = BlazorMobile.Common.BlazorDevice.Windows;
                }
                else if (OperatingSystem.IsLinux())
                {
                    device = BlazorMobile.Common.BlazorDevice.Linux;
                }
                else if (OperatingSystem.IsMacOS())
                {
                    device = BlazorMobile.Common.BlazorDevice.macOS;
                }
            }

            BlazorMobile.Common.BlazorDevice.RuntimePlatform = device;

            _init = true;
        }

        /// <summary>
        /// This method is specific as it is also used as the Blazor app initialization validator.
        /// This method is always called once after a Blazor app boot
        /// </summary>
        /// <returns></returns>
        public Task<string> GetRuntimePlatform()
        {
            //HACK: This is more a hack than the cleanest implementation we would expect.
            //We cannot determine what is the current executing WebView calling this method,
            //as it's a service called remotely than a WebView callback. We are assuming here that
            //all eligible WebViews with IWebViewIdentity interface are the Blazor app to check.
            //The right fix would be to be able to pass the WebView identity to the Blazor app, and then
            //forward it here when the BlazorMobileComponent want to initialize
            foreach (IWebViewIdentity identity in WebViewHelper.GetAllWebViewIdentities())
            {
                identity.BlazorAppLaunched = true;
            }

            return Task.FromResult(BlazorMobile.Common.BlazorDevice.RuntimePlatform);
        }

        public Task WriteLine(string message)
        {
            ConsoleHelper.WriteLine(message);
            return Task.CompletedTask;
        }
    }
}
