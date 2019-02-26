using ThorClient.Core.Model.Clients.Base;

namespace ThorClient.Core.Model.Clients
{
    public class ERC20Token : AbstractToken
    {
        public static ERC20Token VTHO { get; } = new ERC20Token("VTHO", Address.VTHO_Address, 18);
        public  Address ContractAddress { get;}

        protected ERC20Token(string name, Address address, int unit) : base(name, unit)
        {
            ContractAddress = address;
        }
    }
}
