using System;
using System.Collections.Generic;
using ThorClient.Core.Model.BlockChain;
using ThorClient.Core.Model.Clients;
using ThorClient.Utils.Rlp;

namespace ThorClient.Utils
{
    public class ThorClientLogger
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
            var values = asRlpValues(rawTransaction);
            var rlpList = new RlpList(values);
            return RlpEncoder.Encode(rlpList);
        }

        private static List<RlpType> asRlpValues(RawTransaction rawTransaction)
        {
            var result = new List<RlpType>();
            if (rawTransaction.ChainTag == 0)
            {
                throw new ArgumentException("getChainTag is null");
            }
            result.Add(RlpString.Create(rawTransaction.ChainTag));

            if (rawTransaction.BlockRef == null)
            {
                throw new ArgumentException("getBlockRef is null");
            }
            result.Add(RlpString.Create(rawTransaction.BlockRef));

            if (rawTransaction.Expiration == null)
            {
                throw new ArgumentException("getExpiration is null");
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
                throw new ArgumentException("getGas is null");
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
                throw new ArgumentException("getNonce is null");
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
                throw new ArgumentException("reservedList is not supported");
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

        public static RawTransaction Decode(string hexRawTransaction)
        {
            if (!StringUtils.IsHex(hexRawTransaction))
            {
                return null;
            }
            var rawTxBytes = BytesUtils.ToByteArray(hexRawTransaction);
            var list = RlpDecoder.Decode(rawTxBytes);
            if (list == null)
            {
                return null;
            }
            var rlpContent = list.Values;
            //It should only has one element.
            if (rlpContent.Count != 1)
            {
                return null;
            }
            var rawTransaction = new RawTransaction();
            var listValues = ((RlpList)rlpContent[0]).Values;
            for (int index = 0; index < listValues.Count; index++)
            {
                FillTransaction(rawTransaction, listValues, index);
            }
            return rawTransaction;
        }

        private static void FillTransaction(RawTransaction rawTransaction, List<RlpType> listValues, int index)
        {
            RlpString rlpString;
            switch (index)
            {
                case Chain_Tag:
                    rlpString = (RlpString)listValues[index];
                    rawTransaction.ChainTag = rlpString.GetBytes()[0];
                    break;
                case Block_Ref:
                    rlpString = (RlpString)listValues[index];
                    rawTransaction.BlockRef = rlpString.GetBytes();
                    break;
                case Expiration:
                    rlpString = (RlpString)listValues[index];
                    rawTransaction.Expiration = rlpString.GetBytes();
                    break;
                case Clauses:
                    var clauseList = (RlpList)listValues[index];
                    FillClauses(rawTransaction, clauseList);
                    break;
                case GasPriceCoef:
                    rlpString = (RlpString)listValues[index];
                    if (rlpString.GetBytes().Length == 0)
                    {
                        rawTransaction.GasPriceCoef = (byte)0;
                    }
                    else
                    {
                        rawTransaction.GasPriceCoef = rlpString.GetBytes()[0];
                    }

                    break;
                case Gas:
                    rlpString = (RlpString)listValues[index];
                    rawTransaction.Gas = rlpString.GetBytes();
                    break;
                case DependsOn:
                    rlpString = (RlpString)listValues[index];
                    if (rlpString.GetBytes().Length == 0)
                    {
                        rawTransaction.DependsOn = null;
                    }
                    else
                    {
                        rawTransaction.DependsOn = rlpString.GetBytes();
                    }

                    break;
                case Nonce:
                    rlpString = (RlpString)listValues[index];
                    rawTransaction.Nonce = rlpString.GetBytes();
                    break;
                case Reserved:
                    break;
                case Signature:
                    rlpString = (RlpString)listValues[index];
                    rawTransaction.Signature = rlpString.GetBytes();
                    break;
            }
        }

        private static void FillClauses(RawTransaction rawTransaction, RlpList list)
        {
            var clauses = list.Values;
            int clausesSize = clauses.Count;
            var rawClause = new RawClause[clausesSize];
            rawTransaction.Clauses = rawClause;
            for (int clauseIndex = 0; clauseIndex < clausesSize; clauseIndex++)
            {
                var clauseContent = ((RlpList)clauses[clauseIndex]).Values;
                rawClause[clauseIndex] = new RawClause();
                FillOneClause(rawClause, clauseIndex, clauseContent);
            }
        }

        private static void FillOneClause(RawClause[] rawClause, int clauseIndex, List<RlpType> clauseContent)
        {
            for (int index = 0; index < clauseContent.Count; index++)
            {
                var clause = (RlpString)clauseContent[index];
                switch (index)
                {
                    case To:
                        rawClause[clauseIndex].To = clause.GetBytes();
                        break;
                    case Value:
                        rawClause[clauseIndex].Value = clause.GetBytes();
                        break;
                    case Data:
                        rawClause[clauseIndex].Data = clause.GetBytes();
                        break;
                }
            }
        }

    }
}
