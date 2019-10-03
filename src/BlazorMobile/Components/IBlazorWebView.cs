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

        /// <summary>
        /// Post a message to the Blazor app. Any objects that listen to a specific message name by calling BlazorMobileService.MessageSubscribe will trigger their associated handlers.
        /// </summary>
        /// <param name="messageName"></param>
        /// <param name="args"></param>
        void PostMessage(string messageName, params object[] args);

        /// <summary>
        /// Call a static JSInvokable method from native side
        /// </summary>
        /// <param name="assembly">The assembly of the JSInvokable method to call</param>
        /// <param name="method">The JSInvokable method name</param>
        /// <param name="args">Parameters to forward to Blazor app. Check that your parameters are serializable/deserializable from both native and Blazor sides.</param>
        /// <returns></returns>
        void CallJSInvokableMethod(string assembly,string method, params object[] args);

        void GoBack();

        void GoForward();

        void Reload();

        #endregion

        View GetView();

        void LaunchBlazorApp();
    }
}
