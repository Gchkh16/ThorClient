using System;

namespace ThorClient.Core.Model.BlockChain
{
    [Serializable]
    public struct AccountCode
    {
        private string _value;

        public AccountCode(string value)
        {
            _value = value;
        }

        public static implicit operator string(AccountCode code)
        {
            return code._value;
        }

        public static implicit operator AccountCode(string code)
        {
            return new AccountCode(){_value = code};
        }
    }
}
