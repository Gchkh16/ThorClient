using System;
using System.Collections.Generic;
using Org.BouncyCastle.Utilities;

namespace ThorClient.Utils.Rlp
{
    using static RlpDecoder;

    public class RlpEncoder
    {
        public static byte[] Encode(RlpType value)
        {
            if (value is RlpString) {
                return EncodeString((RlpString)value);
            } else {
                return EncodeList((RlpList)value);
            }
        }


        private static byte[] Encode(byte[] bytesValue, int offset)
        {
            if (bytesValue.Length == 1
                && offset == OFFSET_SHORT_STRING
                && bytesValue[0] >= (byte)0x00
                && bytesValue[0] <= (byte)0x7f)
            {
                return bytesValue;
            }
            else if (bytesValue.Length < 55)
            {
                var result = new byte[bytesValue.Length + 1];
                result[0] = (byte)(offset + bytesValue.Length);
                Array.Copy(bytesValue, 0, result, 1, bytesValue.Length);
                return result;
            }
            else
            {
                var encodedStringLength = ToMinimalByteArray(bytesValue.Length);
                var result = new byte[bytesValue.Length + encodedStringLength.Length + 1];

                result[0] = (byte)((offset + 0x37) + encodedStringLength.Length);
                Array.Copy(encodedStringLength, 0, result, 1, encodedStringLength.Length);
                Array.Copy(bytesValue, 0, result, encodedStringLength.Length + 1, bytesValue.Length);
                return result;
            }
        }

        private static byte[] EncodeString(RlpString value) => Encode(value.GetBytes(), OFFSET_SHORT_STRING);

        private static byte[] ToMinimalByteArray(int value)
        {
            var encoded = ToByteArray(value);

            for (int i = 0; i < encoded.Length; i++)
            {
                if (encoded[i] != 0)
                {
                    return Arrays.CopyOfRange(encoded, i, encoded.Length);
                }
            }

            return new byte[] { };
        }

        private static byte[] ToByteArray(int value)
        {
            return new byte[] {
                (byte) ((value >> 24) & 0xff),
                (byte) ((value >> 16) & 0xff),
                (byte) ((value >> 8) & 0xff),
                (byte) (value & 0xff)
            };
        }

        private static byte[] EncodeList(RlpList value)
        {
            var values = value.Values;
            if (values.Count == 0)
            {
                return Encode(new byte[] { }, OFFSET_SHORT_LIST);
            }
            else
            {
                var result = new byte[0];
                foreach (var entry in values)
                {
                    result = Concat(result, Encode(entry));
                }
                return Encode(result, OFFSET_SHORT_LIST);
            }
        }

        private static byte[] Concat(byte[] b1, byte[] b2)
        {
            var result = Arrays.CopyOf(b1, b1.Length + b2.Length);
            Array.Copy(b2, 0, result, b1.Length, b2.Length);
            return result;
        }
    }
}
