namespace ThorClient.Core.Model.BlockChain
{
    public class LogMeta
    {
        public string BlockId { get; set; }
        public int BlockNumber { get; set; }
        public long BlockTimestamp { get; set; }
        public string TxId { get; set; }
        public string TxOrigin { get; set; }
    }
}
