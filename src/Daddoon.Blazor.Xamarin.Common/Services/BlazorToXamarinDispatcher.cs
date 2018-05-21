using Microsoft.AspNetCore.Blazor.Browser.Interop;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daddoon.Blazor.Xam.Common.Services
{
    public static class BlazorToXamarinDispatcher
    {
        public static object Send(string csharpProxy)
        {
            var result = RegisteredFunction.Invoke<string>("contextBridgeSend", csharpProxy);

            //TODO: Return something useful
            return null;
        }
    }
}
