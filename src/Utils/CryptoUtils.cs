using System;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using ThorClient.Utils.Crypto;

namespace ThorClient.Utils
{
    public class CryptoUtils
    {

        public static byte[] Keccak256(byte[] message) => Keccak256(message, 0, message.Length);
        

        /// <summary>
        /// Returns message hash
        /// </summary>
        public static byte[] Keccak256(byte[] message, int offset, int size)
        {
           
            var kecc = new KeccakDigest(256);
            kecc.BlockUpdate(message, offset, size);
            var output = new byte[256];
            kecc.DoFinal(output, 0);

            return output;
        }

        public static byte[] RandomBytes(int byteSize)
        {
            var random = new SecureRandom();
            var randomBytes = new byte[byteSize];
            random.NextBytes(randomBytes);
            return randomBytes;
        }

        public static ECKeyPair recoverPublicKey(byte[] message, byte[] sig)
        {
            if (message == null || message.Length != 32)
            {
                throw new ArgumentException("The recover message is not correct", nameof(message));
            }
            if (sig == null || sig.Length != 65)
            {
                throw new ArgumentException("The recover signature is not correct", nameof(message));
            }
            var rBytes = new byte[32];
            var sBytes = new byte[32];
            Array.Copy(sig, 0, rBytes, 0, rBytes.Length);
            Array.Copy(sig, 32, sBytes, 0, sBytes.Length);
            byte recovery = sig[64];
            var ecdsaSignature = new ECDSASignature(rBytes, sBytes);
            var publicKey = ECDSASign.RecoverFromSignature(recovery, ecdsaSignature, message);
            return new ECKeyPair(null, publicKey);
        }

        public static byte[] Blake2b(byte[] message)
        {
            throw new NotImplementedException();
        }
    }
}
