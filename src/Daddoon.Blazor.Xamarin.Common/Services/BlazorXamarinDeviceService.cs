using Daddoon.Blazor.Xam.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Daddoon.Blazor.Xam.Common.Services
{
    public class BlazorXamarinDeviceService : IBlazorXamarinDeviceService
    {
        public Task<string> GetRuntimePlatform()
        {
            return MethodDispatcher.CallMethodAsync<string>(MethodBase.GetCurrentMethod());
        }
    }
}
