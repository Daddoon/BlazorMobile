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

            WebApplicationFactory.StopWebServer();
        }

        protected override void OnResume()
        {
            if (ContextHelper.IsElectronNET())
            {
                return;
            }

            WebApplicationFactory.ResetBlazorViewIfHttpPortChanged();
            WebApplicationFactory.StartWebServer();
        }
    }
}
