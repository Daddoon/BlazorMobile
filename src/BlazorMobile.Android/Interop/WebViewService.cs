using System;
using System.Threading.Tasks;
using BlazorMobile.Interop;

namespace BlazorMobile.Droid.Interop
{
    public class WebViewService : IWebViewService
    {
        public void ClearCookies()
        {
            //No need to do anything. Android implementation use GeckoView configured with 
            //UsePrivaMode to true. Each session will not cache anything then
        }

        public void ClearWebViewData()
        {
            //No need to do anything. Android implementation use GeckoView configured with 
            //UsePrivaMode to true. Each session will not cache anything then
        }
    }
}