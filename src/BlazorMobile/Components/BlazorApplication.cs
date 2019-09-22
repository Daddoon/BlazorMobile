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

            WebApplicationFactory.StopWebServer();
        }

        protected override void OnResume()
        {
            if (ContextHelper.IsElectronNET())
            {
                return;
            }

            Device.BeginInvokeOnMainThread(async () =>
            {
                //Giving additionnal time on Android, as there is a bug in Release mode that does not free port properly
                if (Device.RuntimePlatform == Device.Android)
                {
                    await Task.Delay(5000);
                }

                WebApplicationFactory.ResetBlazorViewIfHttpPortChanged();
                WebApplicationFactory.StartWebServer();
            });
        }
    }
}
