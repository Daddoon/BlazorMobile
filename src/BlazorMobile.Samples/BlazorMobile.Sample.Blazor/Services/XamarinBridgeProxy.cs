using BlazorMobile.Common.Services;
using BlazorMobile.Sample.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMobile.Sample.Blazor.Services
{
    public class XamarinBridgeProxy : IXamarinBridge
    {
        public Task<List<string>> DisplayAlert(string title, string msg, string cancel)
        {
            return MethodDispatcher.CallMethodAsync<List<string>>(MethodBase.GetCurrentMethod(), title, msg, cancel);
        }
    }
}
