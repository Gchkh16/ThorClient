namespace ThorClient.Utils
{
    
    public class Prefix
    {
        /// <summary>
        /// "VX" prefix string
        /// </summary>
        public static readonly Prefix VeChainX = new Prefix("VX");

        /// <summary>
        /// "0x" prefix string
        /// </summary>
        public static readonly Prefix ZeroLowerX = new Prefix("0x");

        private readonly string prefixString;

        private Prefix(string prefixString)
        {
            this.prefixString = prefixString;
        }

        public override string ToString() => prefixString;

        public static implicit operator string(Prefix prefix) => prefix.ToString();
    }
}
