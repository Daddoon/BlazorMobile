using BlazorMobile.Common.Helpers;
using BlazorMobile.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BlazorMobile.Services
{
    public class BlazorXamarinDeviceService : IBlazorXamarinDeviceService
    {
        public Task<string> GetRuntimePlatform()
        {
            return Task.FromResult(Device.RuntimePlatform);
        }

        public Task WriteLine(string message)
        {
            ConsoleHelper.WriteLine(message);
            return Task.CompletedTask;
        }
    }
}
