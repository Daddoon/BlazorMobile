using Daddoon.Blazor.Xam.Components;
using Daddoon.Blazor.Xam.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Daddoon.Blazor.Xam.InteropApp
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
            InitializeComponent();

            var url = new UrlWebViewSource
            {
                Url = WebApplicationFactory.GetBaseURL()
            };

            BlazorWebView webview = new BlazorWebView()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 1000,
                WidthRequest = 1000,
                Source = url
            };
            content.Children.Add(webview);
        }
	}
}
