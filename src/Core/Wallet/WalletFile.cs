using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace ThorClient.Core.Wallet
{
    public class WalletFile
    {
        public string Address;
        public Crypto Crypto;
        public string Id;
        public int Version;
    }


    public class Crypto
    {
        public string Cipher { get; set; }
        public string Ciphertext { get; set; }
        public CipherParams Cipherparams { get; set; }

        public string Kdf { get; set; }
        public KdfParams Kdfparams { get; set; }

        public string Mac { get; set; }
    }


    public class CipherParams
    {
        public string IV { get; set; }

        public CipherParams()
        {
        }


        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (!(o is CipherParams))
            {
                return false;
            }

            var that = (CipherParams)o;

            return IV?.Equals(that.IV) ?? that.IV == null;
        }

        public override int GetHashCode()
        {
            int result = IV != null ? IV.GetHashCode() : 0;
            return result;
        }

    }

    public interface KdfParams
    {
        int Dklen { get; set; }
        string Salt { get; set; }
    }

    public class Aes128CtrKdfParams : KdfParams
    {
        public int Dklen { get; set; }
        public int C { get; set; }
        public string Prf { get; set; }
        public string Salt { get; set; }

        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (!(o is Aes128CtrKdfParams))
            {
                return false;
            }

            var that = (Aes128CtrKdfParams)o;

            if (Dklen != that.Dklen)
            {
                return false;
            }
            if (C != that.C)
            {
                return false;
            }
            if (!Prf?.Equals(that.Prf) ?? that.Prf != null)
            {
                return false;
            }
            return Salt?.Equals(that.Salt) ?? that.Salt == null;
        }


        public override int GetHashCode()
        {
            int result = Dklen;
            result = 31 * result + C;
            result = 31 * result + (Prf != null ? Prf.GetHashCode() : 0);
            result = 31 * result + (Salt != null ? Salt.GetHashCode() : 0);
            return result;
        }
    }


    public class ScryptKdfParams : KdfParams
    {
        public int Dklen { get; set; }
        public int N { get; set; }
        public int P { get; set; }
        public int R { get; set; }
        public String Salt { get; set; }

        public ScryptKdfParams()
        {
        }


        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (!(o is ScryptKdfParams)) {
                return false;
            }

            var that = (ScryptKdfParams)o;

            if (Dklen != that.Dklen)
            {
                return false;
            }
            if (N != that.N)
            {
                return false;
            }
            if (P != that.P)
            {
                return false;
            }
            if (R != that.R)
            {
                return false;
            }
            return Salt?.Equals(that.Salt) ?? that.Salt == null;
        }

        
        public override int GetHashCode()
        {
            int result = Dklen;
            result = 31 * result + N;
            result = 31 * result + P;
            result = 31 * result + R;
            result = 31 * result + (Salt != null ? Salt.GetHashCode() : 0);
            return result;
        }
    }
}
