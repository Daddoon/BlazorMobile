using BlazorMobile.Common.Components;
using BlazorMobile.Common.Helpers;
using BlazorMobile.Common.Interop;
using BlazorMobile.Common.Serialization;
using Microsoft.JSInterop;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace BlazorMobile.Common.Services
{
    public static class BlazorToXamarinDispatcher
    {
        [JSInvokable]
        public static Task Send(MethodProxy methodProxy)
        {
            string csharpProxy = BridgeSerializer.Serialize(methodProxy);
            BlazorMobileComponent.GetJSRuntime().InvokeAsync<bool>("contextBridgeSend", csharpProxy);
            return Task.CompletedTask;
        }

        [JSInvokable]
        public static bool Receive(string methodProxyJson, bool socketSuccess)
        {
            if (string.IsNullOrEmpty(methodProxyJson))
                return false;

            try
            {
                MethodProxy resultProxy = BridgeSerializer.Deserialize<MethodProxy>(ref methodProxyJson);
                var taskToReturn = MethodDispatcher.GetTaskDispatcher(resultProxy.TaskIdentity);

                if (taskToReturn == null)
                    return false;

                if (socketSuccess && resultProxy.TaskSuccess)
                {
                    MethodDispatcher.SetTaskResult(resultProxy.TaskIdentity, resultProxy);
                }
                else
                {
                    Exception exception = null;

                    //If success value (from javascript) is false, like unable to connect to websocket
                    //or if the native task failed with an exception, cancel the current task, that will throw
                    if (!socketSuccess)
                    {
                        exception = new InvalidOperationException($"BlazorMobile was unable to connect to native through websocket server to execute task {resultProxy.TaskIdentity}");
                    }
                    else if (resultProxy.ExceptionDescriptor != null)
                    {
                        //We have some message to send in this case
                        exception = new Exception(resultProxy.ExceptionDescriptor.Message);
                    }
                    else
                    {
                        //Sending uncustomized message
                        exception = new InvalidOperationException($"Task {resultProxy.TaskIdentity} has thrown an exception on native side. See log for more info.");
                    }

                    MethodDispatcher.SetTaskAsFaulted(resultProxy.TaskIdentity, exception);
                }

                taskToReturn.RunSynchronously();

                //Clear task from task list. Should then call the task to execute. It will throw if it has been cancelled
                MethodDispatcher.ClearTask(resultProxy.TaskIdentity);
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteException(ex);
                return false;
            }

            return true;
        }
    }
}
