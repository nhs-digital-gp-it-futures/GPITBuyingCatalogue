﻿using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class SerilogMvcLoggingAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var diagnosticContext = context.HttpContext.RequestServices.GetService<IDiagnosticContext>();
            diagnosticContext.Set("ActionName", context.ActionDescriptor.DisplayName);
        }
    }
}
