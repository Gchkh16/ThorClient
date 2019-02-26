using System;
using ThorClient.Core.Model.Exception;
using ThorClient.Utils;

namespace ThorClient.Core.Model.Clients
{
    public class StorageKey
    {
        private byte[] _data;

        public static StorageKey Create(int index, byte[] value)
        {

            if (index < 0)
            {
                throw new InvalidArgumentException("index is invalid.");
            }
            if (value == null || value.Length > 32)
            {
                throw new InvalidArgumentException("value is invalid.");
            }
            var indexBytes = new byte[32];
            var valueBytes = value;

            var originIndexBytes = BytesUtils.LongToBytes(index);
            Array.Copy(originIndexBytes, 0, indexBytes, indexBytes.Length - originIndexBytes.Length, originIndexBytes.Length);

            if (value.Length < 32)
            {
                valueBytes = new byte[32];
                Array.Copy(value, 0, valueBytes, valueBytes.Length - value.Length, value.Length);
            }
            return new StorageKey(indexBytes, valueBytes);
        }

        private StorageKey(byte[] index, byte[] value)
        {
            _data = new byte[64];
            Array.Copy(index, 0, _data, 0, index.Length);
            Array.Copy(value, 0, _data, index.Length, value.Length);

        }

        public string HexKey()
        {
            var key = CryptoUtils.Keccak256(_data);
            return BytesUtils.ToHexString(key, Prefix.ZeroLowerX);
        }
    }
}
