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

            byte[] txRawBytes = CryptoUtils.Blake2b(txRawHash);
            byte[] cerHexBytes = CryptoUtils.Sha256(cert);

            byte[] message = new byte[txRawBytes.Length + cerHexBytes.Length];
            Array.Copy(cerHexBytes, 0, message, 0, cerHexBytes.Length);
            Array.Copy(txRawBytes, 0, message, cerHexBytes.Length, txRawBytes.Length);

            ECKeyPair key = ECKeyPair.Create(BytesUtils.ToByteArray(privateKey));
            ECDSASign.SignatureData signature = ECDSASign.SignMessage(CryptoUtils.Sha256(message), key, false);

            byte[] signBytes = signature.ToByteArray();
            _logger.Info("signature: {} {}", BytesUtils.ToHexString(signBytes, null),
            BytesUtils.CleanHexPrefix(BytesUtils.ToHexString(signature.R, Prefix.ZeroLowerX))
            + BytesUtils.CleanHexPrefix(BytesUtils.ToHexString(signature.S, Prefix.ZeroLowerX)) + "0"
            + signature.V);
            return BytesUtils.ToHexString(signBytes, null);
        }
    }
}
