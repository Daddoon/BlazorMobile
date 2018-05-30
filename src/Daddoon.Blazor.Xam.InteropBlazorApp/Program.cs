using Daddoon.Blazor.Xam.InteropApp.Common.Interfaces;
using Daddoon.Blazor.Xam.InteropApp.UWP.Services;
using Microsoft.AspNetCore.Blazor.Browser.Rendering;
using Microsoft.AspNetCore.Blazor.Browser.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Daddoon.Blazor.Xam.InteropBlazorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new BrowserServiceProvider(services =>
            {
                services.AddSingleton<IXamarinBridge, XamarinBridgeProxy>();
                // Add any custom services here
            });

            var br = new BrowserRenderer(serviceProvider);

            br.AddComponent<DaddoonBlazorExtensionScripts>("daddoon");
            br.AddComponent<App>("app");
        }
    }
}
