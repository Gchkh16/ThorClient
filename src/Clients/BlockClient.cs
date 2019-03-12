using System;
using System.Collections.Generic;
using System.IO;
using Org.BouncyCastle.Utilities;
using ThorClient.Clients.Base;
using ThorClient.Core.Model.BlockChain;
using ThorClient.Core.Model.Clients;

namespace ThorClient.Clients
{
    public class BlockClient : AbstractClient
    {
        public static Block GetBlock(Revision revision)
        {
            var currentRevision = revision;
            if (revision == null)
            {
                currentRevision = Revision.BEST;
            }
            var uriParams = Parameters(new string[] { "revision" }, new string[] { currentRevision.ToString() });
            return SendGetRequest<Block>(Path.GetBlockPath, uriParams, null);
        }
    }   
}
