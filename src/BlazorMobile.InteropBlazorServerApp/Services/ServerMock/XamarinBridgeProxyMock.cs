using BlazorMobile.Common.Components;
using BlazorMobile.Common.Services;
using BlazorMobile.InteropApp.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMobile.InteropBlazorApp.Services.ServerMock
{
    public class XamarinBridgeProxyMock : IXamarinBridge
    {
        public Task<List<string>> DisplayAlert(string title, string msg, string cancel)
        {
            BlazorXamarinExtensionScript.GetJSRuntime().InvokeAsync<Task>("alert", msg);
            return Task.FromResult(new List<string>() { "Dummy content" });
        }
    }
}
