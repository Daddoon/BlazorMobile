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
            string device = BlazorMobile.Common.Device.Unknown;

            if (ContextHelper.IsBlazorMobile())
            {
                device = Device.RuntimePlatform;
            }
            else if (ContextHelper.IsElectronNET())
            {
                if (OperatingSystem.IsWindows())
                {
                    device = BlazorMobile.Common.Device.Windows;
                }
                else if (OperatingSystem.IsLinux())
                {
                    device = BlazorMobile.Common.Device.Linux;
                }
                else if (OperatingSystem.IsMacOS())
                {
                    device = BlazorMobile.Common.Device.macOS;
                }
            }

            return Task.FromResult(device);
        }

        public Task WriteLine(string message)
        {
            ConsoleHelper.WriteLine(message);
            return Task.CompletedTask;
        }
    }
}
