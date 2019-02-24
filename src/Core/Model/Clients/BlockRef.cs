using System;
using ThorClient.Utils;

namespace ThorClient.Core.Model.Clients
{
    public class BlockRef
    {
        private readonly byte[] _blockRef;

        private BlockRef(byte[] blockIdBytes)
        {
            _blockRef = new byte[8];
            Array.Copy(blockIdBytes, 0, _blockRef, 0, 8);
        }

        /// <summary>
        /// Create block reference from block hex string
        /// </summary>
        /// <param name="hexBlockId">hex string with "0x" prefix</param>
        /// <returns>block reference used to send transaction.</returns>
        public static BlockRef Create(string hexBlockId)
        {
            if (!StringUtils.IsHex(hexBlockId))
            {
                throw new ArgumentException("hex block id is invalid");
            }
            var blockIdBytes = BytesUtils.ToByteArray(hexBlockId);
            return new BlockRef(blockIdBytes);
        }

        public override string ToString() => BytesUtils.ToHexString(_blockRef, Prefix.ZeroLowerX);

        public byte[] ToByteArray() => _blockRef;
    }
}
