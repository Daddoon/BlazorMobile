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
using Xamarin.Forms;
using ElectronNET.API.Entities;
using BlazorMobile.ElectronNET.Services;
using BlazorMobile.Services;
using BlazorMobile.Sample.AppPackage;

namespace BlazorMobile.Sample.Desktop
{
    public class Startup
    {
        private bool useWASM = false;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            useWASM = (bool)Configuration.GetValue<bool>("UseWasmEngine");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddNewtonsoftJson();
            services.AddRazorPages();
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
                    var uriHelper = s.GetRequiredService<NavigationManager>();
                    return new HttpClient
                    {
                        BaseAddress = new Uri(uriHelper.BaseUri)
                    };
                });
            }

            ServicesHelper.ConfigureCommonServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!useWASM)
            {
                app.UseResponseCompression();

                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                }

                app.UseBlazorFrameworkFiles();

                app.UseStaticFiles();
                app.UseRouting();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapBlazorHub();
                    endpoints.MapDefaultControllerRoute();
                    endpoints.MapBlazorMobileRequestValidator();
                    endpoints.MapFallbackToPage("/server_index");
                });

                //Initialize Blazor app from .NET Core / Server-side
                BlazorMobileService.OnBlazorMobileLoaded += (object source, BlazorMobileOnFinishEventArgs eventArgs) =>
                {
                    Console.WriteLine($"Initialization success: {eventArgs.Success}");
                    Console.WriteLine("Device is: " + BlazorDevice.RuntimePlatform);
                };
            }

            app.UseBlazorMobileWithElectronNET<App>(useWASM);

            //Theses line must be registered after 'UseBlazorMobileWithElectronNET' as it internally call Xamarin.Forms.Init()
            if (useWASM)
            {
                BlazorWebViewService.Init();

                //Register our Blazor app package
                WebApplicationFactory.RegisterAppStreamResolver(AppPackageHelper.ResolveAppPackageStream);
            }

            Forms.ConfigureBrowserWindow(new BrowserWindowOptions()
            {
                //Configure the BrowserWindow that will be used for the Blazor application
            });

            //Launch the Blazor app
            Forms.LoadApplication(new App());

            // If your code already started your BlazorWebView.LaunchBlazorApp method, you should retrieve here the Electron main BrowserWindow used to create it.
            // Otherwise, return a null Task value
            var myBrowserWindow = Forms.GetBrowserWindow();
        }
    }
}
