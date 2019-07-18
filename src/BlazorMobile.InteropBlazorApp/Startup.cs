using BlazorMobile.Common;
using BlazorMobile.Common.Services;
using BlazorMobile.InteropApp.Common.Interfaces;
using BlazorMobile.InteropBlazorApp.Helpers;
using BlazorMobile.InteropBlazorApp.Services;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BlazorMobile.InteropBlazorApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            ServicesHelper.ConfigureCommonServices(services);
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            #region DEBUG

            //Only if you want to test WebAssembly with remote debugging from a dev machine
            BlazorService.EnableClientToDeviceRemoteDebugging("192.168.1.118", 8888);

            #endregion

            BlazorService.Init(app, (bool success) =>
            {
                Console.WriteLine($"Initialization success: {success}");
                Console.WriteLine("Device is: " + Device.RuntimePlatform);
            });

            app.AddComponent<MobileApp>("app");
        }
    }
}
