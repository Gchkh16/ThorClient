using System;
using System.Text;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using ThorClient.Numerics;

namespace ThorClient.Utils
{
    public class ByteUtils
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

        /// <summary>
        /// Get byte array from <seealso cref="BigInteger"/> object. </summary>
        /// <param name="bigInteger"> <seealso cref="BigInteger"/> object. </param>
        /// <returns> <seealso cref="byte[]"/> value, if failed return null; </returns>
        private static byte[] BigIntToBytes(BigInteger bigInteger)
        {
            if (bigInteger == null)
            {
                return null;
            }
            return TrimLeadingZeroes(bigInteger.ToByteArray());
        }

        /// <summary>
		/// Convert long value to byte array. </summary>
		/// <param name="value"> long value. </param>
		/// <returns> byte array, if failed return null. </returns>
        public static byte[] LongToBytes(long value)
        {
            BigInteger bigInteger = BigInteger.ValueOf(value);
            return BigIntToBytes(bigInteger);
        }

        /// <summary>
        /// Get BigInteger from byte array, the side effect is stripping the leading zeros. </summary>
        /// <param name="bytes"> byte array. </param>
        public static BigInteger BytesToBigInt(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }
            return new BigInteger(1, bytes);
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

        /// <summary>
        /// Trim the leading byte </summary>
        /// <param name="bytes"> the input byte array. </param>
        /// <param name="b"> the byte need to be trimmed. </param>
        public static byte[] TrimLeadingBytes(byte[] bytes, byte b)
        {
            int Offset = 0;
            for (; Offset < bytes.Length - 1; Offset++)
            {
                if (bytes[Offset] != b)
                {
                    break;
                }
            }
            return Arrays.CopyOfRange(bytes, Offset, bytes.Length);
        }

        public static byte[] TrimLeadingZeroes(byte[] bytes) => TrimLeadingBytes(bytes, (byte)0);

        /// <summary>
        /// Convert <seealso cref="BigInteger"/> value to <seealso cref="BigDecimal"/> value. </summary>
        /// <param name="bgInt"> <seealso cref="BigInteger"/> value </param>
        /// <param name="precision">  the precision value for VET and VeThor it is 18, means 10 power 18 </param>
        /// <param name="scale"> the remain digits number of fractional part. </param>
        /// <returns> <seealso cref="BigDecimal"/> value. </returns>
        public static BigDecimal BigIntToBigDecimal(BigInteger bgInt, int precision, int scale)
        {
            if (bgInt == null || precision < 0 || scale < 0)
            {
                return null;
            }

            var integer2 = new System.Numerics.BigInteger(bgInt.ToByteArray());
            if (scale < precision)
            {
                integer2 /= (int)Math.Pow(10, scale);
                precision = precision - scale; 
            }
            var dec = new BigDecimal(integer2);
            var precisionDecimal = BigDecimal.Pow(10, precision);
            var value = BigDecimal.Divide(dec, precisionDecimal);
            return value;
        }

        /// <summary>
        /// Convert a decimal defaultDecimalStringToByteArray to a byte array, with default 18 level precision, means 10 power 18. </summary>
        /// <param name="amountString"> it is a decimal string. e.g. "42.42" </param>
        public static byte[] DefaultDecimalStringToByteArray(string amountString)
        {
            return DecimalStringToByteArray(amountString, 18);
        }

        /// <summary>
        /// Convert a decimal defaultDecimalStringToByteArray to a byte array. </summary>
        /// <param name="amountString"> it is a decimal string. e.g. "42.42" </param>
        /// <param name="precisionLevel"> the precision level, means 10 power 18 precision. </param>
        public static byte[] DecimalStringToByteArray(string amountString, int precisionLevel)
        {
            if (string.IsNullOrWhiteSpace(amountString))
            {
                throw new ArgumentException("amount string is blank.", nameof(amountString));
            }
            if (precisionLevel < 0)
            {
                throw new ArgumentException("precision level is less than 0", nameof(precisionLevel));
            }
            BigDecimal amountDecimal = BigDecimal.Parse(amountString);
            BigDecimal precisionDecimal = BigDecimal.Pow(10 ,precisionLevel);
            BigDecimal realAmount = BigDecimal.Multiply(amountDecimal, precisionDecimal);
            return TrimLeadingZeroes(realAmount.GetWholePart().ToByteArray());
        }


        public static byte[] ToBytesPadded(BigInteger value, int length)
        {
            byte[] result = new byte[length];
            byte[] bytes = value.ToByteArray();
            int bytesLength;
            byte srcOffset;
            if (bytes[0] == 0)
            {
                bytesLength = bytes.Length - 1;
                srcOffset = 1;
            }
            else
            {
                bytesLength = bytes.Length;
                srcOffset = 0;
            }

            if (bytesLength > length)
            {
                throw new Exception("Input is too large to put in byte array of size " + length);
            }
            else
            {
                int destOffset = length - bytesLength;
                Array.Copy(bytes, srcOffset, result, destOffset, bytesLength);
                return result;
            }
        }

    }
}
