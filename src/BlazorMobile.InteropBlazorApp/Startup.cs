using BlazorMobile.Common;
using BlazorMobile.Common.Services;
using BlazorMobile.InteropApp.Common.Interfaces;
using BlazorMobile.InteropApp.UWP.Services;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BlazorMobile.InteropBlazorApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IXamarinBridge, XamarinBridgeProxy>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");

            BlazorWebViewService.Init(app, "blazorXamarin", (bool success) =>
            {
                Console.WriteLine($"Initialization success: {success}");
                Console.WriteLine("Device is: " + Device.RuntimePlatform);
            });
        }
    }
}
