using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Daddoon.Blazor.Xam.InteropApp.Common.Interfaces
{
    public interface IXamarinBridge
    {
        Task DisplayAlert(string title, string msg, string cancel);
    }
}
