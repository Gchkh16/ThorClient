using System;

namespace ThorClient.Core.Model.BlockChain
{
    [Serializable]
    public class Account
    {
        public string Balance { get; set; }
        public string Energy { get; set; }
        public bool HasCode { get; set; }
    }
}
