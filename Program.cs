using System;
using ThorClient.Console;
using ThorClient.Core.Model.BlockChain;
using ThorClient.Utils;
using static System.Console;

namespace ThorClient
{
    public static class Program
    {
        private const string SIGN = "sign";

        private const string CREATE_WALLET = "createWallet";

        private const string SEND = "signAndSend";

        private const string TRANSFER_VET = "transferVet";

        private const string TRANSFER_VTHO = "transferVtho";

        private const string CHAIN_TAG = "getChainTag";

        private const string BLOCK_REF = "getBlockRef";

        private const string GET_BLOCK = "getBlock";

        private const string GET_TRANSACTION = "getTransaction";

        private const string GET_TRANSACTION_RECEIPT = "getTransactionReceipt";

        private const string SEND_RAW = "sendRaw";

        private const string SIGN_VTHO = "signVTHO";

        private const string PARSE = "parse";

        private const string BALANCE = "balance";


        public static void Main(string[] args)
        {

            try
            {
                if (args.Length == 0)
                {
                    WriteLine("找不到有效的参数~");
                    Environment.Exit(0);
                }

                string privateKey = ProcessConsoleArguments(args);
                if (args[0] == GET_TRANSACTION)
                {
                    // args=getTransaction txId providerUrl
                    TransactionConsole.GetTransaction(args);
                }
                else if (args[0] == GET_TRANSACTION_RECEIPT)
                {
                    // args=getTransaction txId providerUrl
                    TransactionConsole.GetTransactionRecipient(args);
                }
                else if (args[0] == SIGN)
                {
                    TransactionConsole.SignVETTxn(args);
                }
                else if (args[0] == SIGN_VTHO)
                {
                    TransactionConsole.SignVTHOTxn(args);
                }
                else if (args[0] == TRANSFER_VET)
                {
                    TransactionConsole.TransferVet(args);
                }
                else if (args[0] == TRANSFER_VTHO)
                {
                    TransactionConsole.TransferVtho(args);
                }
                else if (args[0] == BALANCE)
                {
                    TransactionConsole.GetBalance(args);
                }
                else if (args[0] == CHAIN_TAG)
                {
                    BlockchainQueryConsole.GetChainTag();
                }
                else if (args[0] == GET_BLOCK)
                {
                    BlockchainQueryConsole.getBestBlock(args);
                }
                else if (args[0] == BLOCK_REF)
                {
                    BlockchainQueryConsole.GetBestBlockRef();
                }
                else if (args[0] == CREATE_WALLET)
                {
                    WalletConsole.CreateWalletToKeystoreFile(args);
                }
                else if (args[0] == SEND)
                {
                    // args=signAndSendVET {providerUrl} {privateKey} {filePath}
                    TransactionConsole.SendTransactionFromCSVFile(args, privateKey);
                }
                else if (args[0] == SEND_RAW)
                {
                    // args=sendVETRaw {providerUrl} {rawTransaction}
                    TransactionConsole.SendRawTransaction(args);
                }
                else if (args[0] == PARSE)
                {
                    ParserConsole.Parse(args);
                }
                else
                {
                    WriteLine("不支持的操作命令");
                }
            }
            catch (Exception e)
            {
                WriteLine("操作失败 " + e.Message);
            }
        }

        /// <summary>
        /// Process console input arguments
        /// </summary>
        /// <param name="args">input arguments </param>
        private static string ProcessConsoleArguments(string[] args)
        {
            string privateKey = null;
            string nodeProviderUrl = null;
            if (args[0].Equals(CHAIN_TAG) || args[0].Equals(BLOCK_REF) || args[0].Equals(GET_BLOCK) || args[0].Equals(SEND)
                || args[0].Equals(SEND_RAW) || args[0].Equals(TRANSFER_VET) || args[0].Equals(TRANSFER_VTHO)
                || args[0].Equals(BALANCE))
            {

                if (args.Length > 1 && !string.IsNullOrWhiteSpace(args[1]) && args[1].StartsWith("http"))
                {
                    nodeProviderUrl = args[1];
                }
                if (string.IsNullOrWhiteSpace(nodeProviderUrl))
                {
                    WriteLine("You have input invalid parameters.");
                    Environment.Exit(0);
                }
                if (args.Length > 2 && args[0].Equals(SEND))
                {
                    // args=send {providerUrl} {privateKey}
                    if (!string.IsNullOrWhiteSpace(args[2]))
                    {
                        privateKey = args[2];
                    }
                    if (string.IsNullOrWhiteSpace(privateKey))
                    {
                        WriteLine("You have input invalid parameters.");
                        Environment.Exit(0);
                    }
                }
                NodeProvider nodeProvider = NodeProvider.Instance;
                nodeProvider.Provider = nodeProviderUrl;
                nodeProvider.SocketTimeout = 5000;
            }
            return privateKey;
        }
    }
}
