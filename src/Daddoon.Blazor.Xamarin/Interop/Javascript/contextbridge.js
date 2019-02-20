var XamarinCommonAssemblyName = 'Daddoon.Blazor.Xamarin.Common';
var BlazorToXamarinDispatcherNamespace = 'Daddoon.Blazor.Xam.Common.Services';
var BlazorToXamarinDispatcherTypeName = 'BlazorToXamarinDispatcher';
var BlazorToXamarinDispatcherReceiveMethodName = 'Receive';

window.contextBridge = {
    send: function (csharpProxy) {
        console.log("test log");
        console.log(csharpProxy);
    },
    receive: function (csharpProxy) {
        DotNet.invokeMethodAsync(BlazorToXamarinDispatcherNamespace, BlazorToXamarinDispatcherReceiveMethodName, csharpProxy);
    }
};

window.contextBridgeSend = function (data) {
    window.contextBridge.send(data);
    return true;
};