using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity
{
    public class IdentityService : IIdentityService
    {
        public const string UserId = "userId";

        public const string UserDisplayName = "userDisplayName";

        private readonly IHttpContextAccessor context;

        public IdentityService(IHttpContextAccessor context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public int? GetUserId()
        {
            if (context.HttpContext is null || context.HttpContext.User is null)
                return null;

            return int.TryParse(GetClaimValue(context.HttpContext.User, UserId), out var userId) ? userId : null;
        }

        private static string GetClaimValue(ClaimsPrincipal user, string claimType)
        {
            var claim = user.Claims.FirstOrDefault(c => c.Type.Equals(claimType, StringComparison.OrdinalIgnoreCase));

            return claim is not null ? claim.Value : string.Empty;
        }
    }
}
