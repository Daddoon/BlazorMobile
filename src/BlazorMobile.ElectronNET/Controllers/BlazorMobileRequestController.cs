using BlazorMobile.Components;
using BlazorMobile.Helper;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Xamarin.Forms;

namespace BlazorMobile.ElectronNET.Controllers
{
    public class BlazorMobileRequestController : Microsoft.AspNetCore.Mvc.Controller
    {
        public IActionResult Index(string uri, string referrer)
        {
            bool cancel = false;

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
                        cancel = true;
                        break;
                    }
                }
            }

            if (cancel)
            {
                return StatusCode(401);
            }

            return StatusCode(200);
        }
    }
}
