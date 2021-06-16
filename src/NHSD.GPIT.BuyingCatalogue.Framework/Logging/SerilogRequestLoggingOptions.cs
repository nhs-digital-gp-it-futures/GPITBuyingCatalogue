using System;
using Microsoft.AspNetCore.Http;
using Serilog.Events;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Logging
{
    public static class SerilogRequestLoggingOptions
    {
        public static LogEventLevel GetLevel(HttpContext httpContext, double elapsed, Exception exception)
        {
            _ = elapsed;

            if (exception is not null)
                return LogEventLevel.Error;

            if (httpContext is null || httpContext.Response.StatusCode > 499)
                return LogEventLevel.Error;

            return LogEventLevel.Information;
        }
    }
}
