using System;
using System.Text;
using Org.BouncyCastle.Math;

namespace ThorClient.Utils
{
    public class BytesUtils
    {
        /// <summary>
        /// Change the bytes array to hex string with or without prefix.
        /// </summary>
        public static string ToHexString(byte[] input, int offset, int length, Prefix prefix)
        {
            var stringBuilder = new StringBuilder();
            if (prefix != null)
            {
                stringBuilder.Append(prefix);
            }

            for (int i = offset; i < offset + length; ++i)
            {
                stringBuilder.Append(input[i].ToString("X2"));
            }

            return stringBuilder.ToString();
        }


        /// <summary>
        /// Change the bytes array to hex string with or without prefix.
        /// </summary>
        public static string ToHexString(byte[] input, Prefix prefix) => ToHexString(input, 0, input.Length, prefix);

        /// <summary>
        /// Convert hex string to byte array
        /// </summary>
        /// <param name="hexString"> hex string with prefix "0x"</param>
        /// <returns>byte array if success, if fail - null/></returns>
        public static byte[] ToByteArray(string hexString)
        {
            if (hexString == null || string.IsNullOrWhiteSpace(hexString))
            {
                return null;
            }
            hexString = CleanHexPrefix(hexString);
            int len = hexString.Length;
            if (len == 0)
            {
                return new byte[] { };
            }

            byte[] data;
            int startIdx;
            if (len % 2 != 0)
            {
                data = new byte[(len / 2) + 1];
                data[0] = (byte)GetHexVal(hexString[0]);
                startIdx = 1;
            }
            else
            {
                data = new byte[len / 2];
                startIdx = 0;
            }

            for (int i = startIdx; i < len; i += 2)
            {
                data[(i + 1) / 2] = (byte)((GetHexVal(hexString[i]) << 4)
                                           + GetHexVal(hexString[i + 1]));
            }
            return data;
        }


        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }


        /// <summary>
        /// Check if  contains hex prefix
        /// </summary>
        public static bool ContainsHexPrefix(string input) => input.StartsWith(Prefix.ZeroLowerX);

        public static string CleanHexPrefix(string input)
        {
            if (ContainsHexPrefix(input))
            {
                return input.Substring(2);
            }
            else
            {
                return input;
            }
        }

        public static byte[] ToBytesPadded(BigInteger bytes, int paddingOffset)
        {
            throw new NotImplementedException();
        }

        public static BigInteger BytesToBigInt(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public static byte[] LongToBytes(int index)
        {
            throw new NotImplementedException();
        }

        public static byte[] TrimLeadingZeroes(byte[] message)
        {
            throw new NotImplementedException();
        }
    }
}
