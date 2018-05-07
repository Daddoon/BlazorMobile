using Daddoon.Blazor.Xam.Template.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Daddoon.Blazor.Xam.Template
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

            //var url = new HtmlWebViewSource()
            //{
            //    BaseUrl = WebApplicationFactory.GetBaseURL(),
            //    //BaseUrl = "/",
            //    Html = System.Text.Encoding.UTF8.GetString(WebApplicationFactory.GetResource("index.html"))
            //};

            WebView webview = new WebView()
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
