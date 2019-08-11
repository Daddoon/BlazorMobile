using BlazorMobile.Common;
using BlazorMobile.Services;
using Xam.Droid.GeckoView.Forms;
using Xamarin.Forms;

namespace BlazorMobile.Components
{
    public class BlazorGeckoView : GeckoViewForms, IBlazorWebView
    {
        public BlazorGeckoView()
        {
            Navigated += BlazorWebView_Navigated;
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
