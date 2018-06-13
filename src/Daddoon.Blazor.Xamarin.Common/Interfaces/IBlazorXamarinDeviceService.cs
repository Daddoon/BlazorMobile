using Daddoon.Blazor.Xam.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Daddoon.Blazor.Xam.Common.Interfaces
{
    [ProxyInterface]
    public interface IBlazorXamarinDeviceService
    {
        Task<string> GetRuntimePlatform();
    }
}
