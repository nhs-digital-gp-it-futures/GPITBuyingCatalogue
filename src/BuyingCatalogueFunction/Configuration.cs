using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace BuyingCatalogueFunction
{
    public static class Configuration
    {
        public static string GetOrThrow(string key, ILogger logger)
        {
            var value = Environment.GetEnvironmentVariable(key);

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            logger.LogCritical("Attempted to get a value for {Key} from the environment but no value was found", key);

            throw new KeyNotFoundException(key);
        }
    }
}
