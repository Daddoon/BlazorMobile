using BlazorMobile.Common;
using BlazorMobile.Services;
using System;
using Xam.Droid.GeckoView.Forms;
using Xamarin.Forms;

namespace BlazorMobile.Components
{
    public class BlazorGeckoView : GeckoViewForms, IBlazorWebView
    {
        internal BlazorGeckoView()
        {
            Navigated += BlazorWebView_Navigated;
            WebApplicationFactory.BlazorAppNeedReload += ReloadBlazorAppEvent;
        }

        private void ReloadBlazorAppEvent(object sender, EventArgs e)
        {
            LaunchBlazorApp();
        }

        public void LaunchBlazorApp()
        {
            BlazorWebView.InternalLaunchBlazorApp(this);
        }

        private void BlazorWebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
        }


        public View GetView()
        {
            return this;
        }
    }
}
