using BlazorMobile.Common.Models;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BlazorMobile.Web")]
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

        /// <summary>
        /// Flag BlazorMobile common assembly as using ElectronNET
        /// This is mainly a bridge of internals in order to notify BlazorMobile main assembly that it should behave
        /// like on ElectronNET too.
        /// </summary>
        /// <param name="electronUsage"></param>
        public static void SetElectronNETUsage(bool electronUsage)
        {
            _isUsingElectron = electronUsage;
        }

        public static bool IsBlazorMobile()
        {
            return !IsElectronNET();
        }

        public static bool IsElectronNET()
        {
            return _isUsingElectron;
        }
    }
}
