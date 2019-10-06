using BlazorMobile.Interop;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorMobile.Services
{
    internal class BlazorMobileWebApplicationPlatform : IWebApplicationPlatform
    {
        public string GetBaseURL()
        {
            return $"http://{WebApplicationFactory.GetLocalWebServerIP()}:{WebApplicationFactory.GetHttpPort()}";
        }
    }
}
