using System;
using System.Threading.Tasks;
using BlazorMobile.Common.Helpers;
using BlazorMobile.Interop;
using BlazorMobile.Services;
using Windows.Web.Http.Filters;

namespace BlazorMobile.UWP.Interop
{
    public class WebViewService : IWebViewService
    {
        public void ClearCookies()
        {
            try
            {
                HttpBaseProtocolFilter baseFilter = new HttpBaseProtocolFilter();
                foreach (var cookie in baseFilter.CookieManager.GetCookies(new Uri(WebApplicationFactory.GetBaseURL())))
                {
                    baseFilter.CookieManager.DeleteCookie(cookie);
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteException(ex);
            }
        }

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