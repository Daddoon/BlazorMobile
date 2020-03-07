using System;
using BlazorMobile.Common.Helpers;
using BlazorMobile.Interop;
using BlazorMobile.Services;

namespace BlazorMobile.ElectronNET.Interop
{
    public class WebViewService : IWebViewService
    {
        public void ClearCookies()
        {
            //No-op on ElectronNET at the moment
            ConsoleHelper.WriteLine($"{nameof(ClearCookies)} method: no-op on ElectronNET");
        }

        public void ClearWebViewData()
        {
            //No-op on ElectronNET at the moment
            ConsoleHelper.WriteLine($"{nameof(ClearWebViewData)} method: no-op on ElectronNET");
        }
    }
}