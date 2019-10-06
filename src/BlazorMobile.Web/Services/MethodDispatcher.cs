using BlazorMobile.Common.Interop;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using BlazorMobile.Common.Attributes;
using BlazorMobile.Common.Serialization;
using BlazorMobile.Common.Models;
using System.Threading;
using System.Runtime.CompilerServices;
using BlazorMobile.Common.Helpers;

namespace BlazorMobile.Common.Services
{
    public static class MethodDispatcher
    {
        private static Dictionary<Guid, TaskDispatch> _taskDispatcher = new Dictionary<Guid, TaskDispatch>();

        internal static Task<TReturnType> CreateTaskDispatcher<TReturnType>(MethodProxy methodProxy)
        {
            Guid taskId = Guid.Empty;
            var task = CreateTaskDispatcher<TReturnType>(out taskId);

            methodProxy.TaskIdentity = taskId;

            return task;
        }

        internal static Task<TReturnType> CreateTaskDispatcher<TReturnType>(out Guid taskIdentity)
        {
            taskIdentity = Guid.NewGuid();

            var taskDispatch = new TaskDispatch();
            _taskDispatcher.Add(taskIdentity, taskDispatch);

            taskDispatch.CancelTokenSource = new CancellationTokenSource();
            taskDispatch.CancelToken = taskDispatch.CancelTokenSource.Token;

            var taskAction = new Task<TReturnType>(() =>
            {
                //Will throw here if task faulted
                taskDispatch.ThrowExceptionIfFaulted();

                if (taskDispatch.ResultData == null || !taskDispatch.ResultData.TaskSuccess)
                    return default(TReturnType);

                //In case of a void call, we must return the IgnoredType for the caller instead of TaskComplete
                if (typeof(TReturnType) == typeof(IgnoredType))
                {
                    taskDispatch.ResultData.ReturnValue = new IgnoredType();
                }

                return (TReturnType)taskDispatch.ResultData.ReturnValue;

            }, taskDispatch.CancelToken);

            taskDispatch.TaskId = taskIdentity;
            taskDispatch.ResultAction = taskAction;

            return taskAction;
        }

        internal static void SetTaskResult(Guid taskIdentity, MethodProxy resultProxy)
        {
            if (!_taskDispatcher.ContainsKey(taskIdentity))
                return;
            _taskDispatcher[taskIdentity].ResultData = resultProxy;
        }

        internal static Task GetTaskDispatcher(Guid taskIdentity)
        {
            if (_taskDispatcher.ContainsKey(taskIdentity))
                return _taskDispatcher[taskIdentity].ResultAction;
            return null;
        }

        internal static void SetTaskAsFaulted<T>(Guid taskIdentity, T ex) where T : Exception
        {
            if (_taskDispatcher.ContainsKey(taskIdentity))
            {
                var task = _taskDispatcher[taskIdentity];
                task.SetTaskAsFaulted<T>(ex);
            }
        }

        internal static void CancelTask(Guid taskIdentity)
        {
            if (_taskDispatcher.ContainsKey(taskIdentity))
            {
                var task = _taskDispatcher[taskIdentity];

                if (!task.CancelTokenSource.IsCancellationRequested)
                {
                    task.CancelTokenSource.Cancel();
                }
            }
        }

        internal static void ClearTask(Guid taskIdentity)
        {
            if (_taskDispatcher.ContainsKey(taskIdentity))
            {
                var task = _taskDispatcher[taskIdentity];

                if (!task.ResultAction.IsCompleted)
                {
                    try
                    {
                        CancelTask(taskIdentity);
                    }
                    catch (Exception)
                    {
                    }
                }

                _taskDispatcher.Remove(taskIdentity);
            }
        }

        #region VOID CALL ASYNC

        public static Task CallVoidMethodAsync(MethodBase method)
        {
            return InternalCallMethod<IgnoredType>(method, null, null, true);
        }

        public static Task CallVoidMethodAsync(MethodBase method, params object[] args)
        {
            return InternalCallMethod<IgnoredType>(method, null, args, true);
        }

        public static Task CallVoidMethodAsync<TGenericArg>(MethodBase method, params object[] args)
        {
            return InternalCallMethod<IgnoredType>(
                method,
                new Type[] { typeof(TGenericArg) }
                , args, true);
        }

        public static Task CallVoidMethodAsync<TGenericArg1, TGenericArg2>(MethodBase method, params object[] args)
        {
            return InternalCallMethod<IgnoredType>(
                method,
                new Type[] { typeof(TGenericArg1), typeof(TGenericArg2) }
                , args, true);
        }

        public static Task CallVoidMethodAsync<TGenericArg1, TGenericArg2, TGenericArg3>(MethodBase method, params object[] args)
        {
            return InternalCallMethod<IgnoredType>(
                method,
                new Type[] { typeof(TGenericArg1), typeof(TGenericArg2), typeof(TGenericArg3) }
                , args, true);
        }

        public static Task CallVoidMethodAsync<TGenericArg1, TGenericArg2, TGenericArg3, TGenericArg4>(MethodBase method, params object[] args)
        {
            return InternalCallMethod<IgnoredType>(
                method,
                new Type[] { typeof(TGenericArg1), typeof(TGenericArg2), typeof(TGenericArg3), typeof(TGenericArg4) }
                , args, true);
        }

        public static Task CallVoidMethodAsync(MethodBase method, Type[] genericParameters, params object[] args)
        {
            return InternalCallMethod<IgnoredType>(method, genericParameters, args, true);
        }

        #endregion

        #region WITH RETURN VALUE ASYNC

        public static Task<TReturnType> CallMethodAsync<TReturnType>(MethodBase method)
        {
            return InternalCallMethod<TReturnType>(method, null, null, true);
        }

