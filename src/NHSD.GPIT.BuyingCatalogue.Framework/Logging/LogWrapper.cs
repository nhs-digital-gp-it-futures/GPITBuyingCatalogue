using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Logging
{
    [ExcludeFromCodeCoverage]
    public class LogWrapper<T> : ILogWrapper<T>
        where T : class
    {
        private readonly ILogger<T> logger;

        public LogWrapper(ILogger<T> logger)
        {
            this.logger = logger;
        }

        public void LogError(string message, params object[] args)
        {
            logger.LogError(message, args);
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            logger.LogError(exception, message, args);
        }

        public void LogInformation(string message, params object[] args)
        {
            logger.LogInformation(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            logger.LogWarning(message, args);
        }

        public void LogTrace(string message, params object[] args)
        {
            logger.LogTrace(message);
        }
    }
}
