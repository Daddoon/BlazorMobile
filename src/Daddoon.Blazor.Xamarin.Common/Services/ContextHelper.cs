using Daddoon.Blazor.Xam.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daddoon.Blazor.Xam.Common.Services
{
    public static class ContextHelper
    {
        /// <summary>
        /// Get the current executing context, if it's Blazor or Xamarin
        /// TODO: At the moment only return Blazor, as for a Blazor to Xamarin call scenario
        /// </summary>
        /// <returns></returns>
        public static ExecutingContext GetExecutingContext()
        {
            return ExecutingContext.Blazor;
        }
    }
}
