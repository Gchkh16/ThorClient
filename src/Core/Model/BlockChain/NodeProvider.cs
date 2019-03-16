using ThorClient.Clients.Base;
using ThorClient.Utils;

namespace ThorClient.Core.Model.BlockChain
{
    public class NodeProvider
    {
        public static NodeProvider Instance { get; set; } = new NodeProvider();

        public string Provider { get; set; }
        public string WsProvider { get; set; }
        public int SocketTimeout { get; set; }
        public int ConnectTimeout { get; set; }

    }
}
