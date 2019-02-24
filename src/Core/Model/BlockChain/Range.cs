using System;

namespace ThorClient.Core.Model.BlockChain
{
    public class Range
    {
        public string Unit { get; set; }
        public long From { get; set; }
        public long To { get; set; }

        public static Range CreateBlockRange(long from, long to)
        {
            if (from >= to)
            {
                throw new ArgumentException("Range from to error.");
            }
            var range = new Range
            {
                From = from,
                To = to,
                Unit = "block"
            };
            return range;
        }

        public static Range CreateTimeRange(long from, long to)
        {
            if (from >= to)
            {
                throw new ArgumentException("Range from to error.");
            }
            var range = new Range
            {
                From = from,
                To = to,
                Unit = "time"
            };
            return range;
        }


    }
}
