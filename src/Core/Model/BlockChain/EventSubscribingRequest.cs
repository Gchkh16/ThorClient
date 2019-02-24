namespace ThorClient.Core.Model.BlockChain
{
    public class EventSubscribingRequest : WSRequest
    {
        public string Pos { get; set; }
        public string Addr { get; set; }
        public string T0 { get; set; }
        public string T1 { get; set; }
        public string T2 { get; set; }
        public string T3 { get; set; }
        public string T4 { get; set; }
    }
}
