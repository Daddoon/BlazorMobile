using Daddoon.Blazor.Xam.Components;
using Daddoon.Blazor.Xam.Services;
using Xamarin.Forms;

namespace Daddoon.Blazor.Xam.Template
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();

            //Blazor WebView agnostic contoller logic
            IBlazorWebView webview = BlazorWebViewFactory.Create();

            //WebView rendering customization on page
            View webviewView = webview.GetView();
            webviewView.VerticalOptions = LayoutOptions.FillAndExpand;
            webviewView.HorizontalOptions = LayoutOptions.FillAndExpand;

            webview.LaunchBlazorApp();

            content.Children.Add(webviewView);
        }
	}
}
