using Daddoon.Blazor.Xam.Common;
using Daddoon.Blazor.Xam.Common.Interop;
using Daddoon.Blazor.Xam.Interop;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;
using Unosquare.Swan;

namespace Daddoon.Blazor.Xam.Controller
{
    public class BlazorContextBridge : WebSocketsServer
    {
        public BlazorContextBridge()
       : base(true)
        {
        }

        public override string ServerName => nameof(BlazorContextBridge);

        public void SendMessageToClient(string json)
        {
            foreach (var ws in WebSockets)
            {
                Send(ws, json);
            }
        }

        protected override void OnClientConnected(IWebSocketContext context, IPEndPoint localEndPoint, IPEndPoint remoteEndPoint)
        {
        }

        protected override void OnClientDisconnected(IWebSocketContext context)
        {
        }

        protected override void OnFrameReceived(IWebSocketContext context, byte[] buffer, IWebSocketReceiveResult result)
        {
        }

        protected override void OnMessageReceived(IWebSocketContext context, byte[] buffer, IWebSocketReceiveResult result)
        {
            //TODO: Considering to send data from client side as binary Streamed JSON for performance in the future !
            //Value type reference as byte[] and/or string are not good for performance
            string methodProxyJson = buffer.ToText();

            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                MethodProxy taksInput = null;
                MethodProxy taksOutput = null;

                try
                {
                    taksInput = ContextBridge.GetMethodProxyFromJSON(methodProxyJson);
                    taksOutput = ContextBridge.Receive(taksInput);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: [Native] - BlazorContextBridge.Receive: " + ex.Message);
                }

                try
                {
                    string jsonReturnValue = ContextBridge.GetJSONReturnValue(taksOutput);
                    SendMessageToClient(jsonReturnValue);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: [Native] - BlazorContextBridge.Send: " + ex.Message);
                }
            });
        }
    }
}
