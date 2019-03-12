using System;
using System.Text;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Math;
using ThorClient.Numerics;

namespace ThorClient.Utils
{
    /// <summary>
    /// BlockChain utility, include address check, blockId check, amount calculate.
    /// </summary>
    public static class BlockchainUtils
    {
        /// <summary>
        /// Check if block revision is valid
        /// </summary>
        /// <param name="revision"> block revision string</param>
        public static bool IsValidRevision(string revision)
        {

            string blockNumPattern = "^[0-9]\\d*$";

            if (string.IsNullOrWhiteSpace(revision))
            {
                return false;
            }

            if ((StringUtils.IsHex(revision) && revision.Length == 66))
            {
                return true;
            }
            else if (Regex.IsMatch(revision, blockNumPattern))
            {
                return true;
            }
            else if ("best".Equals(revision, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Check if the address hex string is valid
        /// </summary>
        /// <param name="address">address hex string which starts with "0x", "VX" or without prefix string</param>
        public static bool IsAddress(string address)
        {
            return (StringUtils.IsCriticalHex(address) && address.Length == 42);
        }

        /// <summary>
        /// Check if hexId string is valid
        /// </summary>
        /// <param name="hexId">block id or txId</param>
        /// <returns></returns>
        public static bool IsId(string hexId)
        {
            return !string.IsNullOrWhiteSpace(hexId) && StringUtils.IsHex(hexId) && hexId.Length == 66;
        }

        public static bool CheckSumAddress(string address)
        {
            string checkSumAddress = GetChecksumAddress(address);
            if (address == checkSumAddress)
            {
                return true;
            }
            return false;
        }

        public static bool CheckSumAddress(string address, bool isCheck)
        {
            bool rtn = false;
            if (address != null && address.Length == 42 &&  Regex.IsMatch(address.ToLower(), "0x[A-Fa-f0-9]{40}"))
            {
                if (isCheck)
                {
                    string realAddress = address.Substring(2);
                    rtn = CheckSumAddress(realAddress);
                }
                else
                {
                    rtn = true;
                }
            }
            return rtn;
        }

        public static string GetChecksumAddress(string address)
        {

            // remove prefix 0x
            address = BytesUtils.CleanHexPrefix(address);
            address = address.ToLower();

            // do keccak256 once
            var bytes = CryptoUtils.Keccak256(Encoding.UTF8.GetBytes(address));
            var builder = new StringBuilder();
            var hex = BytesUtils.ToHexString(bytes, null);

            var chars = hex.ToCharArray();
            int size = address.Length;

            var raws = address.ToCharArray();

            for (int i = 0; i < size; i++)
            {
                if (ParseInt(chars[i]) >= 8)
                {
                    builder.Append(("" + raws[i]).ToUpper());

                }
                else
                {
                    builder.Append(raws[i]);
                }
            }

            return "0x" + builder;
        }

        private static int ParseInt(char value)
        {
            if (value >= 'a' && value <= 'f')
            {
                return 9 + (value - 'a' + 1);
            }
            else
            {
                return value - '0';
            }
        }

        public static BigDecimal Amount(string hexString, int precision, int scale)
        {
            byte[] balBytes = BytesUtils.ToByteArray(hexString);
            if (balBytes == null)
            {
                return null;
            }
            BigInteger balInteger = BytesUtils.BytesToBigInt(balBytes);
            return BytesUtils.BigIntToBigDecimal(balInteger, precision, scale);
        }

        public static byte[] ByteArrayAmount(BigDecimal value, int precision)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value),"amount is null");
            }
            if (precision < 0)
            {
                throw new ArgumentNullException(nameof(precision),"precision is invalid");
            }
            BigDecimal bigDecimal = value * BigDecimal.Pow(10, precision);
            System.Numerics.BigInteger bigInt = bigDecimal.GetWholePart();
            return BytesUtils.TrimLeadingZeroes(bigInt.ToByteArray());
        }
    }
}
