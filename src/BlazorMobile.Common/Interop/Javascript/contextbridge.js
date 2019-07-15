
window.contextBridge = {

    connectivity: {

        _contextBridgeURI: "%_contextBridgeURI%",
        _contextBridgeIsOpen: false,
        _contextBridgeSocket: null,
        getOrOpenConnection: function (onSuccess, onError) {
            if (window.contextBridge.connectivity._contextBridgeSocket === null) {
                console.log("contextBridge.getOrOpenConnection: trying to open new connection");
                window.contextBridge.connectivity.openWSConnection(window.contextBridge.connectivity._contextBridgeURI, onSuccess, onError);
            }
            else {
                if (window.contextBridge.connectivity._contextBridgeIsOpen === true) {
                    onSuccess(window.contextBridge.connectivity._contextBridgeSocket);
                }
                else {
                    setTimeout(function () {
                        //Second chance interval, if the connection is busy
                        //This should be rare if everything is working
                        if (window.contextBridge.connectivity._contextBridgeIsOpen === true) {
                            onSuccess(window.contextBridge.connectivity._contextBridgeSocket);
                        }
                        else {
                            onError();
                        }
                    }, 100);
                }
            }
        },
        openWSConnection: function (uri, onOpen, onError) {
            var webSocketURL = null;
            webSocketURL = uri;
            console.log("open WS Connection::Connecting to: " + webSocketURL);
            try {
                window.contextBridge.connectivity._contextBridgeSocket = new WebSocket(webSocketURL);
                window.contextBridge.connectivity._contextBridgeSocket.onopen = function (openEvent) {
                    console.log("WebSocket onOpen: " + JSON.stringify(openEvent, null, 4));
                    window.contextBridge.connectivity._contextBridgeIsOpen = true;
                    onOpen(window.contextBridge.connectivity._contextBridgeSocket);
                };
                window.contextBridge.connectivity._contextBridgeSocket.onclose = function (closeEvent) {
                    console.log("WebSocket onClose: " + JSON.stringify(closeEvent, null, 4));

                    if (closeEvent.code == 3001) {
                        console.log("WebSocket onClose: Remote connection closed");
                        //The onError is not returned here, as the initial event was typically called at the time we wanted to launch the remote method
                    } else {
                        //If we fall in this case, that mean that the initial connection failed
                        console.log("WebSocket onClose: Unable to connect to websocket server");
                        onError();
                    }

                    window.contextBridge.connectivity._contextBridgeSocket = null;
                    window.contextBridge.connectivity._contextBridgeIsOpen = false;
                };
                window.contextBridge.connectivity._contextBridgeSocket.onerror = function (errorEvent) {
                    console.log("WebSocket onError: " + JSON.stringify(errorEvent, null, 4));
                };
                window.contextBridge.connectivity._contextBridgeSocket.onmessage = function (messageEvent) {
                    var wsMsg = messageEvent.data;
                    window.contextBridge.receive(wsMsg);
                };
            } catch (exception) {
                console.error(exception);
            }
        }
    },
    metadata: {
        GetBlazorMobileCommonAssemblyName: function () {
            return 'BlazorMobile.Common';
        },
        GetBlazorMobileReceiveMethodName: function () {
            return 'Receive';
        }
    },
    send: function (csharpProxy) {
        //TODO: Manage connexion error in the ContextBridge in order to return a failed Task
        console.log("contextBridge.send called");
        window.contextBridge.connectivity.getOrOpenConnection(
            function (wsSocket) {
                //On success
                console.log("contextBridge.send: sending message");
                wsSocket.send(csharpProxy);

        }, function () {
                //On Error
                console.log("contextBridge.send - error: unable to retrieve a valid socket");
        });
    },
    receive: function (csharpProxy) {
        DotNet.invokeMethod(window.contextBridge.metadata.GetBlazorMobileCommonAssemblyName(),
            window.contextBridge.metadata.GetBlazorMobileReceiveMethodName(), csharpProxy);
    }
};

window.contextBridgeSend = function (data) {
    window.contextBridge.send(data);
    return true;
};