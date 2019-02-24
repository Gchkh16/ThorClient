using System;

namespace ThorClient.Core.Model.BlockChain
{
    [Serializable]
    public class Receipt
    {
        public long GasUsed { get; set; }
        public string GasPayer { get; set; }
        public string Paid { get; set; } //hex form of defaultDecimalStringToByteArray of paid energy
        public string Reward { get; set; } //hex form of defaultDecimalStringToByteArray of reward
        public bool IsReverted { get; set; } //if it is true, then the transaction was reverted by blockchain network
        public LogMeta Meta { get; set; }
    }
}
