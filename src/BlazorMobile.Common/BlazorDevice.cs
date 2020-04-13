using BlazorMobile.Common.Services;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BlazorMobile")]
[assembly: InternalsVisibleTo("BlazorMobile.Web")]
namespace BlazorMobile.Common
{
    public static class BlazorDevice
    {
        public const string iOS = "iOS";
        public const string Android = "Android";
        public const string UWP = "UWP";

        //Added for ElectronNET
        public const string macOS = "macOS";
        public const string Linux = "Linux";
        public const string Windows = "Windows";

        //Custom ones
        public const string Browser = "Browser";
        public const string Unknown = "Unknown";
        public static string RuntimePlatform { get; internal set; } = Unknown;

        /// <summary>
        /// Return true if the current executing context is on ElectronNET
        /// </summary>
        /// <returns></returns>
        public static bool IsElectronNET()
        {
            //Actually the following statement does not entirely behave exactly the same in ElectronNET (Server-side) and
            //WASM (client-side). On ElectronNET, BlazorMobile, BlazorMobile.Web and BlazorMobile.Common are loaded from the same
            //memory process, and so updating the ElectronNET flag in BlazorMobile.Common from BlazorMobile.Web both update the value
            //everywhere. In WASM version, as we are inter-processing between the browser and the Xamarin host, even if BlazorMobile.Common
            //is loaded in both context, they don't share the same memory.

            //But as IsElectronNET is off by default, the WASM version will return the correct value by default.
            //As the only way to set IsElectronNET to true is to call it from the ElectronNET startup project.

            //In the worst case if the user do something wrong, BlazorMobileComponent throw an exception in the Browser if the combination
            //of IsElectronNET == true and WebAssembly == true is detected.

            return ContextHelper.IsElectronNET();
        }

        /// <summary>
        /// Return true if the current application is Using WASM engine
        /// NOTE: This is always true on mobile platform, but not on Electron depending your configuration
        /// </summary>
        /// <returns></returns>
        public static bool IsUsingWASM()
        {
            return ContextHelper.IsUsingWASM();
        }
    }
}
