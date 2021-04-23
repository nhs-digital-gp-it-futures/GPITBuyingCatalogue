using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Middleware
{
    public class DisableMarketingMiddleware
    {
        private readonly RequestDelegate next;

        public DisableMarketingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.HasValue &&
                context.Request.Path.Value.StartsWith("/marketing"))
            {                
                context.Response.StatusCode = 404;                
                return;
            }

            await next.Invoke(context);
        }
    }
}
