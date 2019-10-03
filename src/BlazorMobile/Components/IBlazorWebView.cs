using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace BlazorMobile.Components
{
    public interface IBlazorWebView : IWebViewController
    {
        #region Extended WebView controllers

        WebViewSource Source { get; set; }

        event EventHandler<WebNavigatedEventArgs> Navigated;

        event EventHandler<WebNavigatingEventArgs> Navigating;

        void Eval(string script);

        Task<string> EvaluateJavaScriptAsync(string script);

        Task<string> PostMessage(string assembly,string method, params object[] args);

        void GoBack();

        void GoForward();

        void Reload();

        #endregion

        View GetView();

        void LaunchBlazorApp();
    }
}
