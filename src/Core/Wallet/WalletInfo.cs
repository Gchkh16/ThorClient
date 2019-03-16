using System;
using Newtonsoft.Json;
using ThorClient.Utils.Crypto;

namespace ThorClient.Core.Wallet
{
    public class WalletInfo
    {
        public WalletFile WalletFile { get;}
        public ECKeyPair KeyPair { get; }

        public WalletInfo(WalletFile walletFile, ECKeyPair keyPair)
        {
            this.WalletFile = walletFile;
            this.KeyPair = keyPair;
        }

        public string ToKeystoreString()
        {
            return JsonConvert.SerializeObject(this.WalletFile);
        }
    }
}
