using System;
using ThorClient.Clients.Base;
using ThorClient.Core.Model.BlockChain;
using ThorClient.Core.Model.Clients;
using ThorClient.Utils;

namespace ThorClient.Clients
{
    public class BlockchainClient : AbstractClient
    {
        public static byte GetChainTag()
        {
            var genesisBlock = BlockClient.GetBlock(Revision.Create(0));
            if (genesisBlock == null)
            {
                throw new Exception("Get Genesis block error");
            }
            string hexId = genesisBlock.Id;
            if (!BlockchainUtils.IsId(hexId))
            {
                throw new Exception("Genesis block id is invalid");
            }
            var bytesId = ByteUtils.ToByteArray(hexId);
            if (bytesId == null || bytesId.Length != 32)
            {
                throw new Exception("Genesis block id converted error");
            }
            return bytesId[31];
        }

        public static BlockRef GetBlockRef(Revision revision)
        {
            var block = BlockClient.GetBlock(revision);
            return block?.BlockRef();
        }
    }
}
