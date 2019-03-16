using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using ThorClient.Clients;
using ThorClient.Core.Model.BlockChain;
using ThorClient.Core.Model.Clients;
using ThorClient.Utils;
using static System.Console;

namespace ThorClient.Console
{
    public static class TransactionConsole
    {
        public static void GetTransactionRecipient(string[] args)
        {

            if (args.Length < 3 || string.IsNullOrWhiteSpace(args[2]))
            {
                WriteLine("You have input invalid parameters.");
                Environment.Exit(0);
            }
            string txId = args[1];
            string nodeUrl = args[2];
            if (string.IsNullOrWhiteSpace(nodeUrl) && !nodeUrl.StartsWith("http"))
            {
                WriteLine("You have input invalid parameters.");
                Environment.Exit(0);
            }
            var nodeProvider = NodeProvider.Instance;
            nodeProvider.Provider = nodeUrl;
            nodeProvider.SocketTimeout = nodeProvider.ConnectTimeout = 5000;
            var receipt = TransactionClient.GetTransactionReceipt(txId, null);
            WriteLine("Receipt:" + JsonConvert.SerializeObject(receipt));
        }

        public static void GetTransaction(string[] args)
        {
            if (args.Length < 3 || string.IsNullOrWhiteSpace(args[2]))
            {
                WriteLine("You have input invalid parameters.");
                Environment.Exit(0);
            }
            String txId = args[1];
            String nodeUrl = args[2];
            if (string.IsNullOrWhiteSpace(nodeUrl) && !nodeUrl.StartsWith("http"))
            {
                WriteLine("You have input invalid parameters.");
                Environment.Exit(0);
            }
            var nodeProvider = NodeProvider.Instance;
            nodeProvider.Provider = nodeUrl;
            nodeProvider.SocketTimeout = nodeProvider.ConnectTimeout = 5000;
            var transaction = TransactionClient.GetTransaction(txId, true, null);
            WriteLine("Transaction:" + JsonConvert.SerializeObject(transaction));
        }

        public static void SendRawTransaction(String[] args)
        {
            if (args.Length < 3)
            {
                WriteLine("You have input invalid parameters.");
                Environment.Exit(0);
            }
            String result = ConsoleUtils.SendRawVETTx(args[2]);
            WriteLine("Send Result:");
            WriteLine(result);
        }

        public static void SendTransactionFromCSVFile(String[] args, String privateKey)
        {
            if (args.Length < 4)
            {
                WriteLine("You have input invalid parameters.");
                Environment.Exit(0);
            }

            if (File.Exists(args[3]))
            {
                var transactionList = ConsoleUtils.ReadExcelFile(args[3]);
                String result = ConsoleUtils.DoSignVETTx(transactionList, privateKey, true);
                WriteLine("Send Result:");
                WriteLine(result);
            }
            else
            {
                WriteLine("You have input invalid parameters.");
            }
        }

        public static void SignVETTxn(String[] args)
        {
            String privateKey;// args=sign filePath privateKey
            if (args.Length < 3 || string.IsNullOrWhiteSpace(args[2]))
            {
                WriteLine("You have input invalid parameters.");
                Environment.Exit(0);
            }
            privateKey = args[2];
            if (File.Exists(args[1]))
            {
                var transactionList = ConsoleUtils.ReadExcelFile(args[1]);
                String rawTransaction = ConsoleUtils.DoSignVETTx(transactionList, privateKey, false);
                WriteLine("Raw Transaction:");
                WriteLine(rawTransaction);
            }
            else
            {
                WriteLine("You have input invalid parameters.");
            }
        }

        public static void SignVTHOTxn(String[] args)
        {
            String privateKey;// args=sign filePath privateKey
            if (args.Length < 3 || string.IsNullOrWhiteSpace(args[2]))
            {
                WriteLine("You have input invalid parameters.");
                Environment.Exit(0);
            }
            privateKey = args[2];
            if (File.Exists(args[1]))
            {
                var transactionList = ConsoleUtils.ReadExcelFile(args[1]);
                String rawTransaction = ConsoleUtils.DoSignVTHOTx(transactionList, privateKey, false);
                WriteLine("Raw Transaction:");
                WriteLine(rawTransaction);
            }
            else
            {
                WriteLine("You have input invalid parameters.");
            }
        }

        public static void TransferVet(String[] args)
        {
            String privateKey;
            if (args.Length < 6)
            {
                WriteLine("You have input invalid parameters.");
                Environment.Exit(0);
            }
            privateKey = args[5];

            var transactionList = new List<string[]>();
            var tranfs = new String[4];
            tranfs[0] = args[2];
            tranfs[1] = args[3];
            tranfs[2] = args[4];
            tranfs[3] = null;
            transactionList.Add(tranfs);
            String result = ConsoleUtils.DoSignVETTx(transactionList, privateKey, true);
            WriteLine(result);

        }

        public static void TransferVtho(String[] args)
        {
            String privateKey;
            if (args.Length < 6)
            {
                WriteLine("You have input invalid parameters.");
                Environment.Exit(0);
            }
            privateKey = args[5];

            var transactionList = new List<String[]>();
            var tranfs = new String[4];
            tranfs[0] = args[2];
            tranfs[1] = args[3];
            tranfs[2] = args[4];
            tranfs[3] = null;
            transactionList.Add(tranfs);
            String result = ConsoleUtils.DoSignVTHOTx(transactionList, privateKey, true,
                tranfs.Length > 6 ? (int?)int.Parse(tranfs[6]) : null);
            WriteLine(result);
        }

        public static void GetBalance(String[] args)
        {
            if (args.Length < 3)
            {
                WriteLine("You have input invalid parameters.");
                Environment.Exit(0);
            }
            var address = Address.FromHexString(args[2]);
            var account = AccountClient.GetAccountInfo(address, Revision.BEST);
            WriteLine(JsonConvert.SerializeObject(account));
        }
    }
}
