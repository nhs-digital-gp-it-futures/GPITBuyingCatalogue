using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters
{
    public sealed class BadRequestActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is not BadRequestObjectResult badRequestResult)
            {
                return;
            }

            context.Result = new RedirectToActionResult(
                nameof(HomeController.Error),
                typeof(HomeController).ControllerName(),
                new { error = badRequestResult.Value });
        }
    }
}
