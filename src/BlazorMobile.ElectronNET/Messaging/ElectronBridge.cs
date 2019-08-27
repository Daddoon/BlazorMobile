using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Internals;
using Xamarin.Forms;
using System.Threading.Tasks;
using static Xamarin.Forms.Forms;

namespace BlazorMobile.ElectronNET.Messaging
{
    public class ElectronBridge
    {
        private BlazorMobilePlatformServices _platform = null;
        
        public ElectronBridge()
        {

        }

        public ElectronBridge(object platform)
        {
            _platform = (BlazorMobilePlatformServices)platform;
        }

        public void ForwardOnAlertSignalResult(AlertArguments arguments, bool isOk)
        {
            _platform.OnAlertSignalResult(arguments, isOk);
        }

        [JSInvokable]
        public static Task NotifyAlertSignalResult(DotNetObjectRef<ElectronBridge> bridge, DotNetObjectRef<AlertArguments> arguments, bool isOk)
        {
            try
            {
                bridge.Value.ForwardOnAlertSignalResult(arguments.Value, isOk);
            }
            catch (Exception ex)
            {
                throw;
            }
            return Task.CompletedTask;
        }
    }
}
