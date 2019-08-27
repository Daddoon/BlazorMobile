using BlazorMobile.Common;
using BlazorMobile.Components;
using BlazorMobile.Services;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Xamarin.Forms;

namespace BlazorMobile.InteropApp
{
	public partial class App : BlazorApplication
	{
        public App()
        {
            InitializeComponent();

#if DEBUG
            WebApplicationFactory.EnableDebugFeatures();
#endif

            WebApplicationFactory.SetHttpPort(8888);

            //Regiser Blazor app resolver
            //CUSTOMIZE HERE YOUR OWN CODE LOGIC IF NEEDED !!
            WebApplicationFactory.RegisterAppStreamResolver(() =>
            {
                //Get current class Assembly object
                var assembly = typeof(App).Assembly;

                //Name of our current Blazor package in this project, stored as a Embedded Resource
                string BlazorPackageFolder = "BlazorMobile.InteropBlazorApp.zip";

                string appPackage = $"{assembly.GetName().Name}.Package.{BlazorPackageFolder}";

                return assembly.GetManifestResourceStream(appPackage);
            });

            MainPage = new MainPage();
        }
    }
}
