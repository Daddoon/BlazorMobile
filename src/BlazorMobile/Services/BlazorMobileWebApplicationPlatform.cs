using BlazorMobile.Interop;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorMobile.Services
{
    internal class BlazorMobileWebApplicationPlatform : IWebApplicationPlatform
    {
        /// <summary>
        /// Get the Base URL from the regular BlazorMobile driver
        /// This method is marked as static internal in order to call it from ElectronNET + WASM implementation, without having to rewrite the logic twice as it will use the same webserver as regular BlazorMobile app in this mode.
        /// </summary>
        /// <returns></returns>
        internal static string GetBaseURLInternal()
        {
            return $"http://{WebApplicationFactory.GetLocalWebServerIP()}:{WebApplicationFactory.GetHttpPort()}";
        }

        public string GetBaseURL()
        {
            return GetBaseURLInternal();
        }
    }
}
