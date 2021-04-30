using System;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class ExceptionExtensions
    {
        public static string FullErrorMessage(this Exception exception)
        {
            var errorMessage = exception?.Message;
            if (exception?.InnerException is { } innerException)
                errorMessage += $". Inner Exception Message: {innerException.Message}";

            return errorMessage;
        }
    }
}