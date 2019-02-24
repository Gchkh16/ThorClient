namespace ThorClient.Core.Model.BlockChain
{
    public class TransferSubscribingRequest : WSRequest
    {
        public string Pos { get; set; }
        public string TxOrigin { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }
    }
}
