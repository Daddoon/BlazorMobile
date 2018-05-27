using Daddoon.Blazor.Xam.Common.Interop;
using Daddoon.Blazor.Xam.Common.Serialization;
using Microsoft.AspNetCore.Blazor.Browser.Interop;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daddoon.Blazor.Xam.Common.Services
{
    public static class BlazorToXamarinDispatcher
    {
        public static void Send(MethodProxy methodProxy)
        {
            string csharpProxy = BridgeSerializer.Serialize(methodProxy);
            RegisteredFunction.Invoke<string>("contextBridgeSend", csharpProxy);
        }

        public static void Receive(string methodProxyJson)
        {
            if (string.IsNullOrEmpty(methodProxyJson))
                return;

            MethodProxy resultProxy = BridgeSerializer.Deserialize<MethodProxy>(methodProxyJson);
            var taskToReturn = MethodDispatcher.GetTaskDispatcher(resultProxy.TaskIdentity);
            MethodDispatcher.SetTaskResult(resultProxy.TaskIdentity, resultProxy);

            if (taskToReturn == null)
                return;

            taskToReturn.RunSynchronously();

            MethodDispatcher.ClearTask(resultProxy.TaskIdentity);
        }
    }
}
