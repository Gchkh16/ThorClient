namespace ThorClient.Core.Model.BlockChain
{
    public class TransferSubscribingResponse
    {
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string Amount { get; set; }
        public LogMeta Meta { get; set; }
        public bool Obsolete { get; set; }
    }
}
