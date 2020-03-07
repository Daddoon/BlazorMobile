using BlazorMobile.Common.Components;
using BlazorMobile.Common.Helpers;
using BlazorMobile.Common.Metadata;
using BlazorMobile.Common.Services;
using BlazorMobile.ElectronNET.Services;
using BlazorMobile.Services;
using ElectronNET.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMobile.ElectronNET.Helpers
{
    internal static class WebExtensionHelper
    {
        public static string GetServiceURI()
        {
            return WebApplicationFactory.GetBaseURL() + MetadataConsts.ElectronBlazorMobileRequestValidationPath;
        }

        public static void InstallOnNavigatingBehavior()
        {
            try
            {
                if (HybridSupport.IsElectronActive)
                {
                    Task.Run(async () =>
                    {
                        var resultFromTypeScript = await Electron.HostHook.CallAsync<string>("add-blazormobile-navigating-behavior", GetServiceURI());

                        ConsoleHelper.WriteLine($"add-blazormobile-navigating-behavior returned: {resultFromTypeScript}");
                    });
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteException(ex);
            }
        }
    }
}
