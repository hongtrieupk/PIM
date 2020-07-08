using System;

namespace PIM.Common.CustomExceptions
{
    public class NotFoundFromDbException : Exception
    {
        public NotFoundFromDbException() : base()
        {

        }
        public NotFoundFromDbException(string message) : base(message)
        {

        }
        public NotFoundFromDbException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
