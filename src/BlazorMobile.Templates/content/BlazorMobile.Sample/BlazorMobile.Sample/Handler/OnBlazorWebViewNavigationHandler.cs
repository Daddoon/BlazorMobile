using BlazorMobile.Common;
using BlazorMobile.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xamarin.Forms;

namespace BlazorMobile.Sample.Handler
{
    public static class OnBlazorWebViewNavigationHandler
    {
        public static void OnBlazorWebViewNavigating(object sender, WebNavigatingEventArgs e)
        {
            var applicationBaseURL = WebApplicationFactory.GetBaseURL() + "/";

            if (e.Url.Equals(applicationBaseURL, StringComparison.OrdinalIgnoreCase))
            {
                //This is our application base URI. We should do nothing and continue navigatin to the app
                e.Cancel = false;
            }
            else if (e.Url.StartsWith(WebApplicationFactory.GetBaseURL(), StringComparison.OrdinalIgnoreCase))
            {
                //Here, our application is loading an URI
                //You may add a custom logic, like opening a new view, changing the URI parameters then opening a view...

                e.Cancel = true;

                switch (BlazorDevice.RuntimePlatform)
                {
                    default:
                        Device.OpenUri(new Uri(WebUtility.UrlDecode(e.Url)));
                        break;
                }
            }
            else
            {
                //If here this is not our application loading an URI
                //You may add a custom logic, like opening a new view, changing the URI parameters then opening a view...

                e.Cancel = true;

                switch (BlazorDevice.RuntimePlatform)
                {
                    default:
                        Device.OpenUri(new Uri(WebUtility.UrlDecode(e.Url)));
                        break;
                }
            }
        }
    }
}
