using System;
using ThorClient.Core.Model.Exception;
using ThorClient.Utils;

namespace ThorClient.Core.Model.Clients
{
    public class Revision
    {
        public static Revision BEST { get; } = new Revision();
        private string _revision;


        public static Revision Create(long blockNumber)
        {
            var revision = new Revision
            {
                _revision = "" + blockNumber
            };
            return revision;
        }

        public static Revision Create(string blockId)
        {
            if (blockId.Equals("best", StringComparison.InvariantCultureIgnoreCase))
            {
                return BEST;
            }

            if (!BlockChainUtils.IsId(blockId))
            {
                throw ClientArgumentException.Exception("create revision from blockId invalid");
            }
            var revision = new Revision
            {
                _revision = blockId
            };
            return revision;
        }

        private Revision() => _revision = "best";

        public override string ToString() => _revision;
    }
}
