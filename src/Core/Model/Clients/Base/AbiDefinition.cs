using System.Collections.Generic;
using System.Text;
using ThorClient.Utils;

namespace ThorClient.Core.Model.Clients.Base
{
    public class AbiDefinition
    {
        public bool Constant { get; set; }
        public List<NamedType> Inputs { get; set; }
        public string Name { get; set; }
        public List<NamedType> Outputs { get; set; }
        public string Type { get; set; }
        public bool Payable { get; set; }

        public class NamedType
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public bool Indexed { get; set; }

            public NamedType()
            {
            }

            public NamedType(string name, string type)
            {
                Name = name;
                Type = type;
            }

            public NamedType(string name, string type, bool indexed)
            {
                Name = name;
                Type = type;
                Indexed = indexed;
            }

            public override bool Equals(object o)
            {
                if (this == o)
                {
                    return true;
                }
                if (!(o is NamedType))
                {
                    return false;
                }

                var namedType = (NamedType)o;

                if (Indexed != namedType.Indexed)
                {
                    return false;
                }

                if (!Name?.Equals(namedType.Name) ?? namedType.Name != null)
                {
                    return false;
                }
                return Type?.Equals(namedType.Type) ?? namedType.Type == null;
            }

            public override int GetHashCode()
            {
                int result = Name != null ? Name.GetHashCode() : 0;
                result = 31 * result + (Type != null ? Type.GetHashCode() : 0);
                result = 31 * result + (Indexed ? 1 : 0);
                return result;
            }

        }

        public string GetHexMethodCodeNoPrefix()
        {
            var hashCode = GetBytesMethodHashed();
            return BytesUtils.ToHexString(hashCode, null).Substring(0, 8);
        }

        public byte[] GetBytesMethodHashed()
        {
            string name = this.Name;
            string methodSignature = BuildMethodSignature(name);
            return CryptoUtils.Keccak256(Encoding.UTF8.GetBytes(methodSignature));
        }

        private string BuildMethodSignature(string methodName)
        {
            var result = new StringBuilder();
            result.Append(methodName);
            result.Append("(");
            int index = 0;
            foreach (var namedType in this.Inputs)
            {
                result.Append(namedType.Type);
                index++;
                if (index < Inputs.Count)
                {
                    result.Append(",");
                }
            }
            result.Append(")");
            return result.ToString();
        }
    }
}
