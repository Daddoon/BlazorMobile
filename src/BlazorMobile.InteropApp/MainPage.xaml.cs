using BlazorMobile.Components;
using BlazorMobile.InteropApp.Handler;
using BlazorMobile.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BlazorMobile.InteropApp
{
	public partial class MainPage : ContentPage
	{
        MemoryStream contentStream;

        private async Task PackageTest()
        {
            //Test for store download

            var _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };

            try
            {
                using (var httpResponse = await _httpClient.GetAsync("https://raw.githubusercontent.com/Daddoon/BlazorMobile/master/README.md"))
                {
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        contentStream = new MemoryStream();
                        var result = await httpResponse.Content.ReadAsStreamAsync();
                        result.CopyTo(contentStream);
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            WebApplicationFactory.AddPackage("testpackage", contentStream);

                            //This Stream was initialized outside the "using" scope as we wanted it through 2 differents thread
                            //So disposing it here now as we don't have to use it anymore
                            contentStream.Dispose();
                        });
                    }
                    else
                    {
                        //Url is Invalid
                        return;
                    }
                }
            }
            catch (Exception)
            {
                //Handle Exception
                return;
            }
        }

        public static IBlazorWebView webview = null;

		public MainPage()
		{
            InitializeComponent();

            //Blazor WebView agnostic contoller logic
            webview = BlazorWebViewFactory.Create();

            //WebView rendering customization on page
            View webviewView = webview.GetView();
            webviewView.VerticalOptions = LayoutOptions.FillAndExpand;
            webviewView.HorizontalOptions = LayoutOptions.FillAndExpand;

            //Manage your native application behavior when an external resource is requested in your Blazor web application
            //Customize your app behavior in BlazorMobile.Sample.Handler.OnBlazorWebViewNavigationHandler.cs file or create your own!
            webview.Navigating += OnBlazorWebViewNavigationHandler.OnBlazorWebViewNavigating;

            webview.LaunchBlazorApp();

            content.Children.Add(webviewView);

            //Task.Run(PackageTest);
        }

        ~MainPage()
        {
            if (webview != null)
            {
                webview.Navigating -= OnBlazorWebViewNavigationHandler.OnBlazorWebViewNavigating;
            }
        }
    }
}
