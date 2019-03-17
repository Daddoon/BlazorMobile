using BlazorMobile.Common.Helpers;
using BlazorMobile.Common.Interop;
using BlazorMobile.Common.Serialization;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace BlazorMobile.Common.Services
{
    public static class BlazorToXamarinDispatcher
    {
        [JSInvokable]
        public static async Task Send(MethodProxy methodProxy)
        {
            string csharpProxy = BridgeSerializer.Serialize(methodProxy);
            InternalHelper.SetTimeout(async () =>
            {
                await JSRuntime.Current.InvokeAsync<bool>("contextBridgeSend", csharpProxy);
            }, 100);
        }

        [JSInvokable]
        public static bool Receive(string methodProxyJson)
        {
            if (string.IsNullOrEmpty(methodProxyJson))
                return false;

            InternalHelper.SetTimeout(() =>
            {
                MethodProxy resultProxy = BridgeSerializer.Deserialize<MethodProxy>(methodProxyJson);
                var taskToReturn = MethodDispatcher.GetTaskDispatcher(resultProxy.TaskIdentity);
                MethodDispatcher.SetTaskResult(resultProxy.TaskIdentity, resultProxy);

                if (taskToReturn == null)
                    return;

                taskToReturn.RunSynchronously();

                MethodDispatcher.ClearTask(resultProxy.TaskIdentity);
            }, 10);

            return true;
        }
    }
}
