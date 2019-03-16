using System;
using slf4net;
using ThorClient.Utils.Crypto;

namespace ThorClient.Utils
{
    public static class UsbKeyUtils
    {
        private static ILogger _logger = LoggerFactory.GetLogger(typeof(UsbKeyUtils));

        public static string BuildSignature(byte[] cert, byte[] txRawHash, string privateKey)
        {

            var txRawBytes = CryptoUtils.Blake2b(txRawHash);
            var cerHexBytes = CryptoUtils.Sha256(cert);

            var message = new byte[txRawBytes.Length + cerHexBytes.Length];
            Array.Copy(cerHexBytes, 0, message, 0, cerHexBytes.Length);
            Array.Copy(txRawBytes, 0, message, cerHexBytes.Length, txRawBytes.Length);

            var key = ECKeyPair.Create(ByteUtils.ToByteArray(privateKey));
            var signature = ECDSASign.SignMessage(CryptoUtils.Sha256(message), key, false);

            var signBytes = signature.ToByteArray();
            _logger.Info("signature: {} {}", ByteUtils.ToHexString(signBytes, null),
            ByteUtils.CleanHexPrefix(ByteUtils.ToHexString(signature.R, Prefix.ZeroLowerX))
            + ByteUtils.CleanHexPrefix(ByteUtils.ToHexString(signature.S, Prefix.ZeroLowerX)) + "0"
            + signature.V);
            return ByteUtils.ToHexString(signBytes, null);
        }
    }
}
