using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Middleware;

public sealed class SafeRefererMiddleware
{
    public const string SafeRefererItemKey = "SafeLocalRefererPathAndQuery";

    private readonly RequestDelegate next;
    private readonly ILogger<SafeRefererMiddleware> logger;

    public SafeRefererMiddleware(RequestDelegate next, ILogger<SafeRefererMiddleware> logger)
    {
        this.next = next;
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Invoke(HttpContext context)
    {
        var raw = context.Request.Headers.Referer.ToString();

        if (Uri.TryCreate(raw, UriKind.Absolute, out var refererUri) && IsSameOrigin(context, refererUri))
        {
            var safePathAndQuery = refererUri.PathAndQuery.StartsWith("/", StringComparison.Ordinal)
                ? refererUri.PathAndQuery
                : "/";

            context.Items[SafeRefererItemKey] = safePathAndQuery;
            context.Request.Headers.Referer = BuildAbsoluteLocalUrl(context, safePathAndQuery);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(raw))
            {
                logger.LogInformation(
                    "Rejected non-local Referer. RequestPath: {RequestPath}, Referer: {Referer}",
                    context.Request.Path,
                    raw);
            }

            const string fallbackPath = "/";
            context.Items[SafeRefererItemKey] = fallbackPath;
            context.Request.Headers.Referer = BuildAbsoluteLocalUrl(context, fallbackPath);
        }

        await next(context).ConfigureAwait(false);
    }

    private static bool IsSameOrigin(HttpContext context, Uri refererUri)
    {
        if (!string.Equals(refererUri.Scheme, context.Request.Scheme, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.Equals(refererUri.Host, context.Request.Host.Host, StringComparison.OrdinalIgnoreCase))
            return false;

        var requestPort = context.Request.Host.Port.GetValueOrDefault(
            string.Equals(context.Request.Scheme, "https", StringComparison.OrdinalIgnoreCase) ? 443 : 80);

        var refererPort = refererUri.IsDefaultPort
            ? (string.Equals(refererUri.Scheme, "https", StringComparison.OrdinalIgnoreCase) ? 443 : 80)
            : refererUri.Port;

        return refererPort == requestPort;
    }

    private static string BuildAbsoluteLocalUrl(HttpContext context, string pathAndQuery)
        => $"{context.Request.Scheme}://{context.Request.Host}{pathAndQuery}";
}
