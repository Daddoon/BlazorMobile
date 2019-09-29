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

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            Console.WriteLine("OnStart called");
            base.OnStart();
        }

        protected override void OnSleep()
        {
            Console.WriteLine("OnSleep called");
            base.OnSleep();
        }

        protected override void OnResume()
        {
            Console.WriteLine("OnResume called");
            base.OnResume();
        }
    }
}
