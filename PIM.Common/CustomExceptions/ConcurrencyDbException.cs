using System;

namespace PIM.Common.CustomExceptions
{
    public class ConcurrencyDbException: Exception
    {
        public ConcurrencyDbException():base()
        {

        }
        public ConcurrencyDbException(string message):base(message)
        {

        }
        public ConcurrencyDbException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
