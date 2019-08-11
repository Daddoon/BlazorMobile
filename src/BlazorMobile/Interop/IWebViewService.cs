using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMobile.Interop
{
    internal interface IWebViewService
    {
        /// <summary>
        /// Clear cookies for the local web session / local uri. Does not affect other URIs.
        /// Only needed for UWP that don't have private navigation mode
        /// </summary>
        void ClearCookies();

        void ClearWebViewData();
    }
}
