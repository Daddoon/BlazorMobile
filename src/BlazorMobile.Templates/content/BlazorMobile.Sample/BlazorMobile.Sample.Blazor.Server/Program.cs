using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace BlazorMobile.Sample.Blazor.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                //Uncomment this line if using IIS Express or IIS integration in project launch settings
                //.UseIISIntegration()

                //Uncomment theses lines if using the default project launch settings
                .UseKestrel(options => {
                    options.Listen(IPAddress.Loopback, 5080); //HTTP port
                })
            
            .UseStartup<Startup>();
    }
}
