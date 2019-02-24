using System;
using System.Collections.Generic;
using System.Numerics;
using ThorClient.Core.Model.BlockChain;

namespace ThorClient.Core.Model.Clients
{
    [Serializable]
    public class ContractCallResult
    {
        public string Data { get; set; }
        public List<Event> Events { get; set; }
        public List<Transfer> Transfers { get; set; }

        public BigInteger GasUsed { get; set; }
        public bool Reverted { get; set; }
        public string VmError { get; set; }
    }
}
