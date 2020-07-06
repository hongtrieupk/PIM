using System;

namespace PIM.Common.CustomExceptions
{
    public class ConcurrencyUpdateException: Exception
    {
        public ConcurrencyUpdateException():base()
        {

        }
        public ConcurrencyUpdateException(string message):base(message)
        {

        }
        public ConcurrencyUpdateException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
