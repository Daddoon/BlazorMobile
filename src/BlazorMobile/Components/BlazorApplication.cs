using BlazorMobile.Services;
using Xamarin.Forms;

namespace BlazorMobile.Components
{
    public abstract class BlazorApplication : Application
    {
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
