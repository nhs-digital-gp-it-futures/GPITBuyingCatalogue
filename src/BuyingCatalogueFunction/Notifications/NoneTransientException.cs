using System;

namespace BuyingCatalogueFunction.Notifications
{
    [Serializable]
    public class NoneTransientException : Exception
    {
        public NoneTransientException()
        {
        }

        public NoneTransientException(string message)
            : base(message)
        {
        }

        public NoneTransientException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}
