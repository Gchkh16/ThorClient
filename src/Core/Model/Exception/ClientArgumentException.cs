using System;

namespace ThorClient.Core.Model.Exception
{
    public class ClientArgumentException : InvalidArgumentException
    {
        private ClientArgumentException(string description) : base(description)
        {

        }

        public static ClientArgumentException Exception(string description)
        {
            return new ClientArgumentException(description);
        }
    }
}
