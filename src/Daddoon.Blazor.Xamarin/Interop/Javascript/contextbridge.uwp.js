window.contextBridge.send = function (csharpProxy) {
    console.log("Sending: " + csharpProxy);
    window.external.notify(csharpProxy);
};

window.contextBridge.receive = function () {
    //TODO
};
