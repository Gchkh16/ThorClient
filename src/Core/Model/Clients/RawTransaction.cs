using ThorClient.Core.Model.BlockChain;
using ThorClient.Utils;

namespace ThorClient.Core.Model.Clients
{
    public class RawTransaction
    {
        public byte ChainTag { get; set; }
        public byte[] BlockRef { get; set; } //8 bytes
        public byte[] Expiration { get; set; } //4 bytes
        public RawClause[] Clauses { get; set; }

        // 1-255 used baseprice 255 used 2x base price
        public byte GasPriceCoef { get; set; }

        // gas limit the max gas for VET 21000 for VTHO 80000
        public byte[] Gas { get; set; }//64 bytes
        public byte[] DependsOn { get; set; }
        public byte[] Nonce { get; set; }    //8 bytes
        public byte[] Signature { get; set; }
        public byte[][] Reserved { get;}


        public byte[] Encode()
        {
            return RLPUtils.EncodeRawTransaction(this);
        }

        public RawTransaction copy()
        {
            var transaction = new RawTransaction();
            transaction.Signature = Signature;
            transaction.Clauses = Clauses;
            transaction.BlockRef = BlockRef;
            transaction.DependsOn = DependsOn;
            transaction.ChainTag = ChainTag;
            transaction.Expiration = Expiration;
            transaction.GasPriceCoef = GasPriceCoef;
            transaction.Nonce =  Nonce;
            transaction.Gas = Gas;

            return transaction;
        }
    }
}
