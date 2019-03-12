using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Daddoon.Blazor.Xam.Components
{
    public interface IBlazorWebView : IWebViewController
    {
        #region Extended WebView controllers

        WebViewSource Source { get; set; }

        event EventHandler<WebNavigatedEventArgs> Navigated;

        event EventHandler<WebNavigatingEventArgs> Navigating;

        void Eval(string script);

        Task<string> EvaluateJavaScriptAsync(string script);

        void GoBack();

        void GoForward();

        void Reload();

        #endregion

        View GetView();

        void LaunchBlazorApp();
    }
}
