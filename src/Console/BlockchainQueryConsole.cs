using System;
using System.Diagnostics;
using Newtonsoft.Json;
using ThorClient.Clients;
using ThorClient.Core.Model.BlockChain;
using ThorClient.Core.Model.Clients;
using ThorClient.Utils;
using static System.Console;

namespace ThorClient.Console
{
    public static class BlockchainQueryConsole
    {
        public static void GetBestBlockRef()
        {
            var blockRefByte = BlockClient.GetBlock(Revision.BEST).BlockRef().ToByteArray();
            string blockRef = ByteUtils.ToHexString(blockRefByte, null);
            WriteLine("BlockRef:");
            WriteLine("0x" + blockRef);
        }

        public static void getBestBlock(string[] args)
        {
            Block block = null;
            if (args != null && args.Length > 2)
            {
                var revision = Revision.Create(long.Parse(args[2]));
                block = BlockClient.GetBlock(revision);
            }
            else
            {
                block = BlockClient.GetBlock(Revision.BEST);
            }
            WriteLine("Block:");
            WriteLine(JsonConvert.SerializeObject(block));
        }

        public static void GetChainTag()
        {
            byte chainTagByte = BlockchainClient.GetChainTag();
            string chainTag = string.Format("%02x", chainTagByte);
            WriteLine("ChainTag:");
            WriteLine("0x" + chainTag);
        }
    }
}
