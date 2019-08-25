using BlazorMobile.Common.Helpers;
using BlazorMobile.Common.Interfaces;
using BlazorMobile.Common.Services;
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
        public Task<string> GetRuntimePlatform()
        {
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

            //Sync Blazor and native side
            BlazorMobile.Common.BlazorDevice.RuntimePlatform = device;

            return Task.FromResult(device);
        }

        public Task WriteLine(string message)
        {
            ConsoleHelper.WriteLine(message);
            return Task.CompletedTask;
        }
    }
}
