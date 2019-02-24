using System.Collections.Generic;

namespace ThorClient.Core.Model.Clients
{
    public class BlockSubscribingResponse
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
        public string TxsRoot { get; set; } //32 bytes
        public string StateRoot { get; set; } //32 bytes
        public string ReceiptsRoot { get; set; } //32 bytes
        public string Signer { get; set; }
        public bool Obsolete { get; set; }
        public List<string> Transactions { get; set; }
    }
}
