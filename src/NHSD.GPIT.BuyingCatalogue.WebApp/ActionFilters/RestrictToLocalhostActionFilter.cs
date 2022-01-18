﻿using System;
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
            var remoteIp = context.HttpContext.Connection.RemoteIpAddress;

            if (!IPAddress.IsLoopback(remoteIp))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
