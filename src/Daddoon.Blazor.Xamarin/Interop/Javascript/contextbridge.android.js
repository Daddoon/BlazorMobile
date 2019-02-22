
window.contextBridge.send = function (csharpProxy) {
    blazorxamarinJsBridge.invokeAction(csharpProxy);
};
