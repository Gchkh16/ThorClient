using System;

namespace ThorClient.Core.Model.BlockChain
{
    [Serializable]
    public class Options
    {
        public long Offset { get; set; }
        public long Limit { get; set; }

        public static Options Create(long offset, long limit)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), offset, "offset should be positive number");
            }
            if (limit <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(limit), limit,"limit should be positive number");
            }
            var options = new Options
            {
                Offset = offset,
                Limit = limit
            };
            return options;
        }
    }
}
