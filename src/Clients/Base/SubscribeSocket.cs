using System;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Websocket.Client;

namespace ThorClient.Clients.Base
{
    public class SubscribeSocket : WebsocketClient
    {
        public SubscribeSocket(Uri url, Func<ClientWebSocket> clientFactory = null) : base(url, clientFactory)
        {
            this.MessageReceived.Subscribe(OnReceived);
        }

        public new void Send(string message)
        {
            base.Send(message);
        }

        private static void OnReceived(string message)
        {

        }
    }
}
