namespace ThorClient.Core.Model.Exception
{
    public class ClientIOException : ThorException
    {
        public int HttpStatus { get; set; }

        public ClientIOException(System.Exception ex) : base(ex)
        {
            HttpStatus = -1;
        }

        public ClientIOException(string message) : base(message)
        {
            HttpStatus = -1;
        }

        public ClientIOException(string message, int status) : base(message)
        {
            HttpStatus = status;
        }
    }
}
