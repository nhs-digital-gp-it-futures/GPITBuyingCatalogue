using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

public static class HttpContextExtensions
{
    public static IUrlHelper GetUrlHelper(this HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var factory = httpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();

        return factory.GetUrlHelper(httpContext.GetActionContext());
    }

    private static ActionContext GetActionContext(this HttpContext httpContext)
    {
        var endpoint = httpContext.GetEndpoint();
        var actionDescriptor =
            endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>() ??
            new ActionDescriptor();

        return new ActionContext(
            httpContext,
            httpContext.GetRouteData(),
            actionDescriptor);
    }
}
