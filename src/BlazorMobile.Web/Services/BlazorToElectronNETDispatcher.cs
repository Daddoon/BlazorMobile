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
        private static MethodInfo _remoteReceiveMethod = null;

        private static MethodProxy CallRemoteReceive(MethodProxy methodProxy)
        {
            if (_remoteReceiveMethod == null)
            {
                Type contextBridgeClass = Type.GetType("BlazorMobile.Interop.ContextBridge, BlazorMobile");

                if (contextBridgeClass == null)
                {
                    throw new InvalidOperationException("Unable to find BlazorMobile base assembly in executing runtime. Check that your shared device project is referenced on your ElectronNET project.");
                }

                _remoteReceiveMethod = contextBridgeClass.GetMethod("Receive", BindingFlags.Public | BindingFlags.Static);
            }

            return (MethodProxy)_remoteReceiveMethod.Invoke(null, new object[] { methodProxy });
        }

        public static Task Send(MethodProxy methodProxy)
        {
            MethodProxy result = methodProxy;

            try
            {
                result = CallRemoteReceive(methodProxy);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //Something gone wrong. Writting error and cancelling parent task
                result.TaskSuccess = false;
            }

            BlazorCommonDispatcher.Receive(result);

            return Task.CompletedTask;
        }
    }
}
