using BlazorMobile.Common.Helpers;
using BlazorMobile.Common.Services;
using BlazorMobile.Interop;
using BlazorMobile.Services;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BlazorMobile.Components
{
    public abstract class BlazorApplication : Application
    {
        public BlazorApplication()
        {
            if (ContextHelper.IsElectronNET() && !ContextHelper.IsUsingWASM())
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

            if (ContextHelper.IsElectronNET() && ContextHelper.IsUsingWASM())
            {
                //We must call OnStart by ourselves here are this is not a real Xamarin.Forms driver
                //Maybe this can be delegated somewhere else.
                var realType = this.GetType();
                var _onStartMethod = realType.GetMethod(nameof(OnStart), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                _onStartMethod.Invoke(this, null);
            }
        }

        protected override void OnStart()
        {
            if (ContextHelper.IsElectronNET() && !ContextHelper.IsUsingWASM())
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
            if (ContextHelper.IsElectronNET() && !ContextHelper.IsUsingWASM())
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
