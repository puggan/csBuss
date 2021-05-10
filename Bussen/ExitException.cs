namespace Bussen
{
    public class ExitException : System.Exception
    {
        public ExitException(string message) : base(message)
        {
        }
    }
}