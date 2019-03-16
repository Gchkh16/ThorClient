using System;
using System.Linq;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Utilities;

namespace ThorClient.Utils.Crypto
{
    class ECDSASign
    {
        public static SignatureData SignMessage(byte[] message, ECKeyPair keyPair) => SignMessage(message, keyPair, true);

        /// <summary>
        /// Sign the message with <see cref="ECKeyPair"/>. Only recovery id 0 or 1 is accepted, otherwise the method will continue to retry to sign til get recovery id is 0 or 1.
        /// </summary>
        /// <param name="message">message to be signed</param>
        /// <param name="keyPair"></param>
        /// <param name="needToHash">set true if the message to be hashed before signing. If it is true, hashed first,then sign. If it is false, signed directly.</param>
        /// <returns></returns>
        public static SignatureData SignMessage(byte[] message, ECKeyPair keyPair, bool needToHash)
        {

            var publicKey = keyPair.PublicKey;
            byte[] messageHash;
            messageHash = needToHash ? CryptoUtils.Blake2b(message) : message;
            int recId = -1;
            ECDSASignature sig;

            sig = keyPair.Sign(messageHash);
            for (int i = 0; i < 4; i++)
            {
                var k = RecoverFromSignature(i, sig, messageHash);
                if (k != null && k.Equals(publicKey))
                {
                    recId = i;
                    break;
                }
            }

            if (recId == -1)
            {
                throw new SignException("Sign the data failed.");
            }

            if (recId == 2 || recId == 3)
            {
                throw new SignException("Recovery is not valid for VeChain MainNet.");
            }

            byte v = (byte)recId;
            var r = ByteUtils.ToBytesPadded(sig.R, 32);
            var s = ByteUtils.ToBytesPadded(sig.S, 32);

            return new SignatureData(v, r, s);

        }

        /// <summary>
        /// Recover the public key from signature and message.
        /// </summary>
        /// <param name="i">recovery id which 0 or 1</param>
        /// <param name="sig">a signature object</param>
        /// <param name="message">message bytes array</param>
        /// <returns>public key</returns>
        public static BigInteger RecoverFromSignature(int i, ECDSASignature sig, byte[] message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns public key from give private key
        /// </summary>
        public static BigInteger PublicKeyFromPrivate(BigInteger privateKey)
        {
            var point = PublicPointFromPrivate(privateKey);

            var encoded = point.GetEncoded(false);
            var prefixRemoved = new byte[encoded.Length - 1];
            Array.Copy(encoded, 1, prefixRemoved, 1, encoded.Length - 1);
            return new BigInteger(1, prefixRemoved);  // remove prefix
        }

        /// <summary>
        /// Returns public key point from the given private key.
        /// </summary>
        private static ECPoint PublicPointFromPrivate(BigInteger privKey)
        {
            /*
          * TODO: FixedPointCombMultiplier currently doesn't support scalars longer than the group order
          */
            if (privKey.BitLength > ECKeyPair.CURVE.N.BitLength)
            {
                privKey = privKey.Mod(ECKeyPair.CURVE.N);
            }
            return new FixedPointCombMultiplier().Multiply(ECKeyPair.CURVE.G, privKey);
        }

        public class SignatureData
        {
            public byte V { get; }
            public byte[] R { get; }
            public byte[] S { get; }


            public SignatureData(byte v, byte[] r, byte[] s)
            {
                V = v;
                R = r;
                S = s;
            }

            public override bool Equals(object o)
            {
                if (this == o)
                {
                    return true;
                }
                if (o == null || !(o is SignatureData that))
                {
                    return false;
                }

                if (V != that.V)
                {
                    return false;
                }
                if (R.SequenceEqual(that.R))
                {
                    return false;
                }

                return S.SequenceEqual(that.S);
            }


            /// <summary>
            /// Convert to bytes array. r bytes array append s bytes array, and then append v byte.
            /// </summary>
            public byte[] ToByteArray()
            {
                int size = R.Length + S.Length + 1;
                var flat = new byte[size];
                Array.Copy(R, 0, flat, 0, R.Length);
                Array.Copy(S, 0, flat, R.Length, S.Length);
                flat[size - 1] = V;
                return flat;
            }
        }

        public class SignException : Exception
        {
            public SignException(string exceptionMessage) : base(exceptionMessage)
            {
            }
        }
    }
}
