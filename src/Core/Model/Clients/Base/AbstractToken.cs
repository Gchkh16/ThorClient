using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace ThorClient.Core.Model.Clients.Base
{
    public class AbstractToken
    {
        public static AbstractToken VET { get; } = new AbstractToken("VET", 18);

        public string Name { get; set; }
        public int Precision { get; set; }
        public int Scale { get; set; }

        protected AbstractToken(string name, int unit)
        {
            Name = name;
            Precision = unit;
            Scale = unit;
        }
    }
}
