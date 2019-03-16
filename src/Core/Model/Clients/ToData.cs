using System;
using ThorClient.Core.Model.Exception;
using ThorClient.Utils;

namespace ThorClient.Core.Model.Clients
{
    public class ToData
    {
        public static ToData ZERO = new Zero();
        private string _hexString;


        public virtual void SetData(string hexString)
        {
            if (!StringUtils.IsHex(hexString))
            {
                throw new InvalidArgumentException("hex string is not valid");
            }
            string noPrefixHex = StringUtils.SanitizeHex(hexString);
            if (noPrefixHex.Length <= 0)
            {
                throw new InvalidArgumentException("hex string is not valid");
            }
            _hexString = hexString;
        }

        public virtual byte[] ToByteArray()
        {

            return ByteUtils.ToByteArray(_hexString);
        }


        private class Zero : ToData
        {
            public override byte[] ToByteArray()
            {
                return new byte[] { };
            }

            public override void SetData(string hexString)
            {
                throw new System.Exception("Not allowed to call");
            }
        }
    }
}
