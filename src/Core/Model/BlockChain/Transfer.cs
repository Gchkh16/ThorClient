using System;

namespace ThorClient.Core.Model.BlockChain
{
    [Serializable]
    public class Transfer
    {
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string Amount { get; set; }
    }
}
