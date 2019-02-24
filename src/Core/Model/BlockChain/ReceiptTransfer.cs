using System;
using System.Runtime.Serialization;

namespace ThorClient.Core.Model.BlockChain
{
    [Serializable]
    public class ReceiptTransfer
    {
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string Amount { get; set; }
    }
}
