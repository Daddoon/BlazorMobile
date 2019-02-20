
window.contextBridge.send = function (csharpProxy) {
    console.log("debug from JS Android");
    blazorxamarinJsBridge.invokeAction(csharpProxy);
};
