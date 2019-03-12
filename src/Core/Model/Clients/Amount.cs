
using System;
using Org.BouncyCastle.Math;
using ThorClient.Core.Model.Clients.Base;
using ThorClient.Core.Model.Exception;
using ThorClient.Numerics;
using ThorClient.Utils;

namespace ThorClient.Core.Model.Clients
{
    public class Amount
    {
        public static Amount ZERO { get; } = new Zero();


        public BigDecimal Value { get; set; }
        public AbstractToken Token { get; set; }


        public static Amount CreateFromToken(AbstractToken token)
        {
            var amount = new Amount
            {
                Token = token
            };
            return amount;
        }

        public static Amount VET() => CreateFromToken(AbstractToken.VET);

        public static Amount VTHO() => CreateFromToken(ERC20Token.VTHO);

        private Amount()
        {
        }


        public void SetHexAmount(string hexAmount)
        {
            if (!StringUtils.IsHex(hexAmount))
            {
                throw ClientArgumentException.Exception("setHexValue argument hex value.");
            }
            string noPrefixAmount = StringUtils.SanitizeHex(hexAmount);
            Value = BlockchainUtils.Amount(noPrefixAmount, (int)Token.Precision,
                (int)Token.Scale);
        }

        public void SetDecimalAmount(string decimalAmount)
        {
            if (string.IsNullOrWhiteSpace(decimalAmount))
            {
                throw new ArgumentException("Decimal amount string is blank", nameof(decimalAmount));
            }
            Value = BigDecimal.Parse(decimalAmount);
        }

        public string ToHexString()
        {
            var fullDecimal = Value *  BigDecimal.Pow(10, (int)Token.Precision);
            byte[] bytes = BytesUtils.TrimLeadingZeroes(fullDecimal.GetWholePart().ToByteArray());
            return BytesUtils.ToHexString(bytes, Prefix.ZeroLowerX);
        }

        public virtual byte[] ToByteArray()
        {
            return BlockchainUtils.ByteArrayAmount(Value, (int)Token.Precision);
        }


        public BigInteger ToBigInteger()
        {
            BigDecimal fullDecimal = Value * (BigDecimal.Pow(10, (int)Token.Precision));
            throw new NotImplementedException(); 
        }

        private class Zero : Amount
        {
            public override byte[] ToByteArray() => new byte[0];
        }
    }
}
