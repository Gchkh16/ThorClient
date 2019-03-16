using System;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using ThorClient.Utils;
using ThorClient.Utils.Crypto;

namespace ThorClient.Core.Wallet
{
    public class Wallet
    {
        private const int N_STANDARD = 1 << 18;
        private const int P_STANDARD = 1;

        private const int R = 8;
        private const int DKLEN = 32;

        private const int CURRENT_VERSION = 3;

        private const string CIPHER = "aes-128-ctr";
        private const string AES_128_CTR = "pbkdf2";
        private const string SCRYPT = "scrypt";

        public static WalletFile CreateStandard(String password, ECKeyPair ecKeyPair)
        {
            return Create(password, ecKeyPair, N_STANDARD, P_STANDARD);
        }

        public static WalletFile Create(String password, ECKeyPair ecKeyPair, int n, int p)
        {

            var salt = CryptoUtils.RandomBytes(32);

            var derivedKey = GenerateDerivedScryptKey(Encoding.UTF8.GetBytes(password), salt, n, R, p, DKLEN);

            var encryptKey = Arrays.CopyOfRange(derivedKey, 0, 16);
            var iv = CryptoUtils.RandomBytes(16);

            var privateKeyBytes = ByteUtils.ToBytesPadded(ecKeyPair.PrivateKey, ECKeyPair.PRIVATE_KEY_SIZE);

            var cipherText = PerformCipherOperation(true, iv, encryptKey, privateKeyBytes);

            var mac = GenerateMac(derivedKey, cipherText);

            return CreateWalletFile(ecKeyPair, cipherText, iv, salt, mac, n, p);
        }

        public static WalletFile CreateWalletFile(
            ECKeyPair ecKeyPair, byte[] cipherText, byte[] iv, byte[] salt, byte[] mac,
            int n, int p)
        {

            var walletFile = new WalletFile
            {
                Address = ecKeyPair.GetHexAddress()
            };

            var crypto = new Crypto
            {
                Cipher = CIPHER,
                Ciphertext = ByteUtils.ToHexString(cipherText, null)
            };
            walletFile.Crypto = crypto;

            var cipherParams = new CipherParams
            {
                IV = ByteUtils.ToHexString(iv, null)
            };
            crypto.Cipherparams = cipherParams;

            crypto.Kdf = SCRYPT;
            var kdfParams = new ScryptKdfParams
            {
                Dklen = DKLEN,
                N = n,
                P = p,
                R = R,
                Salt = ByteUtils.ToHexString(salt, null)
            };
            crypto.Kdfparams = kdfParams;

            crypto.Mac = ByteUtils.ToHexString(mac, null);
            walletFile.Crypto = crypto;
            walletFile.Id = Guid.NewGuid().ToString();
            walletFile.Version = CURRENT_VERSION;

            return walletFile;
        }

        public static byte[] GenerateDerivedScryptKey(
            byte[] password, byte[] salt, int n, int r, int p, int dkLen)
        {
            return SCrypt.Generate(password, salt, n, r, p, dkLen);
        }

        private static byte[] GenerateAes128CtrDerivedKey(
            byte[] password, byte[] salt, int c, String prf)
        {

            if (!prf.Equals("hmac-sha256"))
            {
                throw new CipherException("Unsupported prf:" + prf);
            }

            // Java 8 supports this, but you have to convert the password to a character array, see
            // http://stackoverflow.com/a/27928435/3211687

            var gen = new Pkcs5S2ParametersGenerator(new Sha256Digest());
            gen.Init(password, salt, c);
            return ((KeyParameter)gen.GenerateDerivedParameters(256)).GetKey();
        }

        /// <summary>
        /// Encodes or decodes data with aes.
        /// </summary>
        /// <param name="encryptDecrypt">tru if encrypt false if decrypt</param>
        /// <param name="iv">Initialization vector</param>
        /// <param name="encryptKey">Aes key</param>
        /// <param name="text">data to encrypt/decrypt</param>
        /// <returns></returns>
        private static byte[] PerformCipherOperation(bool encryptDecrypt, byte[] iv, byte[] encryptKey, byte[] text)
        {
            try
            {
                var aes = CipherUtilities.GetCipher("AES/CTR/NoPadding");
                aes.Init(encryptDecrypt, new ParametersWithIV(new KeyParameter(encryptKey), iv));
                return aes.DoFinal(text);
            }
            catch (Exception e)
            {
                throw new CipherException("Error performing cipher operation", e);
            }
        }

        public static byte[] GenerateMac(byte[] derivedKey, byte[] cipherText)
        {
            var result = new byte[16 + cipherText.Length];

            Array.Copy(derivedKey, 16, result, 0, 16);
            Array.Copy(cipherText, 0, result, 16, cipherText.Length);

            return CryptoUtils.Keccak256(result);
        }

        public static ECKeyPair Decrypt(String password, WalletFile walletFile)
        {

            Validate(walletFile);

            var crypto = walletFile.Crypto;

            var mac = ByteUtils.ToByteArray(crypto.Mac);
            var iv = ByteUtils.ToByteArray(crypto.Cipherparams.IV);
            var cipherText = ByteUtils.ToByteArray(crypto.Ciphertext);

            byte[] derivedKey;

            var kdfParams = crypto.Kdfparams;
            if (kdfParams is ScryptKdfParams)
            {
                var scryptKdfParams =
                        (ScryptKdfParams)crypto.Kdfparams;
                int dklen = scryptKdfParams.Dklen;
                int n = scryptKdfParams.N;
                int p = scryptKdfParams.P;
                int r = scryptKdfParams.R;
                var salt = ByteUtils.ToByteArray(scryptKdfParams.Salt);
                derivedKey = GenerateDerivedScryptKey(Encoding.UTF8.GetBytes(password), salt, n, r, p, dklen);
            }
            else if (kdfParams is Aes128CtrKdfParams)
            {
                var aes128CtrKdfParams =
                        (Aes128CtrKdfParams)crypto.Kdfparams;
                int c = aes128CtrKdfParams.C;
                String prf = aes128CtrKdfParams.Prf;
                var salt = ByteUtils.ToByteArray(aes128CtrKdfParams.Salt);

                derivedKey = GenerateAes128CtrDerivedKey(Encoding.UTF8.GetBytes(password), salt, c, prf);
            }
            else
            {
                throw new CipherException("Unable to deserialize params: " + crypto.Kdf);
            }

            var derivedMac = GenerateMac(derivedKey, cipherText);

            if (!Arrays.Equals(derivedMac, mac))
            {
                throw new CipherException("Invalid password provided");
            }

            var encryptKey = Arrays.CopyOfRange(derivedKey, 0, 16);
            var privateKey = PerformCipherOperation(false, iv, encryptKey, cipherText);
            return ECKeyPair.Create(privateKey);
        }

        static void Validate(WalletFile walletFile)
        {
            var crypto = walletFile.Crypto;

            if (walletFile.Version != CURRENT_VERSION)
            {
                throw new CipherException("Wallet version is not supported");
            }

            if (!crypto.Cipher.Equals(CIPHER))
            {
                throw new CipherException("Wallet cipher is not supported");
            }

            if (!crypto.Kdf.Equals(AES_128_CTR) && !crypto.Kdf.Equals(SCRYPT))
            {
                throw new CipherException("KDF type is not supported");
            }
        }
    }
}
