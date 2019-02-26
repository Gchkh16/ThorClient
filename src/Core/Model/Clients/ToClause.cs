using System;
using ThorClient.Core.Model.Clients.Base;

namespace ThorClient.Core.Model.Clients
{
    [Serializable]
    public class ToClause
    {
        public Address To { get; set; } 
        public Amount Value { get; set; }
        public ToData Data { get; set; }

        public ToClause()
        {

        }

        public ToClause(Address to, Amount value, ToData data)
        {
            To = to;
            Value = value;
            Data = data;
        }
    }
}
