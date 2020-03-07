using BlazorMobile.InteropApp.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorMobile.InteropBlazorApp.Helpers
{
    public static class ServicesHelper
    {
        public static void ConfigureCommonServices(IServiceCollection services)
        {
            services.AddBlazorMobileNativeServices<Program>();
        }

        private static IXamarinBridge _xamarinBridge = null;

        public static void SetXamarinBridge(IXamarinBridge service)
        {
            if (_xamarinBridge == null && service != null)
            {
                _xamarinBridge = service;
            }
        }

        public static IXamarinBridge GetXamarinBridge()
        {
            return _xamarinBridge;
        }
    }
}
