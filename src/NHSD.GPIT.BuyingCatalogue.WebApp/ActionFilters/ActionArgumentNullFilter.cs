using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters
{
    public class ActionArgumentNullFilter : IAsyncActionFilter
    {
        private readonly ILogWrapper<ActionArgumentNullFilter> logger;

        public ActionArgumentNullFilter(ILogWrapper<ActionArgumentNullFilter> logger)
        {
            this.logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var isValid = true;

            foreach (var argument in context.ActionArguments)
            {
                if (!isValid)
                    break;

                switch (argument.Value)
                {
                    case string:
                        if (string.IsNullOrWhiteSpace((string)argument.Value))
                        {
                            isValid = false;
                            logger.LogWarning($"{argument.Key} was null");
                        }

                        break;
                    case Guid:
                        if ((Guid)argument.Value == Guid.Empty)
                        {
                            isValid = false;
                            logger.LogWarning($"{argument.Key} was null");
                        }

                        break;
                    default:
                        if (argument.Value == null)
                        {
                            isValid = false;
                            logger.LogWarning($"{argument.Key} was null");
                        }

                        break;
                }
            }

            if (isValid)
            {
                await next();
            }
            else
            {
                context.Result = new BadRequestResult();
            }
        }
    }
}
