using Daddoon.Blazor.Xam.Common;
using Daddoon.Blazor.Xam.Common.Services;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Daddoon.Blazor.Xam.BlazorApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
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
