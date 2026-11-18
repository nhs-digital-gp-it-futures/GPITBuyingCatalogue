using Microsoft.AspNetCore.Http;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Extensions;

public static class HttpContextSafeRefererExtensions
{
    public static string GetSafeRefererOrFallback(this HttpContext context, string fallback)
      => context.Items.TryGetValue(Middleware.SafeRefererMiddleware.SafeRefererItemKey, out var value)
         && value is string safe
         && !string.IsNullOrWhiteSpace(safe)
          ? safe
          : fallback;
}
