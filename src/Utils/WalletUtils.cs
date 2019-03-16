using System;
using System.IO;
using Newtonsoft.Json;
using ThorClient.Core.Wallet;
using ThorClient.Utils.Crypto;

namespace ThorClient.Utils
{
    public static class WalletUtils
    {
        /// <summary>
        /// Load keystore for keystore string and passphases.
        /// </summary>
        /// <param name="keystore">
        ///            keystore string </param>
        /// <param name="passphases">
        ///            password string to encrypt </param>
        /// <returns> <seealso cref="WalletInfo"/> </returns>
        public static WalletInfo LoadKeystore(string keystore, string passphases)
        {
            if (string.IsNullOrWhiteSpace(keystore) && string.IsNullOrWhiteSpace(passphases))
            {
                return null;
            }

            //ObjectMapper objectMapper = new ObjectMapper();
            keystore = keystore.ToLower();
            WalletFile walletFile = null;
            try
            {
                walletFile = JsonConvert.DeserializeObject<WalletFile>(keystore);
            } catch (IOException e) {
                System.Console.WriteLine(e.StackTrace);
                System.Console.WriteLine(Environment.NewLine);
                return null;
            }

            ECKeyPair ecKeyPair = null;
            try {
                ecKeyPair = Wallet.Decrypt(passphases, walletFile);
            } catch (CipherException e) {
                System.Console.WriteLine(e.StackTrace);
                System.Console.WriteLine(Environment.NewLine);
                return null;
            }

            return new WalletInfo(walletFile, ecKeyPair);
        }

        /// <summary>
        /// Create wallet from password.
        /// </summary>
        /// <param name="passphases">password to encrypt the private key. </param>
        /// <returns> <seealso cref="WalletInfo"/> </returns>
        public static WalletInfo CreateWallet(string passphases)
        {
            if (string.IsNullOrWhiteSpace(passphases))
            {
                return null;
            }
            ECKeyPair keyPair = ECKeyPair.Create();
            WalletFile walletFile = null;
            try
            {
                walletFile = Wallet.CreateStandard(passphases, keyPair);
            }
            catch (CipherException e)
            {
                System.Console.WriteLine(e.StackTrace);
                System.Console.WriteLine(Environment.NewLine);
            }

            return new WalletInfo(walletFile, keyPair);
        }
    }
}
