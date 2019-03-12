using System;
using System.Collections.Generic;
using ThorClient.Clients.Base;
using ThorClient.Core.Model.BlockChain;
using ThorClient.Core.Model.Clients;
using ThorClient.Core.Model.Clients.Base;
using ThorClient.Core.Model.Exception;
using ThorClient.Utils;

namespace ThorClient.Clients
{
    public class TransactionClient : AbstractClient
    {
        public const int ContractGasLimit = 21000;

        public static Transaction GetTransaction(string txId, bool isRaw, Revision revision)
        {
            if (!BlockchainUtils.IsId(txId))
            {
                throw ClientArgumentException.Exception("Tx id is invalid");
            }
            var currRevision = revision ?? Revision.BEST;
            var uriParams = Parameters(new string[] { "id" }, new string[] { txId });
            var queryParams = Parameters(new string[] { "revision", "raw" }, new string[] { currRevision.ToString(), isRaw.ToString() });
            return SendGetRequest<Transaction>(AbstractClient.Path.GetTransactionPath, uriParams, queryParams);
        }

        public static Receipt GetTransactionReceipt(string txId, Revision revision)
        {
            if (!BlockchainUtils.IsId(txId))
            {
                throw ClientArgumentException.Exception("Tx id is invalid");
            }
            var currRevision = revision ?? Revision.BEST;
            var uriParams = Parameters(new string[] { "id" }, new string[] { txId });
            var queryParams = Parameters(new string[] { "revision" }, new string[] { currRevision.ToString() });
            return SendGetRequest<Receipt>(Path.GetTransactionReceipt, uriParams, queryParams);
        }

        public static TransferResult Transfer(RawTransaction transaction)
        {
            if (transaction?.Signature == null)
            {
                throw ClientArgumentException.Exception("Raw transaction is invalid");
            }
            var rawBytes = transaction.Encode();
            if (rawBytes == null)
            {
                throw ClientArgumentException.Exception("Raw transaction is encode error");
            }
            string hexRaw = BytesUtils.ToHexString(rawBytes, Prefix.ZeroLowerX);
            var request = new TransferRequest
            {
                Raw = hexRaw
            };
            return SendPostRequest<TransferResult>(Path.PostTransaction, null, null, request);
        }
    }
}
