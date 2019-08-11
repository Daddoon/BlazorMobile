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
        private void ClearCookiesURIs(string uri)
        {
            try
            {
                HttpBaseProtocolFilter baseFilter = new HttpBaseProtocolFilter();
                foreach (var cookie in baseFilter.CookieManager.GetCookies(new Uri(uri)))
                {
                    baseFilter.CookieManager.DeleteCookie(cookie);
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteException(ex);
            }
        }

        public void ClearCookies()
        {
            try
            {
                ClearCookiesURIs(WebApplicationFactory.GetBaseURL());
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteException(ex);
            }

            if (WebApplicationFactory._cookiesURI != null)
            {
                foreach (string cookieURI in WebApplicationFactory._cookiesURI)
                {
                    try
                    {
                        ClearCookiesURIs(cookieURI);
                    }
                    catch (Exception ex)
                    {
                        ConsoleHelper.WriteException(ex);
                    }
                }
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