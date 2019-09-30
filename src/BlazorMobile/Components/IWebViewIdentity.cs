using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorMobile.Components
{
    internal interface IWebViewIdentity
    {
        /// <summary>
        /// If true, the Blazor app booted, otherwise false.
        /// This boolean is mainly used to detect if the Blazor app finished it's loading after an OnSleep/OnResume event.
        /// If not, we may reload the Blazor app on the OnResume event, as we want our application boot to be atomic.
        /// </summary>
        bool BlazorAppLaunched { get; set; }

        int GetWebViewIdentity();
    }
}
