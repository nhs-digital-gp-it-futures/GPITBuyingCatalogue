using Microsoft.AspNetCore.Http;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Extensions
{
    public static class HttpContextExtensions
    {
        public const string LineIdKey = "view-context-line-id";

        public static int NextLineId(this HttpContext context)
        {
            var output = (int)(context.Items[LineIdKey] ?? 0);

            context.Items[LineIdKey] = output + 1;

            return output;
        }
    }
}
