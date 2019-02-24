using System;
using System.Collections.Generic;
using ThorClient.Core.Model.Clients;

namespace ThorClient.Core.Model.BlockChain
{
    [Serializable]
    public class Transaction
    {
        public string Id { get; set; } //32 bytes
        public int Size { get; set; }
        public int ChainTag { get; set; }
        public string BlockRef { get; set; } //8 bytes
        public long Expiration { get; set; }
        public List<ToClause> Clauses { get; set; }

        public int GasPriceCoef { get; set; }

        public long Gas { get; set; }

        public string DependsOn { get; set; }

        public string Nonce { get; set; }

        public string Origin { get; set; }

        public string Raw { get; set; }

        public TxMeta Meta { get; set; }
    }
}
