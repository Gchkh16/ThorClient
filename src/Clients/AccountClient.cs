using System;
using System.Collections.Generic;
using ThorClient.Clients.Base;
using ThorClient.Core.Model.BlockChain;
using ThorClient.Core.Model.Clients;
using ThorClient.Core.Model.Exception;
using ThorClient.Utils;

namespace ThorClient.Clients
{
    public class AccountClient : AbstractClient
    {
        public static Account GetAccountInfo(Address address, Revision revision)
        {

            if (address == null)
            {
                throw ClientArgumentException.Exception("Address account is null");
            }
            var currRevision = revision ?? Revision.BEST;
            var uriParams = Parameters(new string[] { "address" }, new string[] { address.ToHexString(Prefix.ZeroLowerX) });
            var queryParams = Parameters(new string[] { "revision" }, new string[] { currRevision.ToString() });
            return SendGetRequest<Account>(Path.GetAccountPath, uriParams, queryParams);
        }

        public static ContractCallResult DeployContractInfo(ContractCall contractCall)
        {
            if (contractCall == null)
            {
                throw ClientArgumentException.Exception("contract call object is null");
            }
            return SendPostRequest<ContractCallResult>(Path.PostDeployContractPath, null, null, contractCall);
        }

        public static AccountCode GetAccountCode(Address address, Revision revision)
        {
            if (address == null)
            {
                throw ClientArgumentException.Exception("Address account is null");
            }
            var currRevision = revision ?? Revision.BEST;
            var uriParams = Parameters(new string[] { "address" }, new string[] { address.ToHexString(Prefix.ZeroLowerX) });
            var queryParams = Parameters(new string[] { "revision" }, new string[] { currRevision.ToString() });
            return SendGetRequest<AccountCode>(Path.GetAccountCodePath, uriParams, queryParams);
        }

        public static StorageData GetStorageAt(Address address, StorageKey key, Revision revision)
        {
            if (address == null)
            {
                throw ClientArgumentException.Exception("Address account is null");
            }
            if (key == null)
            {
                throw ClientArgumentException.Exception("key is null");
            }
            var currRevision = revision ?? Revision.BEST;
            var uriParams = Parameters(new string[] { "address", "key" }, new string[] { address.ToHexString(Prefix.ZeroLowerX), key.HexKey() });
            var queryParams = Parameters(new string[] { "revision" }, new string[] { currRevision.ToString() });
            return SendGetRequest<StorageData>(Path.GetStorageValuePath, uriParams, queryParams);
        }
    }
}
