using BlazorMobile.Common.Helpers;
using BlazorMobile.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMobile.Common.Services
{
    public class BlazorXamarinDeviceService : IBlazorXamarinDeviceService
    {
        public Task<string> GetRuntimePlatform()
        {
            return MethodDispatcher.CallMethodAsync<string>(MethodBase.GetCurrentMethod());
        }

        public Task<bool> IsElectronActive()
        {
            return MethodDispatcher.CallMethodAsync<bool>(MethodBase.GetCurrentMethod());
        }

        public Task StartupMetadataForElectron(string baseURL, string userDataFolder)
        {
            return MethodDispatcher.CallVoidMethodAsync(MethodBase.GetCurrentMethod(), baseURL, userDataFolder);
        }

        public Task WriteLine(string message)
        {
            //Dispatch message on browser
            Console.WriteLine(message);

            //Dispatch message on native
            return MethodDispatcher.CallVoidMethodAsync(MethodBase.GetCurrentMethod(), message);
        }
    }
}
