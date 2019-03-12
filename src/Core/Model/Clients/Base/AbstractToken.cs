using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using ThorClient.Numerics;

namespace ThorClient.Core.Model.Clients.Base
{
    public class AbstractToken
    {
        public static AbstractToken VET { get; } = new AbstractToken("VET", 18);

        public string Name { get; set; }
        public BigDecimal Precision { get; set; }
        public BigDecimal Scale { get; set; }

        protected AbstractToken(string name, int unit)
        {
            Name = name;
            Precision = unit;
            Scale = unit;
        }
    }
}
