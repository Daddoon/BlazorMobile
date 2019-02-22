using Daddoon.Blazor.Xam.Common.Interop;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Daddoon.Blazor.Xam.Common.Attributes;
using Daddoon.Blazor.Xam.Common.Serialization;
using Daddoon.Blazor.Xam.Common.Models;
using System.Threading;

namespace Daddoon.Blazor.Xam.Common.Services
{
    public static class MethodDispatcher
    {
        /// <summary>
        /// Fake type just to return void value wrapped around a simple Task type
        /// </summary>
        internal class IgnoredType
        {

        }

        private static Dictionary<int, TaskDispatch> _taskDispatcher = new Dictionary<int, TaskDispatch>();

        private static int taskQueueId = 0;

        internal static Task<TReturnType> CreateTaskDispatcher<TReturnType>(out int taskIdentity)
        {
            taskQueueId++;
            taskIdentity = taskQueueId;

            var taskDispatch = new TaskDispatch();
            _taskDispatcher.Add(taskIdentity, taskDispatch);

            taskDispatch.CancelTokenSource = new CancellationTokenSource();
            taskDispatch.CancelToken = taskDispatch.CancelTokenSource.Token;

            var taskAction = new Task<TReturnType>(() =>
            {
                //Check if we are first from a CancellationContext
                if (taskDispatch.CancelToken.IsCancellationRequested)
                {
                    taskDispatch.CancelToken.ThrowIfCancellationRequested();
                }

                if (taskDispatch.ResultData == null || !taskDispatch.ResultData.TaskSuccess)
                    return default(TReturnType);

                return (TReturnType)taskDispatch.ResultData.ReturnValue;

            }, taskDispatch.CancelToken);

            taskDispatch.TaskId = taskIdentity;
            taskDispatch.ResultAction = taskAction;

            return taskAction;
        }

        internal static void SetTaskResult(int taskIdentity, MethodProxy resultProxy)
        {
            if (!_taskDispatcher.ContainsKey(taskIdentity))
                return;
            _taskDispatcher[taskIdentity].ResultData = resultProxy;
        }

        internal static Task GetTaskDispatcher(int taskIdentity)
        {
            if (_taskDispatcher.ContainsKey(taskIdentity))
                return _taskDispatcher[taskIdentity].ResultAction;
            return null;
        }

        internal static void ClearTask(int taskIdentity)
        {
            if (_taskDispatcher.ContainsKey(taskIdentity))
            {
                var task = _taskDispatcher[taskIdentity];
                if (!task.ResultAction.IsCompleted)
                {
                    task.CancelTask();
                }

                _taskDispatcher.Remove(taskIdentity);
            }
        }

        #region VOID CALL

        public static void CallVoidMethod(MethodBase method)
        {
            InternalCallMethod<IgnoredType>(method, null, null, false);
        }

        public static void CallVoidMethod(MethodBase method, params object[] args)
        {
            InternalCallMethod<IgnoredType>(method, null, args, false);
        }

        #region SYNTACTIC SUGAR HELPER

        //Just a Generic argument signature for easier signature call

        public static void CallVoidMethod<TGenericArg>(MethodBase method, params object[] args)
        {
            InternalCallMethod<IgnoredType>(
                method,
                new Type[] { typeof(TGenericArg) }
                , args, false);
        }

        public static void CallVoidMethod<TGenericArg1, TGenericArg2>(MethodBase method, params object[] args)
        {
            InternalCallMethod<IgnoredType>(
                method,
                new Type[] { typeof(TGenericArg1), typeof(TGenericArg2) }
                , args, false);
        }

        public static void CallVoidMethod<TGenericArg1, TGenericArg2, TGenericArg3>(MethodBase method, params object[] args)
        {
            InternalCallMethod<IgnoredType>(
                method,
                new Type[] { typeof(TGenericArg1), typeof(TGenericArg2), typeof(TGenericArg3) }
                , args, false);
        }

