using BlazorMobile.Common.Helpers;
using BlazorMobile.Common.Interop;
using BlazorMobile.Common.Models;
using BlazorMobile.Common.Serialization;
using BlazorMobile.Components;
using BlazorMobile.Extensions;
using BlazorMobile.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: InternalsVisibleTo("BlazorMobile.Web")]
[assembly: InternalsVisibleTo("BlazorMobile.ElectronNET")]
namespace BlazorMobile.Interop
{
    internal static class ContextBridge
    {
        private static object GetDefault(Type type)
        {
            if (type == typeof(void))
            {
                return null;
            }

            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        public static void ClearRequestValues(MethodProxy methodResult)
        {
            if (methodResult == null)
                return;

            methodResult.GenericTypes = null;
            methodResult.InterfaceType = null;
            methodResult.Parameters = null;
        }

        private static object GetResultFromTask(Type returnType, Task taskResult)
        {
            if (returnType == null || returnType == typeof(void) || returnType == typeof(Task) || returnType == typeof(IgnoredType))
            {
                return null;
            }

            try
            {
                if (taskResult.IsCompleted == false)
                {
                    taskResult.GetAwaiter().GetResult();
                }

                var result = taskResult.GetType().GetProperty("Result").GetValue(taskResult);

                return result;
            }
            catch (Exception)
            {
                return GetDefault(returnType);
            }
        }

        public static string GetJSONReturnValue(MethodProxy methodResult)
        {
            ClearRequestValues(methodResult);
            return BridgeSerializer.Serialize(methodResult);
        }

        public static MethodProxy GetMethodProxyFromJSON(ref string json)
        {
            return BridgeSerializer.Deserialize<MethodProxy>(ref json);
        }

        private static void DispatchInvokation(MethodProxy methodProxy, MethodInfo baseMethod, object concreteService)
        {
            if (methodProxy.GenericTypes != null && methodProxy.GenericTypes.Length > 0)
            {
                Type[] genericTypes = methodProxy.GenericTypes.Select(p => p.ResolvedType()).ToArray();

                methodProxy.ReturnValue = baseMethod.MakeGenericMethod(genericTypes).Invoke(concreteService, methodProxy.Parameters);
                methodProxy.TaskSuccess = true;
            }
            else
            {
                methodProxy.ReturnValue = baseMethod.Invoke(concreteService, methodProxy.Parameters);
                methodProxy.TaskSuccess = true;
            }
        }

        public static async Task<MethodProxy> Receive(MethodProxy methodProxy)
        {
            object defaultValue = default(object);

            try
            {
                Type iface = methodProxy.InterfaceType.ResolvedType();
                object concreteService = DependencyServiceExtension.Get(iface);

                if (concreteService == null)
                {
                    //iface should not be null
                    string error = $"The service implementation class of your interface '{iface.Name}' was not found on native side. If you are targeting UWP with .NET native toolchain, you must register your services through 'DependencyService.Register' at startup as the toolchain may strip your class from assembly at build time";
                    ConsoleHelper.WriteError(error);
                    throw new InvalidOperationException(error);
                }

                MethodInfo baseMethod = MethodProxyHelper.GetClassMethodInfo(concreteService.GetType(), iface, methodProxy);

                //In case of failure, getting Default Return Type
                defaultValue = GetDefault(baseMethod.ReturnType);

                DispatchInvokation(methodProxy, baseMethod, concreteService);

                if (methodProxy.AsyncTask)
                {
                    await ((Task)methodProxy.ReturnValue);
                }

                methodProxy.ReturnValue = GetResultFromTask(methodProxy.ReturnType.ResolvedType(), (Task)methodProxy.ReturnValue);

            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError($"[Native] - {(ex.InnerException != null ? ex.InnerException.Message : ex.Message)}");

                methodProxy.ExceptionDescriptor = new ExceptionDescriptor(ex.InnerException != null ? ex.InnerException : ex);
                methodProxy.ReturnValue = defaultValue;
                methodProxy.TaskSuccess = false;
            }

            return methodProxy;
        }
    }
}
