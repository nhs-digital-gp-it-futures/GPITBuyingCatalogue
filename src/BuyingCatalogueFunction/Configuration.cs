using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Logging;

namespace BuyingCatalogueFunction
{
    public static class Configuration
    {
        private const string LogMessage = "Attempted to get a value for {0} from the environment but no value was found.";

        public static string GetOrThrow(string key, ILogger logger)
        {
            var value = Environment.GetEnvironmentVariable(key);

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            logger.LogCritical(string.Format(CultureInfo.InvariantCulture, LogMessage, key));

            throw new KeyNotFoundException(key);
        }
    }
}
