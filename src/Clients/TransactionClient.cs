using System;
using System.Collections.Generic;
using ThorClient.Clients.Base;
using ThorClient.Core.Model.BlockChain;
using ThorClient.Core.Model.Clients;
using ThorClient.Core.Model.Clients.Base;
using ThorClient.Core.Model.Exception;
using ThorClient.Utils;
using ThorClient.Utils.Crypto;

namespace ThorClient.Clients
{
    public class TransactionClient : AbstractClient
    {
        public const int ContractGasLimit = 21000;

        /// <summary>
        /// Get transaction by transaction Id.
        /// </summary>
        /// <param name="txId">
        ///            required transaction id . </param>
        /// <param name="isRaw">
        ///            is response raw transaction. </param>
        /// <param name="revision">
        ///            <seealso cref="Revision"/> revision. </param>
        /// <returns> Transaction <seealso cref="Transaction"/> </returns>
        /// <exception cref="ClientIOException"> </exception>
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

        /// <summary>
        /// Get transaction receipt
        /// </summary>
        /// <param name="txId">
        ///            txId hex string start with "0x" </param>
        /// <param name="revision">
        ///            <seealso cref="Revision"/> </param>
        /// <returns> <seealso cref="Receipt"/> return Receipt . </returns>
        /// <exception cref="ClientIOException"> </exception>
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

        /// <summary>
        /// Transfer amount, raw transaction will be encoded by rlp encoder and convert
        /// to hex string with prefix "0x". Then the hex string will be packaged into
        /// <seealso cref="TransferRequest"/> bean object and serialized to JSON string.
        /// </summary>
        /// <param name="transaction">
        ///            <seealso cref="RawTransaction"/> raw transaction to to send </param>
        /// <returns> <seealso cref="TransferResult"/> </returns>
        /// <exception cref="ClientIOException">
        ///             network error, 5xx http status means request error, 4xx http
        ///             status means no enough gas or balance. </exception>

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
            string hexRaw = ByteUtils.ToHexString(rawBytes, Prefix.ZeroLowerX);
            var request = new TransferRequest
            {
                Raw = hexRaw
            };
            return SendPostRequest<TransferResult>(Path.PostTransaction, null, null, request);
        }

        /// <summary>
        /// Send the transaction hex string. </summary>
        /// <param name="rawTransactionHexString"> hex string of raw transaction. </param>
        /// <returns> <seealso cref="TransferResult"/> </returns>
        /// <exception cref="ClientIOException"> </exception>
        public static TransferResult Transfer(string rawTransactionHexString)
        {

            if (!StringUtils.IsHex(rawTransactionHexString))
            {
                throw ClientArgumentException.Exception("Raw transaction is encode error");
            }
            var request = new TransferRequest
            {
                Raw = rawTransactionHexString
            };
            return SendPostRequest<TransferResult>(Path.PostTransaction, null, null, request);
        }

        /// <summary>
        /// Sign the raw transaction.
        /// </summary>
        /// <param name="rawTransaction">
        ///            <seealso cref="RawTransaction"/> </param>
        /// <returns> <seealso cref="RawTransaction"/> with signature. </returns>
        public static RawTransaction Sign(RawTransaction rawTransaction, ECKeyPair keyPair)
        {
            if (rawTransaction == null)
            {
                throw ClientArgumentException.Exception("raw transaction object is invalid");
            }
            var signature = ECDSASign.SignMessage(rawTransaction.Encode(), keyPair, true);
            var signBytes = signature.ToByteArray();
            rawTransaction.Signature = signBytes;
            return rawTransaction;
        }

        /// <summary>
        /// Sign and transfer the raw transaction.
        /// </summary>
        /// <param name="rawTransaction">
        ///            <seealso cref="RawTransaction"/> raw transaction without signature data </param>
        /// <param name="keyPair">
        ///            <seealso cref="ECKeyPair"/> the key pair for the private key. </param>
        /// <returns> <seealso cref="TransferResult"/> </returns>
        /// <exception cref="ClientIOException"> </exception>
        public static TransferResult SignThenTransfer(RawTransaction rawTransaction, ECKeyPair keyPair)
        {
            var signedRawTxn = Sign(rawTransaction, keyPair);
            return Transfer(signedRawTxn);
        }

