using System;
using System.Collections.Generic;

namespace ThorClient.Utils
{
    public static class UrlUtils
    {
        public static string UrlComposite(string urlString, Dictionary<string, string> pathMap, Dictionary<string, string> queryMap)
        {
            string compositeUrl = urlString;
            if (pathMap != null && pathMap.Count > 0)
            {
                foreach (var key in pathMap.Keys)
                {
                    string pathValue = pathMap[key];
                    //String encodedPathValue = URLEncoder.encode( pathValue, "utf-8");
                    compositeUrl = compositeUrl.Replace("{" + key + "}", pathValue);
                }
            }
            string queryComposite = "";
            if (queryMap != null && queryMap.Count > 0)
            {
                int querySize = queryMap.Count;
                int index = 0;
                foreach (string key in queryMap.Keys)
                {


                    string queryValue = queryMap[key];
                    string queryString = key + "=" + queryValue;
                    index++;
                    if (index < querySize)
                    {
                        queryString += "&";
                    }
                    queryComposite += queryString;
                }
            }
            if (!string.IsNullOrWhiteSpace(queryComposite))
            {
                compositeUrl += "?" + queryComposite;
            }
            return new Uri(compositeUrl).AbsoluteUri;
        }
    }
}
