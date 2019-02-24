using System;
using System.Collections.Generic;

namespace ThorClient.Core.Model.BlockChain
{
    public class WSRequest
    {
        public Dictionary<string, string> CompositeRequestHashMap()
        {
            var result = new Dictionary<string, string>();
            foreach (var property in GetType().GetProperties())
            {
                result[property.Name] = result[property.GetValue(this).ToString()];
            }

            return result;
        }

    }

}
