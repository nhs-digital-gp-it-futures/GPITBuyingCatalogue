using System;

namespace NHSD.GPIT.BuyingCatalogue.Services.Document
{
    public sealed class DocumentRepositoryException : Exception
    {
        public const string DefaultMessage = "An error occurred with the document repository.";

        public DocumentRepositoryException()
            : this(DefaultMessage)
        {
        }

        public DocumentRepositoryException(string message)
            : base(message)
        {
        }

        public DocumentRepositoryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public DocumentRepositoryException(Exception innerException, int httpStatusCode)
            : this(innerException.Message, innerException)
        {
            HttpStatusCode = httpStatusCode;
        }

        public int HttpStatusCode { get; }
    }
}
