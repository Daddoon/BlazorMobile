using System;
using System.Threading.Tasks;
using BlazorMobile.Common.Helpers;
using BlazorMobile.Interop;

namespace BlazorMobile.UWP.Interop
{
    public class WebViewService : IWebViewService
    {
        public void ClearWebViewData()
        {
            try
            {
                Windows.UI.Xaml.Controls.WebView.ClearTemporaryWebDataAsync().AsTask().Wait();
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteException(ex);
            }
        }
    }
}