using Daddoon.Blazor.Xam.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Daddoon.Blazor.Xam.Services
{
    public class BlazorXamarinDeviceService : IBlazorXamarinDeviceService
    {
        public Task<string> GetRuntimePlatform()
        {
            return Task.FromResult(Device.RuntimePlatform);
        }
    }
}
