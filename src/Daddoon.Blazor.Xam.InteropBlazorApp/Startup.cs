using Daddoon.Blazor.Xam.Common;
using Daddoon.Blazor.Xam.Common.Services;
using Daddoon.Blazor.Xam.InteropApp.Common.Interfaces;
using Daddoon.Blazor.Xam.InteropApp.UWP.Services;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Daddoon.Blazor.Xam.InteropBlazorApp
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
