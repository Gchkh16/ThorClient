using ThorClient.Core.Model.Exception;
using ThorClient.Utils;

// ReSharper disable InconsistentNaming

namespace ThorClient.Core.Model.Clients
{
    /// <summary>
    ///  Address object is wrapped address string or byte array.
    /// </summary>
    public class Address
    {
        private const int ADDRESS_SIZE = 20;

        public static Address NULL_ADDRESS { get;} = new NullAddress();
        public static Address VTHO_Address { get;} = FromHexString("0x0000000000000000000000000000456e65726779");

        private string _sanitizeHexAddress;

        public static Address FromBytes(byte[] addressBytes)
        { 
            if (addressBytes != null && addressBytes.Length == ADDRESS_SIZE)
            {
                return new Address(addressBytes);
            }
            else
            {
                throw ClientArgumentException.Exception("Address.fromBytes Argument Exception");
            }
        }

        public static Address FromHexString(string hexAddress)
        {
            if (string.IsNullOrWhiteSpace(hexAddress))
            {
                throw ClientArgumentException.Exception("Address.fromHexString hexAddress is blank string");
            }
            if (!BlockchainUtils.IsAddress(hexAddress))
            {
                throw ClientArgumentException.Exception("Address.fromHexString hexAddress is not hex format ");
            }
            string sanitizeHexStr = StringUtils.SanitizeHex(hexAddress);
            return new Address(sanitizeHexStr);
        }

        private Address(string sanitizeHexAddress) => _sanitizeHexAddress = sanitizeHexAddress;

        private Address(byte[] addressBytes) => _sanitizeHexAddress = BytesUtils.ToHexString(addressBytes, null);

        private Address() {}

        public byte[] ToByteArray() => BytesUtils.ToByteArray(_sanitizeHexAddress);

        public string ToHexString(Prefix prefix)
        {
            if (prefix != null)
            {
                return prefix + _sanitizeHexAddress;
            }
            else
            {
                return _sanitizeHexAddress;
            }
        }

        private class NullAddress :Address
        {
            public NullAddress()
            {
                _sanitizeHexAddress = "";
            }
        }
    }
}
