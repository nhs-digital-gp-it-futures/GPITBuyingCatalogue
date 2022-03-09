using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters
{
    public class LogFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<LogFilter> logger;

        public LogFilter(ILogger<LogFilter> logger)
        {
            this.logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            logger.LogError(context.Exception, "An unhandled exception occurred");
        }
    }
}
