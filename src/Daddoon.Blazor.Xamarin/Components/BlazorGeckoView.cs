using Daddoon.Blazor.Xam.Common;
using Daddoon.Blazor.Xam.Services;
using Xam.Droid.GeckoView.Forms;
using Xamarin.Forms;

namespace Daddoon.Blazor.Xam.Components
{
    public class BlazorGeckoView : GeckoViewForms, IBlazorWebView
    {
        public BlazorGeckoView()
        {
            Navigated += BlazorWebView_Navigated;
        }

        public void LaunchBlazorApp()
        {
            switch (Xamarin.Forms.Device.RuntimePlatform)
            {
                default:
                    Source = new UrlWebViewSource()
                    {
                        Url = WebApplicationFactory.GetBaseURL()
                    };
                    blazorAppLaunched = true;
                    break;
            }
        }

        private bool blazorAppLaunched = false;
        private void BlazorWebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            OnNavigated();
        }

        public void OnNavigated()
        {
            if (blazorAppLaunched)
            {
                //TODO: Actually as GeckoView is unable to inject javascript, we must use a websocket implementation
            }
        }

        public View GetView()
        {
            return this;
        }
    }
}
