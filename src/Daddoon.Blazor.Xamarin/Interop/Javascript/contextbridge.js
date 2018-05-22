
const XamarinCommonAssemblyName = 'Daddoon.Blazor.Xamarin.Common';
const BlazorToXamarinDispatcherNamespace = 'Daddoon.Blazor.Xamarin.Common';
const BlazorToXamarinDispatcherTypeName = 'BlazorToXamarinDispatcher';
const BlazorToXamarinDispatcherReceiveMethodName = 'Receive';

var BlazorToXamarinDispatcherReceiveMethodInfo = null;

function ResolveBlazorToXamarinReceiver() {

    if (BlazorToXamarinDispatcherReceiveMethodInfo == null || BlazorToXamarinDispatcherReceiveMethodInfo == undefined) {
        BlazorToXamarinDispatcherReceiveMethodInfo = Blazor.platform.findMethod(
            XamarinCommonAssemblyName,
            BlazorToXamarinDispatcherNamespace,
            BlazorToXamarinDispatcherTypeName,
            BlazorToXamarinDispatcherReceiveMethodName);
    }

    return BlazorToXamarinDispatcherReceiveMethodInfo;
}

window.contextBridge = {
    send: function (csharpProxy) {
    },
    receive: function (csharpProxy) {
        var receiver = ResolveBlazorToXamarinReceiver();
        if (receiver == null || receiver == undefined)
            return;

        let jsonDotNet = Blazor.platform.toDotNetString(csharpProxy);

        Blazor.platform.callMethod(receiver, null, [
            jsonDotNet
        ]);
    }
};

Blazor.registerFunction('contextBridgeSend', (data) => {
    window.contextBridge.send(data);
});