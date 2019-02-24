using System.Collections.Generic;

namespace ThorClient.Core.Model.BlockChain
{
    public class EventSubscribingResponse
    {
        public string Address { get; set; }
        public List<string> Topics { get; set; }
        public string Data { get; set; }
        public bool Obsolete { get; set; }
        public LogMeta Meta { get; set; }
    }
}
