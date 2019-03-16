using System;
using ThorClient.Core.Model.BlockChain;
using ThorClient.Core.Model.Clients;
using ThorClient.Core.Model.Clients.Base;
using ThorClient.Utils;
using static System.Console;

namespace ThorClient.Console
{
    public static class ParserConsole
    {
        public static void Parse(string[] args)
        {
            if (args.Length != 3)
            {
                WriteLine("parse [vet|erc20] [raw transaction hex string]");
                return;
            }
            if (args[1].Equals("vet", StringComparison.OrdinalIgnoreCase))
            {
                ParseVET(args[2]);
            }
            else if (args[1].Equals("erc20", StringComparison.OrdinalIgnoreCase) || args[1].Equals("vtho", StringComparison.OrdinalIgnoreCase))
            {
                ParseERC20(args[2]);
            }
            else
            {
                WriteLine("parse [vet|erc20] [raw transaction hex string]");
                throw new Exception("un-support tx type");
            }
        }


        public static void ParseVET(string hexRawTxn)
        {
            var rawTransaction = RLPUtils.Decode(hexRawTxn);
            var rawClauses = rawTransaction.Clauses;
            int index = 1;
            WriteLine("----------------------------------------------------------");
            WriteLine("ChainTag:" + rawTransaction.ChainTag);
            WriteLine("BlockRef:" + ByteUtils.ToHexString(rawTransaction.BlockRef, Prefix.ZeroLowerX));
            WriteLine("Expiration:" + ByteUtils.BytesToBigInt(rawTransaction.Expiration));
            WriteLine("Gas:" + ByteUtils.BytesToBigInt(rawTransaction.Gas).ToString());
            foreach (var rawClause in rawClauses)
            {
                var addressBytes = rawClause.To;
                var valueBytes = rawClause.Value;
                var amount = Amount.VET();
                amount.SetHexAmount(ByteUtils.ToHexString(valueBytes, Prefix.ZeroLowerX));
                WriteLine("No." + index);
                WriteLine("Address:" + ByteUtils.ToHexString(addressBytes, Prefix.ZeroLowerX));
                WriteLine("Value:" + amount.Value);
                WriteLine("-----");
                index++;
            }
            WriteLine("----------------------------------------------------------");
        }

        private static void ParseERC20(string hexRawTxn)
        {
            var rawTransaction = RLPUtils.Decode(hexRawTxn);
            var rawClauses = rawTransaction.Clauses;
            int index = 1;
            WriteLine("----------------------------------------------------------");
            WriteLine("ChainTag:" + rawTransaction.ChainTag);
            WriteLine("BlockRef:" + ByteUtils.ToHexString(rawTransaction.BlockRef, Prefix.ZeroLowerX));
            WriteLine("Expiration:" + ByteUtils.BytesToBigInt(rawTransaction.Expiration));
            WriteLine("Gas:" + ByteUtils.BytesToBigInt(rawTransaction.Gas));
            foreach (var rawClause in rawClauses)
            {
                WriteLine("No." + index);
                var addressBytes = rawClause.To;
                var dataBytes = rawClause.Data;
                if (dataBytes.Length != 68)
                {
                    throw new Exception("The data length is not 68 bytes");
                }
                var methodId = new byte[4];
                var address = new byte[20];
                var value = new byte[32];
                Array.Copy(dataBytes, 0, methodId, 0, 4);
                Array.Copy(dataBytes, 16, address, 0, 20);
                Array.Copy(dataBytes, 36, value, 0, 32);
                string methodIdHex = ByteUtils.ToHexString(methodId, Prefix.ZeroLowerX);
                var contract = new ERC20Contract();
                var abiDefinition = contract.FindAbiDefinition("transfer");
                String transferMethodId = "0x" + abiDefinition.GetHexMethodCodeNoPrefix();
                if (!methodIdHex.Equals(transferMethodId, StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("the method id is not transfer");
                }
                var erc2Amount = Amount.VTHO();
                erc2Amount.SetHexAmount(ByteUtils.ToHexString(value, Prefix.ZeroLowerX));
                WriteLine("ERC20 Contract:" + ByteUtils.ToHexString(addressBytes, Prefix.ZeroLowerX));
                WriteLine("To Address:" + ByteUtils.ToHexString(address, Prefix.ZeroLowerX));
                WriteLine("To Value:" + erc2Amount.Value);
                WriteLine("-----");
                index++;
            }
            WriteLine("----------------------------------------------------------");
        }
    }
}
