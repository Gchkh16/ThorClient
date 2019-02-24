using ThorClient.Core.Model.BlockChain;

namespace ThorClient.Core.Model.Clients
{
    public class BlockSubscribingRequest : WSRequest
    {
        public string Pos { get; set; }
    }
}
