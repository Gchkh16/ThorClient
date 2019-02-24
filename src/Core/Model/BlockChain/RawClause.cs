namespace ThorClient.Core.Model.BlockChain
{
    public class RawClause
    {
        public byte[] To { get; set; }

        public byte[] Value { get; set; }

        public byte[] Data { get; set; }

        public RawClause()
        {
            To = new byte[]{};
            Value = new byte[]{};
            Data = new byte[]{};
        }

        public RawClause(byte[] to, byte[] value, byte[] data)
        {
            To = to;
            Value = value;
            Data = data;
        }
    }
}
