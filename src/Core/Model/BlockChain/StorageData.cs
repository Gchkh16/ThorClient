using System;

namespace ThorClient.Core.Model.BlockChain
{
    [Serializable]
    public class StorageData
    {
        public string Value { get; set; }

        public override string ToString() => Value;
    }
}
