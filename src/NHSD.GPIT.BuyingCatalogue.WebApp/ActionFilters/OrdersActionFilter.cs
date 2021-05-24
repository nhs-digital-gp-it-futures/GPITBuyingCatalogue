using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters
{
    public class OrdersActionFilter : IAsyncActionFilter
    {
        private readonly ILogWrapper<OrdersActionFilter> logger;

        public OrdersActionFilter(ILogWrapper<OrdersActionFilter> logger)
        {
            this.logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.Request.Path.StartsWithSegments("/order/organisation", StringComparison.InvariantCultureIgnoreCase))
            {
                if (!context.HttpContext.User.IsBuyer())
                {
                    logger.LogWarning($"Attempt was made to access {context.HttpContext.Request.Path} when user is not a buyer.");
                    context.Result = new NotFoundResult();
                    return;
                }

                var odsCode = context.HttpContext.Request.Path.ToString().Split('/', StringSplitOptions.RemoveEmptyEntries)[2];

                if (!odsCode.Equals(context.HttpContext.User.GetPrimaryOdsCode(), StringComparison.InvariantCultureIgnoreCase))
                {
                    // TODO - Now we need to do a service lookup to see if the user has permission to order on behalf of this ODS code
                    if (odsCode == "03P")
                    {
                        logger.LogWarning($"Attempt was made to access {context.HttpContext.Request.Path} when user cannot access {odsCode}.");
                        context.Result = new NotFoundResult();
                        return;
                    }
                }
            }

            await next();
        }
    }
}
