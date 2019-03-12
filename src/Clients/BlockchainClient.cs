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
            var bytesId = BytesUtils.ToByteArray(hexId);
            if (bytesId == null || bytesId.Length != 32)
            {
                throw new Exception("Genesis block id converted error");
            }
            return bytesId[31];
        }
    }
}
