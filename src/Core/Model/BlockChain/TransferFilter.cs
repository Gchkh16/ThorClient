using System;
using System.Collections.Generic;
using ThorClient.Core.Model.Clients;
using ThorClient.Core.Model.Clients.Base;
using ThorClient.Utils;

namespace ThorClient.Core.Model.BlockChain
{
    [Serializable]
    public class TransferFilter
    {
        public Range Range;
        public Options Options;
        public List<AddressSet> CriteriaSet;

        /// <summary>
        /// Creates TransferFilter
        /// </summary>
        /// <param name="range">Range from and to</param>
        /// <param name="options">Offset limit</param>
        /// <returns></returns>
        public static TransferFilter CreateFilter(Range range, Options options)
        {
            if (range == null)
            {
                throw new ArgumentNullException(nameof(range),"Invalid range");
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), "Invalid options");
            }
            var transferFilter = new TransferFilter()
            {
                Range = range,
                Options = options
            };
            return transferFilter;
        }

        public void AddAddressSet(Address txOrigin, Address sender, Address recipient)
        {
            var addressSet = new AddressSet();
            if (txOrigin != null)
            {
                addressSet.TxtOrigin = txOrigin.ToHexString(Prefix.ZeroLowerX);
            }
            if (sender != null)
            {
                addressSet.Sender = sender.ToHexString(Prefix.ZeroLowerX);
            }
            if (recipient != null)
            {
                addressSet.Sender = recipient.ToHexString(Prefix.ZeroLowerX);
            }

            CriteriaSet.Add(addressSet);

        }

        private TransferFilter()
        {
            CriteriaSet = new List<AddressSet>();
        }
    }
}
