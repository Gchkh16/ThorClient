using System;

namespace ThorClient.Core.Model.BlockChain
{
    [Serializable]
    public class TransactionAttributes
    {
        public enum TransactionType{ VET, VTHO }

        public struct ERC20ContractMethod
        {
            public static ERC20ContractMethod BALANCEOF = new ERC20ContractMethod("70a08231");
            public static ERC20ContractMethod TRANSFER = new ERC20ContractMethod("a9059cbb");

            public string Id { get; set; }

            public ERC20ContractMethod(string id)
            {
                Id = id;
            }
        }
    }


}
