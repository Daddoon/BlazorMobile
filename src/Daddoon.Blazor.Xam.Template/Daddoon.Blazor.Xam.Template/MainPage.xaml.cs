using Daddoon.Blazor.Xam.Template.Interfaces;
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

            //Note: How to manage local files: https://docs.microsoft.com/fr-fr/xamarin/xamarin-forms/user-interface/webview?tabs=vswin

            var source = new HtmlWebViewSource();
            source.BaseUrl = DependencyService.Get<IBaseUrl>().Get();
            source.Html = @"<html><body>
                          <h1>Xamarin.Forms</h1>
                          <p>Welcome to WebView.</p>
                          </body></html>";

            WebView webview = new WebView()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 1000,
                WidthRequest = 1000,
                Source = source
            };
            content.Children.Add(webview);
		}
	}
}
