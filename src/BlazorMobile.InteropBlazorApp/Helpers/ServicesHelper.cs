using BlazorMobile.InteropApp.Common.Interfaces;
using BlazorMobile.InteropBlazorApp.Services;
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
            services.AddSingleton<IXamarinBridge, XamarinBridgeProxy>();
        }
    }
}