        public static Task<TReturnType> CallMethodAsync<TReturnType>(MethodBase method, params object[] args)
        {
            return InternalCallMethod<TReturnType>(method, null, args, true);
        }

        public static Task<TReturnType> CallMethodAsync<TReturnType, TGenericArg>(MethodBase method, params object[] args)
        {
            return InternalCallMethod<TReturnType>(
                method,
                new Type[] { typeof(TGenericArg) }
                , args, true);
        }

        public static Task<TReturnType> CallMethodAsync<TReturnType, TGenericArg1, TGenericArg2>(MethodBase method, params object[] args)
        {
            return InternalCallMethod<TReturnType>(
                method,
                new Type[] { typeof(TGenericArg1), typeof(TGenericArg2) }
                , args, true);
        }

        public static Task<TReturnType> CallMethodAsync<TReturnType, TGenericArg1, TGenericArg2, TGenericArg3>(MethodBase method, params object[] args)
        {
            return InternalCallMethod<TReturnType>(
                method,
                new Type[] { typeof(TGenericArg1), typeof(TGenericArg2), typeof(TGenericArg3) }
                , args, true);
        }

        public static Task<TReturnType> CallMethodAsync<TReturnType, TGenericArg1, TGenericArg2, TGenericArg3, TGenericArg4>(MethodBase method, params object[] args)
        {
            return InternalCallMethod<TReturnType>(
                method,
                new Type[] { typeof(TGenericArg1), typeof(TGenericArg2), typeof(TGenericArg4) }
                , args, true);
        }

        public static Task<TReturnType> CallMethodAsync<TReturnType>(MethodBase method, Type[] genericParameters, params object[] args)
        {
            return InternalCallMethod<TReturnType>(method, genericParameters, args, true);
        }

        #endregion

        private static Task<TReturnType> InternalCallMethod<TReturnType>(MethodBase methodBase, Type[] genericParameters, object[] args, bool isAsync)
        {
            MethodInfo method = (MethodInfo)methodBase;

            if (genericParameters == null)
            {
                genericParameters = new Type[0];
            }

            if (args == null)
            {
                args = new object[0];
            }

            #region Invokation

            MethodProxy methodProxy = new MethodProxy();

            var iface = methodBase.DeclaringType.GetInterfaces().FirstOrDefault(p => p.GetCustomAttribute<ProxyInterfaceAttribute>() != null);

            if (iface == null)
            {
                //First exception chance. We are maybe in an Async task context. Trying to retrieve base method
                 MethodBase noAsyncMethodBase = GetRealMethodBaseFromAsyncMethod(methodBase);

                if (noAsyncMethodBase != null)
                {
                    //Replacing local references by the found one
                    methodBase = noAsyncMethodBase;
                    method = (MethodInfo)methodBase;
                    iface = methodBase.DeclaringType.GetInterfaces().FirstOrDefault(p => p.GetCustomAttribute<ProxyInterfaceAttribute>() != null);
                }
            }

            if (iface == null)
            {
                throw new ArgumentException("Unable to find the method to call on the interface. Be sure to add the [ProxyInterface] attribute on top of your interface definition. If using MethodBase.GetCurrentMethod(), check that your calling method is not marked as 'async', as it may hide the real method definition.");
            }

            //As we are using a DispatchProxy, the targetMethod should be the interface MethodInfo (and not a class MethodInfo)
            int index = MethodProxyHelper.GetInterfaceMethodIndex(methodBase.DeclaringType, method, iface);

            if (index == -1)
                return Task.FromResult(default(TReturnType));

            //Only used with UWP that does not support GetInterfaceMap calls with .NET Native toolchain
            //Other implementation will call the method according to the method index
            methodProxy.MethodName = method.Name;

            methodProxy.MethodIndex = index;
            methodProxy.GenericTypes = genericParameters.Select(p => new TypeProxy(p)).ToArray();

            //On this case, the DeclaringType should be the Interface type
            methodProxy.InterfaceType = new TypeProxy(iface);

            methodProxy.ReturnType = new TypeProxy(typeof(TReturnType));

            //NOTE: We must flag if the current Task is Async or not, as explicit Async method must wrap the result out on a Task.FromResult.
            //This way, the Receive dispatcher know that it must Serialize the Task result, and not the Task object itself
            methodProxy.AsyncTask = isAsync;

            //We will let the serializer do the thing for RunTime values
            methodProxy.Parameters = args;

            Task<TReturnType> task = Task.FromResult(default(TReturnType));

            //TODO: Should refactorate in order to avoid copy/past for this switch
            switch (ContextHelper.GetExecutingContext())
            {
                case Models.ExecutingContext.Blazor:
                    task = CreateTaskDispatcher<TReturnType>(methodProxy);
                    BlazorToXamarinDispatcher.Send(methodProxy);
                    break;
                case Models.ExecutingContext.ElectronNET:
                    task = CreateTaskDispatcher<TReturnType>(methodProxy);
                    BlazorToElectronNETDispatcher.Send(methodProxy);
                    break;
            }

            return task;

            #endregion
        }

        //See: https://stackoverflow.com/a/28633192/1417492
        private static MethodBase GetRealMethodBaseFromAsyncMethod(MethodBase asyncMethod)
        {
            try
            {
                var generatedType = asyncMethod.DeclaringType;
                var originalType = generatedType.DeclaringType;
                var matchingMethods =
                    from methodInfo in originalType.GetMethods()
                    let attr = methodInfo.GetCustomAttribute<AsyncStateMachineAttribute>()
                    where attr != null && attr.StateMachineType == generatedType
                    select methodInfo;

                // If this throws, the async method scanning failed.
                return matchingMethods.Single();
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteException(ex);
            }

            return null;
        }
    }
}
