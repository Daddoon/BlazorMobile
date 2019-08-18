using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("BlazorMobile")]
[assembly: InternalsVisibleTo("BlazorMobile.Web")]
[assembly: InternalsVisibleTo("BlazorMobile.Android")]
[assembly: InternalsVisibleTo("BlazorMobile.iOS")]
[assembly: InternalsVisibleTo("BlazorMobile.UWP")]
namespace BlazorMobile.Common.Models
{
    /// <summary>
    /// Fake type just to return void value wrapped around a simple Task type
    /// </summary>
    internal class IgnoredType
    {

    }
}
