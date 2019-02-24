using System;
using System.Collections.Generic;

namespace ThorClient.Core.Model.BlockChain
{
    [Serializable]
    public class ReceiptOutput
    {
        public string ContractAddress { get; set; }
        public List<Event> Events { get; set; }
        public List<Transfer> Transfers { get; set; }
    }
}
