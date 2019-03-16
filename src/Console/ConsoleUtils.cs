using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using ThorClient.Clients;
using ThorClient.Core.Model.BlockChain;
using ThorClient.Core.Model.Clients;
using ThorClient.Core.Model.Clients.Base;
using ThorClient.Utils;
using ThorClient.Utils.Crypto;

namespace ThorClient.Console
{
    public static class ConsoleUtils
    {
        public static string DoSignVETTx(List<string[]> transactions, string privateKey, bool isSend)
        {


            byte chainTag = 0;
            byte[] blockRef = null;

            var clauses = new List<ToClause>();
            foreach (var transaction in transactions)
            {
                var amount = Amount.CreateFromToken(AbstractToken.VET);
                amount.SetDecimalAmount(transaction[1]);
                clauses.Add(TransactionClient.BuildVETToClause(Address.FromHexString(transaction[0]), amount, ToData.ZERO));
                chainTag = ByteUtils.ToByteArray(transaction[2])[0];
                if (transaction[3] == null)
                {
                    blockRef = BlockchainClient.GetBlockRef(null).ToByteArray();
                }
                else
                {
                    blockRef = ByteUtils.ToByteArray(transaction[3]);
                }
            }
            int gas = clauses.Count * 21000;
            var rawTransaction = RawTransactionFactory.Instance.CreateRawTransaction(chainTag, blockRef,
                720, gas, (byte)0x0, CryptoUtils.GenerateTxNonce(), clauses.ToArray());
            if (isSend)
            {
                var result = TransactionClient.SignThenTransfer(rawTransaction, ECKeyPair.Create(privateKey));
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                var result = TransactionClient.Sign(rawTransaction, ECKeyPair.Create(privateKey));
                return ByteUtils.ToHexString(result.Encode(), Prefix.ZeroLowerX);
            }
        }

        public static string SendRawVETTx(string rawTransaction)
        {
            var result = TransactionClient.Transfer(rawTransaction);
            return JsonConvert.SerializeObject(result);
        }

        public static List<string[]> ReadExcelFile(string fiePath)
        {

            var results = new List<string[]>();

            using (var workbook = new XLWorkbook(fiePath))
            {

                var sheet = workbook.Worksheet(1);
                //DataFormatter dataFormatter = new DataFormatter();
                foreach (var row in sheet.RowsUsed().Skip(1))
                {
                    int length = row.CellsUsed().Count();
                    var rowData = new List<string>(length);
                    rowData.AddRange(row.CellsUsed().Select(cell => cell.GetValue<string>()).Where(string.IsNullOrWhiteSpace));
                    if (rowData.Count > 0)
                    {
                        results.Add(rowData.ToArray());
                    }
                }
            }
            return results;
        }


        public static string DoSignVTHOTx(List<string[]> transactions, string privateKey, bool isSend)
        {
            return DoSignVTHOTx(transactions, privateKey, isSend, null);
        }


        public static string DoSignVTHOTx(List<string[]> transactions, string privateKey, bool isSend, int? gasLimit)
        {


            byte chainTag = 0;
            byte[] blockRef = null;

            var clauses = new List<ToClause>();
            foreach (var transaction in transactions)
            {
                var amount = Amount.VTHO();
                amount.SetDecimalAmount(transaction[1]);
                clauses.Add(
                    ERC20Contract.BuildTranferToClause(ERC20Token.VTHO, Address.FromHexString(transaction[0]), amount));
                chainTag = ByteUtils.ToByteArray(transaction[2])[0];
                blockRef = transaction[3] == null ? BlockchainClient.GetBlockRef(null).ToByteArray() : ByteUtils.ToByteArray(transaction[3]);
            }
            int gas = clauses.Count * gasLimit?? 80000;
            var rawTransaction = RawTransactionFactory.Instance.CreateRawTransaction(chainTag, blockRef,
            720, gas, (byte)0x0, CryptoUtils.GenerateTxNonce(), clauses.ToArray());
            if (isSend)
            {
                var result = TransactionClient.SignThenTransfer(rawTransaction, ECKeyPair.Create(privateKey));
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                var result = TransactionClient.Sign(rawTransaction, ECKeyPair.Create(privateKey));
                return ByteUtils.ToHexString(result.Encode(), Prefix.ZeroLowerX);
            }
        }
    }
}
