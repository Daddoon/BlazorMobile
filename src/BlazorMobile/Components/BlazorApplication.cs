using BlazorMobile.Common.Helpers;
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
            WebApplicationFactory.StartWebServer();
        }

        protected override void OnSleep()
        {
            WebApplicationFactory.StopWebServer();
        }

        protected override void OnResume()
        {
            WebApplicationFactory.StartWebServer();
        }
    }
}
