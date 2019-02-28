using System;
using System.Collections.Generic;
using ThorClient.Core.Model.BlockChain;
using ThorClient.Core.Model.Clients;

namespace ThorClient.Utils
{
    public class RawTransactionBuilder
    {
        private Dictionary<string, object>   _refValue;
        private RawClause[] _clauses;

        public RawTransactionBuilder()
        {
            _refValue = new Dictionary<string, object>();
        }

        public RawTransactionBuilder Update(byte value, string field)
        {

            _refValue[field] = value;

            return this;
        }

        public RawTransactionBuilder Update(byte[] value, string field)
        {
            _refValue[field] = value;
            return this;
        }

        public RawTransactionBuilder Update(RawClause[] clauses)
        {
            _clauses = clauses;
            return this;
        }


        public RawTransaction Build()
        {

            var rawTransaction = new RawTransaction();
            BeanRefUtils.SetFieldValue(rawTransaction, _refValue);
            rawTransaction.Clauses = _clauses;
            return rawTransaction;
        }
    }
}