        /// <summary>
        /// Build a transaction clause
        /// </summary>
        /// <param name="toAddress">
        ///            <seealso cref="Address"/> destination address. </param>
        /// <param name="amount">
        ///            <seealso cref="Amount"/> amount to transfer. </param>
        /// <param name="data">
        ///            <seealso cref="ToData"/> some comments maybe. </param>
        /// <returns> <seealso cref="ToClause"/> to clause. </returns>
        public static ToClause BuildVETToClause(Address toAddress, Amount amount, ToData data)
        {
            if (toAddress == null)
            {
                throw ClientArgumentException.Exception($"{nameof(toAddress)} is null");
            }
            if (amount == null)
            {
                throw ClientArgumentException.Exception($"{nameof(amount)} is null");
            }
            if (data == null)
            {
                throw ClientArgumentException.Exception($"{nameof(data)} is null");
            }
            return new ToClause(toAddress, amount, data);
        }

        /// <summary>
        /// Build deploying the contract codes. </summary>
        /// <param name="contractHex"> byte array
        /// @return </param>
        public static ToClause BuildDeployClause(string contractHex)
        {
            if (!string.IsNullOrWhiteSpace(contractHex))
            {
                return null;
            }
            ToData toData = new ToData();
            toData.SetData(contractHex);
            return new ToClause(Address.NULL_ADDRESS, Amount.ZERO, toData);
        }

        /// <summary>
        /// Deploy a contract to the block chain. </summary>
        /// <param name="contractHex"> the contract hex string with </param>
        /// <param name="gas">  the gas </param>
        /// <param name="gasCoef">  the gas coefficient </param>
        /// <param name="expiration"> the expiration </param>
        /// <param name="keyPair">  private keypair </param>
        public static TransferResult DeployContract(String contractHex, int gas, byte gasCoef, int expiration, ECKeyPair keyPair)
        {
            ToClause toClause = BuildDeployClause(contractHex);
            if (toClause == null)
            {
                throw ClientArgumentException.Exception("The contract hex string is null");
            }
            ToClause[] toClauses = new ToClause[1];
            toClauses[0] = toClause;
            return InvokeContractMethod(toClauses, gas, gasCoef, expiration, keyPair);
        }

        /// <summary>
        /// InvokeContractMethod send transaction to contract.
        /// </summary>
        protected static TransferResult InvokeContractMethod(ToClause[] toClauses, int gas, byte gasCoef, int expiration, ECKeyPair keyPair)
        {

            if (keyPair == null)
            {
                throw ClientArgumentException.Exception("ECKeyPair is null.");
            }

            if (gas < ContractGasLimit)
            {
                throw ClientArgumentException.Exception("gas is too small.");
            }
            if (gasCoef < 0)
            {
                throw ClientArgumentException.Exception("gas coef is too small.");
            }

            if (expiration <= 0)
            {
                throw ClientArgumentException.Exception("expiration is invalid.");
            }

            if (toClauses == null)
            {
                throw ClientArgumentException.Exception("To clause is null");
            }

            byte chainTag = BlockchainClient.GetChainTag();
            BlockRef bestRef = BlockchainClient.GetBlockRef(null);
            if (bestRef == null || chainTag == 0)
            {
                throw new ClientIOException("Get chainTag: " + chainTag + " BlockRef: " + bestRef);
            }
            RawTransaction rawTransaction = RawTransactionFactory.Instance.CreateRawTransaction(chainTag, bestRef.ToByteArray(), expiration, gas, gasCoef, CryptoUtils.GenerateTxNonce(), toClauses);
            return TransactionClient.SignThenTransfer(rawTransaction, keyPair);
        }

    }
}
