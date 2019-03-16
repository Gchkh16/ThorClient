using System;
using System.Collections.Generic;
using ThorClient.Core.Model.Clients;

namespace ThorClient.Core.Model.BlockChain
{
    [Serializable]
    public class Block
    {
        public string Number { get; set; }
        public string Id { get; set; }
        public long Size { get; set; }
        public string ParentId { get; set; }
        public long Timestamp { get; set; }
        public long GasLimit { get; set; }
        public string Beneficiary { get; set; }
        public long GasUsed { get; set; }
        public long TotalScore { get; set; }
        public string TxsRoot { get; set; }//32 bytes
        public string StateRoot { get; set; } //32 bytes
        public string ReceiptsRoot { get; set; } //32 bytes
        public string Signer { get; set; }
        public bool IsTrunk { get; set; }
        public List<string> Transactions { get; set; }

        public override string ToString() => $"number:{Number}  block:{Id} parentId:{ParentId}";

        public BlockRef BlockRef() => Clients.BlockRef.Create(Id);
    }
}
