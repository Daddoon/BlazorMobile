using Microsoft.AspNetCore.Blazor.Browser.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daddoon.Blazor.Xam.Common.Services
{
    public static class BlazorWebViewService
    {
        private static bool _isInit = false;
        public static void Init(BrowserRenderer br, string domElementSelector, Action onFinish = null)
        {
            if (!_isInit)
            {
                Device.Init(br, domElementSelector, onFinish);
                _isInit = true;
            }
        }
    }
}
