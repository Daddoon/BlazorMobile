using BlazorMobile.Common.Interop;
using BlazorMobile.Common.Models;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("BlazorMobile.Web")]
[assembly: InternalsVisibleTo("BlazorMobile.ElectronNET")]
namespace BlazorMobile.Common.Services
{
    internal static class ContextHelper
    {
        /// <summary>
        /// Get the current executing context, if it's Blazor or Xamarin
        /// TODO: At the moment only return Blazor, as for a Blazor to Xamarin call scenario
        /// </summary>
        /// <returns></returns>
        public static ExecutingContext GetExecutingContext()
        {
            if (IsBlazorMobile())
            {
                return ExecutingContext.Blazor;
            }
            else if (IsElectronNET())
            {
                return ExecutingContext.ElectronNET;
            }

            //Assuming Blazor if not found
            return ExecutingContext.Blazor;
        }

        private static bool _isUsingElectron = false;

        private static bool _isUsingWASM = true;

        /// <summary>
        /// Flag BlazorMobile common assembly as using ElectronNET
        /// This is mainly a bridge of internals in order to notify BlazorMobile main assembly that it should behave
        /// like on ElectronNET too.
        /// </summary>
        /// <param name="electronUsage"></param>
        public static void SetElectronNETUsage(bool electronUsage, bool useWasm)
        {
            _isUsingElectron = electronUsage;
            _isUsingWASM = useWasm;
        }

        public static bool IsUsingWASM()
        {
            return _isUsingWASM;
        }

        public static bool IsBlazorMobile()
        {
            return !IsElectronNET();
        }

        public static bool IsElectronNET()
        {
            return _isUsingElectron;
        }

        private static Func<MethodProxy, Task<MethodProxy>> _nativeReceiveMethod = null;

        /// <summary>
        /// This shorthand is only called from ElectronNET implementation / runtime.
        /// The main reason is that BlazorMobile.Web does not have a direct to the native side Receive method even if in ElectronNET implementation
        /// they are in the same memory space. This shorthand is used as a Bridge to this method, as *BlazorMobile.ElectronNET has access to both assemblies.
        /// </summary>
        /// <param name="methodProxy"></param>
        /// <returns></returns>
        public static Task<MethodProxy> CallNativeReceive(MethodProxy methodProxy)
        {
            if (_nativeReceiveMethod == null)
            {
                throw new InvalidOperationException("ERROR: Native receive delegate has not been set in BlazorMobile.Common");
            }

            return _nativeReceiveMethod(methodProxy);
        }

        internal static void SetNativeReceive(Func<MethodProxy, Task<MethodProxy>> nativeReceive)
        {
            if (_nativeReceiveMethod != null)
            {
                return;
            }

            _nativeReceiveMethod = nativeReceive;
        }
    }
}
