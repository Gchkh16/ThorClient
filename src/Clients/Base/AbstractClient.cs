using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using Flurl.Http;
using Newtonsoft.Json;
using slf4net;
using ThorClient.Core.Model.BlockChain;
using ThorClient.Core.Model.Clients;
using ThorClient.Core.Model.Exception;
using ThorClient.Utils;

namespace ThorClient.Clients.Base
{
    public class AbstractClient
    {
        private static ILogger _logger = LoggerFactory.GetLogger(typeof(AbstractClient));

        public struct Path
        {
            public static Path GetAccountPath = new Path("accounts/{address}");
            public static Path PostContractCallPath = new Path("/accounts/{address}");
            public static Path PostDeployContractPath = new Path("/accounts");
            public static Path GetAccountCodePath = new Path("/accounts/{address}/code");
            public static Path GetStorageValuePath = new Path("/accounts/{address}/storage/{key}");
            public static Path GetTransactionPath = new Path("/transactions/{id}");
            public static Path GetTransactionReceipt = new Path("/transactions/{id}/receipt");
            public static Path PostTransaction = new Path("/transactions");
            public static Path GetBlockPath = new Path("/blocks/{revision}");
            public static Path PostFilterEventsLogPath = new Path("/logs/event");
            public static Path PostFilterTransferLogPath = new Path("/logs/transfer");
            public static Path GetNodeInfoPath = new Path("/node/network/peers");
            public static Path GetSubBlockPath = new Path("/subscriptions/block");
            public static Path GetSubEventPath = new Path("/subscriptions/event");
            public static Path GetSubTransferPath = new Path("/subscriptions/transfer");

            public string Value { get; set; }

            private Path(string value)
            {
                Value = value;
            }
        }

        private static string RawUrl(Path path)
        {
            return NodeProvider.Instance.Provider + path.Value;
        }

        protected HttpClient client = new HttpClient();

        public static T SendGetRequest<T>(Path path, Dictionary<string, string> uriParams,
            Dictionary<string, string> queryParams)
        {
            string rawURL = RawUrl(path);
            string getURL = UrlUtils.UrlComposite(rawURL, uriParams, queryParams);
            try
            {
                return getURL.WithTimeout(5).GetJsonAsync<T>().Result;
            }
            catch (Exception e)
            {
                throw new ClientIOException(e);
            }
        }

        public static T SendPostRequest<T>(Path path, Dictionary<string, string> uriParams,
            Dictionary<string, string> queryParams, object postBody)
        {
            string rawURL = RawUrl(path);
            string postURL = UrlUtils.UrlComposite(rawURL, uriParams, queryParams);

            string resp;
            try
            {
                resp = postURL.WithTimeout(5).PostJsonAsync(postBody).Result.Content.ReadAsStringAsync().Result;
            }
            catch (Exception e)
            {
                throw new ClientIOException(e);
            }



            return JsonConvert.DeserializeObject<T>(resp);
        }

        protected static Dictionary<string, string> Parameters(string[] keys, string[] values)
        {
            if (keys == null || values == null || keys.Length != values.Length)
            {
                throw ClientArgumentException.Exception("Parameters creating failed");
            }

            var parameters = new Dictionary<string, string>();
            for (int index = 0; index < keys.Length; index++)
            {
                parameters[keys[index]] = values[index];
            }
            return parameters;
        }

        public static ContractCallResult CallContract(ContractCall call, Address contractAddress, Revision revision)
        {
            var currentRevision = revision ?? Revision.BEST;

            var uriParams = Parameters(new string[] { "address" },
                new string[] { contractAddress.ToHexString(Prefix.ZeroLowerX) });
            var queryParams = Parameters(new string[] { "revision" },
                    new string[] { currentRevision.ToString() });

            return SendPostRequest<ContractCallResult>(Path.PostContractCallPath, uriParams, queryParams, call);
        }
    }
}
