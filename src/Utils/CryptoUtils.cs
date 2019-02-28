using System;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using ThorClient.Utils.Crypto;
// ReSharper disable InconsistentNaming

namespace ThorClient.Utils
{
    public static class CryptoUtils
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

        public static byte[] GenerateTxNonce()
        {
            SecureRandom random = new SecureRandom();
            byte[] bytes = new byte[8];
            random.NextBytes(bytes);
            return bytes;
        }

        public static ECKeyPair RecoverPublicKey(byte[] message, byte[] sig)
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
            /**
            * Thor public blockchain is using 256 bits digest
            */
            var blake2b = new Blake2bDigest(256);
            blake2b.BlockUpdate(message, 0, message.Length);
            byte[] digest = new byte[32];
            int size = blake2b.DoFinal(digest, 0);
            if (size > 0)
            {
                return digest;
            }
            else
            {
                return null;
            }
        }

        public static byte[] Sha256(byte[] bytes)
        {
            return Sha256(bytes, 0, bytes.Length);
        }

        private static byte[] Sha256(byte[] bytes, int offset, int size)
        {
            var sha256Digest = new Sha256Digest();
            sha256Digest.BlockUpdate(bytes, offset, size);
            byte[] sha256 = new byte[32];
            sha256Digest.DoFinal(sha256, 0);
            return sha256;
        }
    }
}
