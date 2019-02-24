namespace ThorClient.Core.Model.Exception
{
    public class ThorException : System.Exception
    {
        
        public ThorException()
        {
            
        }

        public ThorException(string message) : base(message)
        {
           
        }

        public ThorException(System.Exception cause) : base(cause.Message, cause)
        {

        }
    }
}
