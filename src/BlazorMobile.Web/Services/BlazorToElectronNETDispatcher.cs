using BlazorMobile.Common.Components;
using BlazorMobile.Common.Helpers;
using BlazorMobile.Common.Interop;
using BlazorMobile.Common.Serialization;
using BlazorMobile.Web.Services;
using Microsoft.JSInterop;
using System;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;

namespace BlazorMobile.Common.Services
{
    internal static class BlazorToElectronNETDispatcher
    {
        public static async Task Send(MethodProxy methodProxy)
        {
            MethodProxy result = methodProxy;

            try
            {
                result = await ContextHelper.CallNativeReceive(methodProxy);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //Something gone wrong. Writting error and cancelling parent task
                result.TaskSuccess = false;
            }

            BlazorCommonDispatcher.Receive(result);

            return;
        }
    }
}
