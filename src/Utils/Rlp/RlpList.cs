using System.Collections.Generic;
using System.Linq;
using Org.BouncyCastle.Utilities;

namespace ThorClient.Utils.Rlp
{
    public class RlpList : RlpType
    {
        public List<RlpType> Values { get; }

        public RlpList(params RlpType[] values) => Values = values.ToList();

        public RlpList(List<RlpType> values) => Values = values;
    }
}
