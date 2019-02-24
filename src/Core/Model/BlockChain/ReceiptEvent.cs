using System;
using System.Collections.Generic;

namespace ThorClient.Core.Model.BlockChain
{
    [Serializable]
    public class ReceiptEvent
    {
        public string Address { get; set; }
        public List<string> Topics { get; set; }
        public string Data { get; set; }
    }
}
