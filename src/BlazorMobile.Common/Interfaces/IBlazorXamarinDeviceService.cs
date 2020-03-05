using BlazorMobile.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMobile.Common.Interfaces
{
    [ProxyInterface]
    public interface IBlazorXamarinDeviceService
    {       
        Task<bool> IsElectronActive();

        Task StartupMetadataForElectron(string baseURL, string userDataFolder);

        Task<string> GetRuntimePlatform();

        Task WriteLine(string message);
    }
}
