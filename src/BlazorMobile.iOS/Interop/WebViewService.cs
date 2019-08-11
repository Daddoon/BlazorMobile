using BlazorMobile.Interop;

namespace BlazorMobile.iOS.Interop
{
    public class WebViewService : IWebViewService
    {
        public void ClearCookies()
        {
            //No need to do anything. iOS implementation use is set in private mode with BlazorMobile
        }

        public void ClearWebViewData()
        {
            //No need to do anything. iOS implementation use is set in private mode with BlazorMobile
        }
    }
}