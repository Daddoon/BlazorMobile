using BlazorMobile.Common;
using BlazorMobile.Common.Services;
using BlazorMobile.Sample.Common.Interfaces;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using BlazorMobile.Sample.Blazor.Helpers;

namespace BlazorMobile.Sample.Blazor
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
