using Daddoon.Blazor.Xam.Services;
using System;
using Xamarin.Forms;

namespace Daddoon.Blazor.Xam.InteropApp
{
	public partial class App : Application
	{
        public App()
        {
            InitializeComponent();

            WebApplicationFactory.SetHttpPort(8888);

            //Regiser Blazor app resolver
            //CUSTOMIZE HERE YOUR OWN CODE LOGIC IF NEEDED !!
            WebApplicationFactory.RegisterAppStreamResolver(() =>
            {
                //Get current class Assembly object
                var assembly = typeof(App).Assembly;

                //Name of our current Blazor package in this project, stored as a Embedded Resource
                string BlazorPackageFolder = "Mobile.package.app.zip";

                string appPackage = $"{assembly.GetName().Name}.{BlazorPackageFolder}";

                return assembly.GetManifestResourceStream(appPackage);
            });

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            WebApplicationFactory.StartWebServer();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            WebApplicationFactory.StopWebServer();
        }

        protected override void OnResume()
        {
            WebApplicationFactory.StartWebServer();
        }
    }
}
