using System;
using System.IO;
using DocumentFormat.OpenXml.Bibliography;
using ThorClient.Core.Wallet;
using ThorClient.Utils;
using static System.Console;

namespace ThorClient.Console
{
    public static class WalletConsole
    {
        public static void CreateWalletToKeystoreFile(string[] args)
        {
            if (args.Length < 2)
            {
                WriteLine("You have input invalid parameters.");
                Environment.Exit(0);
            }
            WalletInfo walletInfo = WalletUtils.CreateWallet(args[1]);
            byte[] rawPrivateKey = walletInfo.KeyPair.GetRawPrivateKey();
            string newPrivateKey = ByteUtils.ToHexString(rawPrivateKey, Prefix.ZeroLowerX);
            string keyStoreStr = walletInfo.ToKeystoreString();
            string path = "./keystore.json";
            if (args.Length > 2)
            {
                path = args[2];
            }

            var parent = Directory.GetDirectoryRoot(path);
            if (!string.IsNullOrEmpty(parent)) Directory.CreateDirectory(parent);
            File.WriteAllText(path, keyStoreStr);
            WriteLine("The wallet created successfully and the key store is:");
            WriteLine(keyStoreStr);
            WriteLine("The wallet created successfully and the privateKey is:");
            WriteLine(newPrivateKey);
        }
    }
}
