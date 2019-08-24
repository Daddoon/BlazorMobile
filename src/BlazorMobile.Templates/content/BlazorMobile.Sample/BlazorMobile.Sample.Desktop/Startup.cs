using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.ResponseCompression;
using BlazorMobile.Common.Services;
using BlazorMobile.Common;
using BlazorMobile.Sample.Blazor.Helpers;
using System.Threading.Tasks;
using ElectronNET.API;
using BlazorMobile.Sample.Blazor;

namespace BlazorMobile.Sample.Desktop
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddNewtonsoftJson();
            services.AddServerSideBlazor();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            // Server Side Blazor doesn't register HttpClient by default
            if (!services.Any(x => x.ServiceType == typeof(HttpClient)))
            {
                // Setup HttpClient for server side in a client side compatible fashion
                services.AddScoped<HttpClient>(s =>
                {
                    // Creating the URI helper needs to wait until the JS Runtime is initialized, so defer it.
                    var uriHelper = s.GetRequiredService<IUriHelper>();
                    return new HttpClient
                    {
                        BaseAddress = new Uri(uriHelper.GetBaseUri())
                    };
                });
            }

            ServicesHelper.ConfigureCommonServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBlazorDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseClientSideBlazorFiles<BlazorMobile.Sample.Blazor.Startup>();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                var componentBuilder = endpoints.MapBlazorHub<MobileApp>("app");
                endpoints.MapDefaultControllerRoute();
                endpoints.MapFallbackToClientSideBlazor<BlazorMobile.Sample.Blazor.Startup>("server_index.html");
            });

            app.UseBlazorMobileWithElectronNET(typeof(App));

            BlazorMobileService.Init((bool success) =>
            {
                Console.WriteLine($"Initialization success: {success}");
                Console.WriteLine("Device is: " + BlazorDevice.RuntimePlatform);
            });

            Task.Run(async () => await Electron.WindowManager.CreateWindowAsync());
        }
    }
}
