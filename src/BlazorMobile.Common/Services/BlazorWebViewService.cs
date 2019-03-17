using BlazorMobile.Common.Helpers;
using Microsoft.AspNetCore.Components.Builder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMobile.Common.Services
{
    public static class BlazorWebViewService
    {
        private static bool _isInit = false;
        public static void Init(IComponentsApplicationBuilder app, string domElementSelector, Action<bool> onFinish = null)
        {
            if (!_isInit)
            {
                Device.Init(app, domElementSelector, onFinish);
                _isInit = true;
            }
        }
    }
}
