using BlazorMobile.Common.Metadata;
using BlazorMobile.Components;
using BlazorMobile.Helper;
using BlazorMobile.Interop;
using BlazorMobile.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Constants;
using Unosquare.Labs.EmbedIO.Modules;
using Xamarin.Forms;

namespace BlazorMobile.Controller
{
    public class BlazorController : WebApiController
    {
        public BlazorController(IHttpContext context) : base(context)
        {
        }

        private bool ShouldExcludeFromNavigatingEvent(string uri, out bool shouldCancel)
        {
            shouldCancel = false;

            if (!string.IsNullOrEmpty(uri))
            {
                if (uri.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
                {
                    shouldCancel = true;
                    return true;
                }
            }

            return false;
        }

        private async Task<bool> ValidateRequestAndroid(string webextensionId)
        {
            string cancel = "false";

            if (int.TryParse(webextensionId, out int runtimeId))
            {
                var webview = WebViewHelper.GetWebViewByRuntimeIdentity(runtimeId);
                if (webview != null)
                {
                    string uri = this.Request.QueryString.Get("uri");
                    string referrer = this.Request.QueryString.Get("referrer");

                    if (ShouldExcludeFromNavigatingEvent(uri, out bool shouldCancel))
                    {
                        if (shouldCancel)
                        {
                            cancel = "true";
                        }
                    }
                    else
                    {
                        var args = new WebNavigatingEventArgs(
                        WebNavigationEvent.NewPage,
                        new UrlWebViewSource() { Url = referrer },
                        uri);

                        webview.SendNavigating(args);

                        if (args.Cancel)
                        {
                            cancel = "true";
                        }
                    }
                }
            }

            IWebResponse response = new EmbedIOWebResponse(this.Request, this.Response);
            response.SetEncoding("UTF-8");
            response.AddResponseHeader("Cache-Control", "no-cache");
            response.SetStatutCode(200);
            response.SetReasonPhrase("OK");
            response.SetMimeType("text/plain");
            await response.SetDataAsync(new MemoryStream(Encoding.UTF8.GetBytes(cancel)));
            return true;
        }

        private async Task<bool> ValidateRequestElectronWASM()
        {
            string uri = this.Request.QueryString.Get("uri");
            string referrer = this.Request.QueryString.Get("referrer");

            IWebResponse response = new EmbedIOWebResponse(this.Request, this.Response);
            response.SetEncoding("UTF-8");
            response.AddResponseHeader("Cache-Control", "no-cache");
            response.SetReasonPhrase("OK");
            response.SetMimeType("text/plain");

            if (!string.IsNullOrEmpty(uri) && !string.IsNullOrEmpty(referrer))
            {
                var args = new WebNavigatingEventArgs(
                WebNavigationEvent.NewPage,
                new UrlWebViewSource() { Url = referrer },
                uri);

                //This is not entirely true as we would only compare the current BlazorWebview
                //but it must be only one executed btw
                foreach (var webIdentity in WebViewHelper.GetAllWebViewIdentities())
                {
                    var webview = webIdentity as IBlazorWebView;
                    webview.SendNavigating(args);

                    if (args.Cancel)
                    {
                        break;
                    }
                }

                if (args.Cancel)
                    response.SetStatutCode(401);
                else
                    response.SetStatutCode(200);
            }
            else
            {
                response.SetStatutCode(200);
            }

            await response.SetDataAsync(new MemoryStream(Encoding.UTF8.GetBytes("OK")));
            return true;
        }

        [WebApiHandler(HttpVerbs.Get, @"^(?!\/contextBridge.*$).*")]
        public async Task<bool> BlazorAppRessources()
        {
            try
            {
                //This is a request validation mecanism for the missing behavior of Navigating event when
                //navigating through an iframe. We must ensure that this behavior is concistent on each
                //platforms.
                string webextensionId = this.Request.Headers.Get("BlazorMobile-Validator");
                if (!string.IsNullOrEmpty(webextensionId))
                {
                    return await ValidateRequestAndroid(webextensionId);
                }
                else if (this.Request.Url.AbsolutePath.Equals(MetadataConsts.ElectronBlazorMobileRequestValidationPath, StringComparison.OrdinalIgnoreCase))
                {
                    return await ValidateRequestElectronWASM();
                }
                else
                {
                    //Standard behavior, serving Blazor app files and content
                    IWebResponse response = new EmbedIOWebResponse(this.Request, this.Response);
                    await WebApplicationFactory.ManageRequest(response);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return this.JsonExceptionResponse(ex);
            }
        }

        // You can override the default headers and add custom headers to each API Response.
        public override void SetDefaultHeaders() => this.NoCache();
    }
}
