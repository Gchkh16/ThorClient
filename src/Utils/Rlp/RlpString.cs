using System;
using System.Text;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace ThorClient.Utils.Rlp
{
    public class RlpString : RlpType
    {
        public static byte[] EMPTY { get; } = new byte[] { };

        private byte[] Value { get; }

        private RlpString(byte[] value) => Value = value;

        public byte[] GetBytes() => Value;

        public BigInteger AsBigInteger()
        {
            if (Value.Length == 0)
            {
                return BigInteger.Zero;
            }
            return new BigInteger(Value);
        }

        public string AsString()
        {
            return BytesUtils.ToHexString(Value, Prefix.ZeroLowerX);
        }

        public static RlpString Create(byte[] value) => new RlpString(value);

        public static RlpString Create(byte value) => new RlpString(new byte[] { value });

        public static RlpString Create(BigInteger value)
        {
            // RLP encoding only supports positive integer values
            if (value.SignValue < 1)
            {
                return new RlpString(EMPTY);
            }
            else
            {
                var bytes = value.ToByteArray();
                if (bytes[0] == 0)
                {  // remove leading zero
                    return new RlpString(Arrays.CopyOfRange(bytes, 1, bytes.Length));
                }
                else
                {
                    return new RlpString(bytes);
                }
            }
        }

        public static RlpString Create(long value) => Create(BigInteger.ValueOf(value));

        public static RlpString Create(string value)
        {
            return new RlpString(Encoding.UTF8.GetBytes(value));
        }

        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || GetType() != o.GetType())
            {
                return false;
            }

            var rlpString = (RlpString)o;
            return Arrays.Equals(Value, rlpString.Value);
        }

        public override int GetHashCode()
        {
            return Arrays.GetHashCode(Value);
        }
    }
}
