using System;
using ThorClient.Core.Model.Clients.Base;
using ThorClient.src.Core.Model.Clients;

namespace ThorClient.Core.Model.Clients
{
    [Serializable]
    public class ToClause
    {
        public Address To { get; set; } //to address
        public Amount Value { get; set; } //hex form of coin to transferred
        public ToData Data { get; set; }

        public ToClause()
        {

        }

        public ToClause(string to, string value, string data)
        {
            To = to;
            Value = value;
            Data = data;
        }
    }
}
