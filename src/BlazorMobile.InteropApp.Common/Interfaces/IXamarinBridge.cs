using BlazorMobile.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMobile.InteropApp.Common.Interfaces
{
    [BlazorMobile.Common.Attributes.ProxyInterface]
    public interface IXamarinBridge
    {
        Task TriggerJSInvokableTest();

        Task TriggerPostMessageTest();

        Task TriggerPostMessageTestBool();

        Task<List<string>> DisplayAlert(string title, string msg, string cancel);

        Task CallFaultyTask();
    }
}
