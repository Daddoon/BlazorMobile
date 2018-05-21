window.contextBridge.send = function (csharpProxy) {
    window.external.notify(csharpProxy);
};

window.contextBridge.receive = function () {
    //TODO
};
