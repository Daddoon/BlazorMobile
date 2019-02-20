var XamarinCommonAssemblyName = 'Daddoon.Blazor.Xamarin.Common';
var BlazorToXamarinDispatcherNamespace = 'Daddoon.Blazor.Xam.Common.Services';
var BlazorToXamarinDispatcherTypeName = 'BlazorToXamarinDispatcher';
var BlazorToXamarinDispatcherReceiveMethodName = 'Receive';

window.contextBridge = {
    send: function (csharpProxy) {
    },
    receive: function (csharpProxy) {
        console.log("receiver called");
        console.log(csharpProxy);
        DotNet.invokeMethod(XamarinCommonAssemblyName, BlazorToXamarinDispatcherReceiveMethodName, csharpProxy);
    }
};

window.contextBridgeSend = function (data) {
    window.contextBridge.send(data);
    return true;
};