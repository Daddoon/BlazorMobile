using BlazorMobile.Common.Helpers;
using BlazorMobile.Common.Services;
using BlazorMobile.Interop;
using BlazorMobile.Services;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BlazorMobile.Components
{
    public abstract class BlazorApplication : Application
    {
        public BlazorApplication(bool isElectron)
        {

        }

        public BlazorApplication()
        {
            if (ContextHelper.IsElectronNET())
            {
                return;
            }

            WebApplicationFactory.SetHttpPort();

            try
            {
                var webViewService = DependencyService.Get<IWebViewService>();
                webViewService.ClearWebViewData();
            }
            catch (System.Exception ex)
            {
                ConsoleHelper.WriteException(ex);
            }
        }

        protected override void OnStart()
        {
            if (ContextHelper.IsElectronNET())
            {
                return;
            }

            WebApplicationFactory.StartWebServer();
        }

        protected override void OnSleep()
        {
            if (ContextHelper.IsElectronNET())
            {
                return;
            }

            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    //There is a bug on Android in release mode that seem to prevent to stop or release port OnSleep
                    //Avoiding to Stop the server OnSleep as the event seem to not be fully propagated
                    break;
                default:
                    WebApplicationFactory.StopWebServer();
                    break;
            }
        }

        protected override void OnResume()
        {
            if (ContextHelper.IsElectronNET())
            {
                return;
            }

            bool needReload = WebApplicationFactory.ResetBlazorViewIfHttpPortChanged();

            WebApplicationFactory.StartWebServer();

            if (!needReload)
            {
                //As the previous event can fire a reload too, we just check that a reload is not already pending,
                //preventing to call the WebView reload twice
                WebApplicationFactory.NotifyEnsureBlazorAppLaunchedOrReload();
            }
        }
    }
}
