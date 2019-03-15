window.contextBridge.send = function (csharpProxy) {
    window.external.notify(csharpProxy);
};
