using System;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace ThorClient.Utils.Crypto
{
    /// <summary>
    /// ECDSA Keypair contains private key and public key.
    /// </summary>
    public class ECKeyPair
    {
        public const int PRIVATE_KEY_SIZE = 32;
        private const int PUBLIC_KEY_SIZE = 64;
        private static X9ECParameters CURVE_PARAMS { get; } = CustomNamedCurves.GetByName("secp256k1");
        public static BigInteger HALF_CURVE_ORDER { get; } = CURVE_PARAMS.N.ShiftRight(1);
        public static ECDomainParameters CURVE { get; } = new ECDomainParameters(CURVE_PARAMS.Curve, CURVE_PARAMS.G, CURVE_PARAMS.N, CURVE_PARAMS.H);

        private static SecureRandom secureRandom { get; } = new SecureRandom();
        private static ECDomainParameters domain = new ECDomainParameters(CURVE.Curve,
        CURVE.G, CURVE.N, CURVE.H);

        public BigInteger PrivateKey { get; }
        public BigInteger PublicKey { get; }


        public static ECKeyPair Create(BigInteger privateKey) => new ECKeyPair(privateKey, ECDSASign.PublicKeyFromPrivate(privateKey));

        public static ECKeyPair Create(string privateKeyHex)
        {
            var privKey = BytesUtils.ToByteArray(privateKeyHex);
            return Create(privKey);
        }

        public static ECKeyPair Create(byte[] privateKey)
        {
            if (privateKey.Length == PRIVATE_KEY_SIZE)
            {
                return Create(BytesUtils.BytesToBigInt(privateKey));
            }
            else
            {
                throw new ArgumentException("Invalid privatekey size", nameof(privateKey));
            }
        }

        public static ECKeyPair Create()
        {
            var generator = new ECKeyPairGenerator();
            var keygenParams = new ECKeyGenerationParameters(domain,
                secureRandom);
            generator.Init(keygenParams);
            var keypair = generator.GenerateKeyPair();
            var privParams = (ECPrivateKeyParameters)keypair.Private;
            var k = ECKeyPair.Create(privParams.D);
            return k;
        }

        public ECKeyPair(BigInteger privateKey, BigInteger publicKey)
        {
            this.PrivateKey = privateKey;
            this.PublicKey = publicKey;
        }

        public byte[] GetRawPrivateKey() => BytesUtils.ToBytesPadded(PrivateKey, PRIVATE_KEY_SIZE);

        public byte[] GetRawPublicKey() => BytesUtils.ToBytesPadded(PublicKey, PUBLIC_KEY_SIZE);

        public byte[] GetRawAddress()
        {
            var hash = CryptoUtils.Keccak256(this.GetRawPublicKey());
            var address = new byte[20];
            Array.Copy(hash, 12, address, 0, address.Length);
            return address;  // right most 160 bits
        }

        public string GetHexAddress()
        {
            var addressBytes = GetRawAddress();
            return BytesUtils.ToHexString(addressBytes, Prefix.ZeroLowerX);
        }

        /// <summary>
        /// Sign a hash with the private key of this key pair
        /// </summary>
        /// <param name="message">a hash to sign</param>
        /// <returns></returns>
        public ECDSASignature Sign(byte[] message)
        {
            var signer = new ECDsaSigner(new HMacDsaKCalculator(new Sha256Digest()));

            var privKey = new ECPrivateKeyParameters(PrivateKey, CURVE);
            signer.Init(true, privKey);
            var components = signer.GenerateSignature(message);

            return new ECDSASignature(components[0], components[1]).ToCanonicalised();
        }

        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || !(o is ECKeyPair ecKeyPair) )
            {
                return false;
            }

            if (PrivateKey != null
                    ? !PrivateKey.Equals(ecKeyPair.PrivateKey) : ecKeyPair.PrivateKey != null)
            {
                return false;
            }

            return PublicKey != null
                       ? PublicKey.Equals(ecKeyPair.PublicKey) : ecKeyPair.PublicKey == null;
        }

        public override int GetHashCode()
        {
            int result = PrivateKey != null ? PrivateKey.GetHashCode() : 0;
            result = 31 * result + (PublicKey != null ? PublicKey.GetHashCode() : 0);
            return result;
        }
    }
}
