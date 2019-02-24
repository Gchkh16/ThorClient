using System;

namespace ThorClient.Core.Model.Clients
{
    [Serializable]
    public class ContractCall
    {
        public string Value;
        public string Data;
        public long Gas;
        public string GasPrice;
        public string Caller;

        public ContractCall()
        {
            Value = "0x0";
            Data = "0x0";
        }


    }
}
