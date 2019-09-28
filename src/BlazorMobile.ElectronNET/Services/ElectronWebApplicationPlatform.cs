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

        public string GetBaseURL()
        {
            if (string.IsNullOrEmpty(_cachedURI))
            {
                var mainWindow = (ElectronBlazorWebView)BlazorWebViewFactory.GetMainElectronBlazorWebViewInstance();
                var fetchURITask = Task.Run(async() => _cachedURI = await mainWindow.GetBrowserWindow().WebContents.GetUrl());
                Task.WaitAll(fetchURITask);
            }

            return _cachedURI;
        }
    }
}
