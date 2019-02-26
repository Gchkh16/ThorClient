using System;
using System.Collections.Generic;
using ThorClient.Core.Model.BlockChain;
using ThorClient.Core.Model.Clients;
using ThorClient.Core.Model.Exception;
using ThorClient.Utils.Rlp;

namespace ThorClient.Utils
{
    public static class RLPUtils
    {
        private const int Chain_Tag = 0;
        private const int Block_Ref = 1;
        private const int Expiration = 2;
        private const int Clauses = 3;
        private const int GasPriceCoef = 4;
        private const int Gas = 5;
        private const int DependsOn = 6;
        private const int Nonce = 7;
        private const int Reserved = 8;
        private const int Signature = 9;

        private const int To = 0;
        private const int Value = 1;
        private const int Data = 2;


        public static byte[] EncodeRawTransaction(RawTransaction rawTransaction)
        {
            var values = AsRlpValues(rawTransaction);
            var rlpList = new RlpList(values);
            return RlpEncoder.Encode(rlpList);
        }

        private static List<RlpType> AsRlpValues(RawTransaction rawTransaction)
        {
            var result = new List<RlpType>();
            if (rawTransaction.ChainTag == 0)
            {
                throw new InvalidArgumentException("getChainTag is null");
            }
            result.Add(RlpString.Create(rawTransaction.ChainTag));

            if (rawTransaction.BlockRef == null)
            {
                throw new InvalidArgumentException("getBlockRef is null");
            }
            result.Add(RlpString.Create(rawTransaction.BlockRef));

            if (rawTransaction.Expiration == null)
            {
                throw new InvalidArgumentException("getExpiration is null");
            }
            result.Add(RlpString.Create(rawTransaction.Expiration));

            var clauses = BuildRlpClausesLIst(rawTransaction);
            var rlpList = new RlpList(clauses);
            result.Add(rlpList);

            if (rawTransaction.GasPriceCoef == 0)
            {
                result.Add(RlpString.Create(RlpString.EMPTY));
            }
            else
            {
                result.Add(RlpString.Create(rawTransaction.GasPriceCoef));
            }

            if (rawTransaction.Gas == null)
            {
                throw new InvalidArgumentException("getGas is null");
            }
            result.Add(RlpString.Create(rawTransaction.Gas));

            if (rawTransaction.DependsOn == null)
            {
                result.Add(RlpString.Create(RlpString.EMPTY));
            }
            else
            {
                result.Add(RlpString.Create(rawTransaction.DependsOn));
            }

            if (rawTransaction.Nonce == null)
            {
                throw new InvalidArgumentException("getNonce is null");
            }
            result.Add(RlpString.Create(rawTransaction.Nonce));

            if (rawTransaction.Reserved == null)
            {
                var reservedRlp = new List<RlpType>();
                var reservedList = new RlpList(reservedRlp);
                result.Add(reservedList);
            }
            else
            {
                throw new InvalidArgumentException("reservedList is not supported");
            }

            if (rawTransaction.Signature != null)
            {
                result.Add(RlpString.Create(rawTransaction.Signature));
            }
            return result;

        }

        private static List<RlpType> BuildRlpClausesLIst(RawTransaction rawTransaction)
        {
            var clauses = new List<RlpType>();

            foreach (var clause in rawTransaction.Clauses)
            {

                var rlpClause = new List<RlpType>();
                rlpClause.Add(clause.To == null ? RlpString.Create(RlpString.EMPTY) : RlpString.Create(clause.To));


                rlpClause.Add(clause.Value == null ? RlpString.Create(RlpString.EMPTY) : RlpString.Create(clause.Value));

                rlpClause.Add(clause.Data == null ? RlpString.Create(RlpString.EMPTY) : RlpString.Create(clause.Data));
                var clauseRLP = new RlpList(rlpClause);
                clauses.Add(clauseRLP);
            }
            return clauses;
        }
    }
}
