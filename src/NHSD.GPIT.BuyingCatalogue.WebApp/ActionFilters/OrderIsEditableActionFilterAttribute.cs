using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public class OrderIsEditableActionFilterAttribute(
    ILogWrapper<OrderIsEditableActionFilterAttribute> logger,
    IOrderService orderService)
    : ActionFilterAttribute
{
    private readonly ILogWrapper<OrderIsEditableActionFilterAttribute> logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly IOrderService orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.Controller is not Controller controller)
        {
            await next();
            return;
        }

        var userPrimaryOrganisationId = controller.User.GetPrimaryOrganisationInternalIdentifier();

        Match match = OrderFilterData.BasicCallOffIdRegex.Match(context.HttpContext.Request.Path);

        if (!match.Success)
        {
            await UnableToParseCallOffId();
            return;
        }

        var extractedCallOffId = match.Value;

        (var parseSuccess, CallOffId callOffId) = CallOffId.Parse(extractedCallOffId);

        if (!parseSuccess)
        {
            await UnableToParseCallOffId();
            return;
        }

        var orderIsEditable = (await orderService.GetOrderThin(callOffId, userPrimaryOrganisationId)).Order.OrderStatus
            == OrderStatus.InProgress;

        if (!orderIsEditable)
        {
            logger.LogWarning($"Attempt was made to edit non editable order {callOffId}.");
            context.Result = new BadRequestResult();
            return;
        }

        await next();
        return;

        async Task UnableToParseCallOffId()
        {
            logger.LogWarning("Unable to retrieve CallOffId from route url");
            await next();
        }
    }
}
