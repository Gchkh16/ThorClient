using System;

namespace ThorClient.Core.Model.BlockChain
{
    [Serializable]
    public class AddressSet
    {
        public string TxtOrigin { get; set; }
        public string Sender { get; set; }
        private string Recipient { get; set; }
    }
}
