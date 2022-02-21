using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters
{
    public sealed class OrdersActionFilter : IAsyncActionFilter
    {
        private readonly ILogWrapper<OrdersActionFilter> logger;

        public OrdersActionFilter(ILogWrapper<OrdersActionFilter> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

                var internalOrgId = context.HttpContext.Request.Path.ToString().Split('/', StringSplitOptions.RemoveEmptyEntries)[2];

                if (!internalOrgId.Equals(context.HttpContext.User.GetPrimaryOrganisationInternalIdentifier(), StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!context.HttpContext.User.GetSecondaryOrganisationInternalIdentifiers().Any(s => s.EqualsIgnoreCase(internalOrgId)))
                    {
                        logger.LogWarning($"Attempt was made to access {context.HttpContext.Request.Path} when user cannot access {internalOrgId}.");
                        context.Result = new NotFoundResult();
                        return;
                    }
                }
            }

            await next();
        }
    }
}
