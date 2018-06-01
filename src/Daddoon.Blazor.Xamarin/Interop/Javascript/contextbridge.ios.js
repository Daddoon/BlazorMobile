
window.contextBridge.send = function (csharpProxy) {
    window.webkit.messageHandlers.invokeAction.postMessage(csharpProxy);
};