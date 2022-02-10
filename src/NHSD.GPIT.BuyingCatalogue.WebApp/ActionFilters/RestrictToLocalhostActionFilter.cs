using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RestrictToLocalhostActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!IPAddress.IsLoopback(context.HttpContext.Connection.RemoteIpAddress))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
