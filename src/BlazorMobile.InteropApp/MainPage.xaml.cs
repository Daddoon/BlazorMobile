using BlazorMobile.Components;
using BlazorMobile.InteropApp.Handler;
using BlazorMobile.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BlazorMobile.InteropApp
{
	public partial class MainPage : ContentPage
	{
        IBlazorWebView webview;

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
