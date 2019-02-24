using System.Text.RegularExpressions;

namespace ThorClient.Utils
{
    class StringUtils
    {
        /// <summary>
        /// Remove the prefix "0x" or "VX"
        /// </summary>
        /// <param name="value">full string</param>
        /// <returns>string without prefix</returns>
        public static string SanitizeHex(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
            if (value.ToLower().StartsWith(Prefix.ZeroLowerX))
            {
                return value.Substring(2);
            }
            return value;
        }

        /// <summary>
        /// Check if the string is hex string or not.
        /// </summary>
        /// <param name="value">hex string</param>
        public static bool IsHex(string value)
        {
            return value != null && Regex.IsMatch(value,("^(0x|0X)?[0-9a-fA-F]+$"));
        }

        /// <summary>
        /// Check if the string is critical hex string or not, must start with 0x or OX
        /// </summary>
        /// <param name="value">hex string</param>
        public static bool IsCriticalHex(string value)
        {
            return value != null && Regex.IsMatch(value,"^(0x|0X)[0-9a-fA-F]+$");
        }
    }
}
