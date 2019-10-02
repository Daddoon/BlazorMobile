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
            //NOTE: As there is some issue on each platform when trying to stop server gracefully at OnSleep event
            //We don't manage anything here. Instead if the current server throw, it will restart, and if we detect
            //that server is not started, we will try to restart it at OnResume event instead
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
