using System;
using System.Collections.Generic;

namespace ThorClient.Core.Model.BlockChain
{
    [Serializable]
    public class Event
    {
        public string Address;
        public List<string> Topics;
        public string Data;
    }
}
