window.contextBridge = {
    getNewToken: function () {
        window.contextBridge.currentToken++;
        return window.contextBridge.currentToken.currentToken;
    },
    currentToken: 0,
    send: function () {
    },
    receive: function () {
    }
};

Blazor.registerFunction('contextBridgeSend', (data) => {
    var token = window.contextBridge.getNewToken();
    window.contextBridge.send(data);

    //TODO: Register a generic Task for this call and return it
    return token;
});