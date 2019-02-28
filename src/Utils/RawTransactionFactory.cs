using System;
using ThorClient.Core.Model.BlockChain;
using ThorClient.Core.Model.Clients;

namespace ThorClient.Utils
{
    public class RawTransactionFactory
    {
        private static RawTransactionFactory INSTANCE = new RawTransactionFactory();

        public RawTransaction createRawTransaction(byte chainTag, byte[] blockRef, int expiration, int gasInt,
            byte gasPriceCoef, byte[] nonce, params ToClause[] toClauses)
        {
            if (chainTag == 0 || blockRef == null || expiration <= 0 || gasInt< 21000 || gasPriceCoef< 0
            || toClauses == null) {
            throw new ArgumentException("The arguments of create raw transaction is illegal.");
            }
            var builder = new RawTransactionBuilder();

            // chainTag
            builder.Update(chainTag, "chainTag");

            // Expiration
            var expirationBytes = BytesUtils.LongToBytes(expiration);
            builder.Update(expirationBytes, "expiration");

            // BlockRef
            var currentBlockRef = BytesUtils.TrimLeadingZeroes(blockRef);
            builder.Update(currentBlockRef, "blockRef");

            // Nonce
            var trimedNonce = BytesUtils.TrimLeadingZeroes(nonce);
            builder.Update(trimedNonce, "nonce");

            // gas
            var gas = BytesUtils.LongToBytes(gasInt);
            builder.Update(gas, "gas");
            builder.Update(gasPriceCoef, "gasPriceCoef");

            // clause
            int size = toClauses.Length;
            var rawClauses = new RawClause[size];
            int index = 0;
                foreach (var clause in toClauses) {
                    rawClauses[index] = new RawClause
                    {
                        To = clause.To.ToByteArray(),
                        Value = clause.Value.ToByteArray(),
                        Data = clause.Data.ToByteArray()
                    };
                    index++;
            }
                // update the clause
            builder.Update(rawClauses);
            var rawTxn = builder.Build();
                return rawTxn;
        }
    }
}
