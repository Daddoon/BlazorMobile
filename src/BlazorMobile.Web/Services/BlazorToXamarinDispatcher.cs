using BlazorMobile.Common.Components;
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
                await BlazorMobileComponent.GetJSRuntime().InvokeAsync<bool>("contextBridgeSend", csharpProxy);
            }, 100);
        }

        [JSInvokable]
        public static bool Receive(string methodProxyJson, bool success)
        {
            if (string.IsNullOrEmpty(methodProxyJson))
                return false;

            InternalHelper.SetTimeout(() =>
            {
                MethodProxy resultProxy = BridgeSerializer.Deserialize<MethodProxy>(methodProxyJson);
                var taskToReturn = MethodDispatcher.GetTaskDispatcher(resultProxy.TaskIdentity);

                if (taskToReturn == null)
                    return;

                if (success)
                {
                    MethodDispatcher.SetTaskResult(resultProxy.TaskIdentity, resultProxy);
                    taskToReturn.RunSynchronously();
                }
                else
                {
                    MethodDispatcher.CancelTask(resultProxy.TaskIdentity);
                }

                //Clear task from task list. Should then call the task to execute. It will throw if it has been cancelled
                MethodDispatcher.ClearTask(resultProxy.TaskIdentity);
            }, 10);

            return true;
        }
    }
}
