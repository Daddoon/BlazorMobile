using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("BlazorMobile.ElectronNET")]
namespace BlazorMobile.Helper
{
    internal static class PlatformHelper
    {
        private static Func<bool> _isElectronActive;

        public static bool IsElectronActive()
        {
            return _isElectronActive != null ? _isElectronActive() : false;
        }

        /// <summary>
        /// Set the delegate used to answer the PlatformHelper.IsElectronActive method
        /// when called from the BlazorMobile assembly. If unset, will return false
        /// </summary>
        /// <param name="action"></param>
        internal static void SetIsElectronActive(Func<bool> action)
        {
            _isElectronActive = action;
        }
    }
}