        public static void CallVoidMethod<TGenericArg1, TGenericArg2, TGenericArg3, TGenericArg4>(MethodBase method, params object[] args)
        {
            InternalCallMethod<IgnoredType>(
                method,
                new Type[] { typeof(TGenericArg1), typeof(TGenericArg2), typeof(TGenericArg4) }
                , args, false);
        }

        #endregion

        public static void CallVoidMethod(MethodBase method, Type[] genericParameters, params object[] args)
        {
            InternalCallMethod<IgnoredType>(method, genericParameters, args, false);
        }

        #endregion

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
                new Type[] { typeof(TGenericArg1), typeof(TGenericArg2), typeof(TGenericArg4) }
                , args, true);
        }

        public static Task CallVoidMethodAsync(MethodBase method, Type[] genericParameters, params object[] args)
        {
            return InternalCallMethod<IgnoredType>(method, genericParameters, args, true);
        }

        #endregion


        #region WITH RETURN VALUE

        public static TReturnType CallMethod<TReturnType>(MethodBase method)
        {
            return InternalCallMethod<TReturnType>(method, null, null, false).GetAwaiter().GetResult();
        }

        public static TReturnType CallMethod<TReturnType>(MethodBase method, params object[] args)
        {
            return InternalCallMethod<TReturnType>(method, null, args, false).GetAwaiter().GetResult();
        }

        public static TReturnType CallMethod<TReturnType, TGenericArg>(MethodBase method, params object[] args)
        {
            return InternalCallMethod<TReturnType>(
                method,
                new Type[] { typeof(TGenericArg) }
                , args, false).GetAwaiter().GetResult();
        }

        public static TReturnType CallMethod<TReturnType, TGenericArg1, TGenericArg2>(MethodBase method, params object[] args)
        {
            return InternalCallMethod<TReturnType>(
                method,
                new Type[] { typeof(TGenericArg1), typeof(TGenericArg2) }
                , args, false).GetAwaiter().GetResult();
        }

        public static TReturnType CallMethod<TReturnType, TGenericArg1, TGenericArg2, TGenericArg3>(MethodBase method, params object[] args)
        {
            return InternalCallMethod<TReturnType>(
                method,
                new Type[] { typeof(TGenericArg1), typeof(TGenericArg2), typeof(TGenericArg3) }
                , args, false).GetAwaiter().GetResult();
        }

        public static TReturnType CallMethod<TReturnType, TGenericArg1, TGenericArg2, TGenericArg3, TGenericArg4>(MethodBase method, params object[] args)
        {
            return InternalCallMethod<TReturnType>(
                method,
                new Type[] { typeof(TGenericArg1), typeof(TGenericArg2), typeof(TGenericArg4) }
                , args, false).GetAwaiter().GetResult();
        }

        public static TReturnType CallMethod<TReturnType>(MethodBase method, Type[] genericParameters, params object[] args)
        {
            return InternalCallMethod<TReturnType>(method, genericParameters, args, false).GetAwaiter().GetResult();
        }

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
                throw new ArgumentException("Given class method does not contain any possible ProxyInterface. Be sure to add [ProxyInterface] attribute on top of your interface definition");
            }

            //As we are using a DispatchProxy, the targetMethod should be the interface MethodInfo (and not a class MethodInfo)
            int index = MethodProxyHelper.GetInterfaceMethodIndex(methodBase.DeclaringType, method, iface);

            if (index == -1)
                return Task.FromResult(default(TReturnType));

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

            switch (ContextHelper.GetExecutingContext())
            {
                case Models.ExecutingContext.Blazor:
                    int taskId = 0;
                    var task = CreateTaskDispatcher<TReturnType>(out taskId);

                    methodProxy.TaskIdentity = taskId;

                    BlazorToXamarinDispatcher.Send(methodProxy);

                    return task;
                default:
                    return Task.FromResult(default(TReturnType));
            }

            #endregion
        }

        #endregion
    }
}
