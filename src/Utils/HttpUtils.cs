using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using Flurl.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ThorClient.Core.Model.Exception;

namespace ThorClient.Utils
{
    public static class HttpUtils
    {
        public static T GetJson<T>(string url, Dictionary<string, string> headers)
        {
            return url.WithHeaders(headers).GetJsonAsync<T>().Result;
        }

        public static T PostJson<T>(string url, Dictionary<string, string> headers, object json)
        {
            var resp = url.WithHeaders(headers).PostJsonAsync(json).Result;
            resp.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<T>(resp.Content.ReadAsStringAsync().Result);
        }
    }
}
