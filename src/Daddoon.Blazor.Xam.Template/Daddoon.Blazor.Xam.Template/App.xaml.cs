using Daddoon.Blazor.Xam.Template.Services;
using Waher.Networking.HTTP;
using Xamarin.Forms;

namespace Daddoon.Blazor.Xam.Template
{
    public partial class App : Application
	{
        public App ()
		{
			InitializeComponent();
			MainPage = new MainPage();
		}

		protected override void OnStart ()
		{
            // Handle when your app starts
            WebApplicationFactory.StartWebServer();
		}

		protected override void OnSleep ()
		{
            // Handle when your app sleeps
            WebApplicationFactory.StopWebServer();
        }

		protected override void OnResume ()
		{
            WebApplicationFactory.StartWebServer();
        }
	}
}
