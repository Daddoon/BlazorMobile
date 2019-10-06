using BlazorMobile.Components;
using BlazorMobile.ElectronNET.Components;
using BlazorMobile.Interop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMobile.ElectronNET.Services
{
    public class ElectronWebApplicationPlatform : IWebApplicationPlatform
    {
        private string _cachedURI = null;

        internal void SetCachedBaseURL(string baseURL)
        {
            _cachedURI = baseURL;
        }

        public string GetBaseURL()
        {
            if (string.IsNullOrEmpty(_cachedURI))
            {
                throw new InvalidOperationException("Unable to determine the application BaseURL. On ElectronNET, check that WebApplicationFactory.GetBaseURL method is called after a call to your IBlazorWebView.LaunchBlazorApp method");
            }
            return _cachedURI;
        }
    }
}
