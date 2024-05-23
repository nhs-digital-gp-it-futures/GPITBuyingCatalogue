using System;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Extensions
{
    public static class HttpContextExtensions
    {
        public const string LineIdKey = "view-context-line-id";

        public static UriBuilder GetRefererUriBuilder(this HttpContext context)
        {
            var request = context.Request.GetUri();
            var referer = new Uri(context.Request.Headers.Referer.ToString());
            return new UriBuilder(request.GetLeftPart(UriPartial.Authority))
            {
                Path = referer.LocalPath,
                Query = referer.Query,
            };
        }

        public static int NextLineId(this HttpContext context)
        {
            var output = (int)(context.Items[LineIdKey] ?? 0);

            context.Items[LineIdKey] = output + 1;

            return output;
        }
    }
}
