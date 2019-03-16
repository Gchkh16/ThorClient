using ThorClient.Core.Model.Clients;
using ThorClient.Core.Model.Clients.Base;

namespace ThorClient.Clients
{
    public class ERC20ContractClient : TransactionClient
    {
        public static Amount GetERC20Balance(Address address, ERC20Token token, Revision revision)
        {
            var contractAddr = token.ContractAddress;
            var currRevision = revision ?? Revision.BEST;
            var abiDefinition = ERC20Contract.DefaultERC20Contract.FindAbiDefinition("balanceOf");
            var call = AbstractContract.BuildCall(abiDefinition, address.ToHexString(null));
            var contractCallResult = CallContract(call, contractAddr, currRevision);
            return contractCallResult?.GetBalance(token);
        }
    }
}
