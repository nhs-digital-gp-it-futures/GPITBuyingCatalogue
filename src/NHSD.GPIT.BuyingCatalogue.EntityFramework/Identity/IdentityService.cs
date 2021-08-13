using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity
{
    public sealed class IdentityService : IIdentityService
    {
        public const string UserId = "userId";

        public const string UserDisplayName = "userDisplayName";

        private readonly IHttpContextAccessor context;

        public IdentityService(IHttpContextAccessor context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public (int UserId, string UserName) GetUserInfo()
        {
            if (context.HttpContext is null)
                throw new InvalidOperationException();

            var userId = int.Parse(GetClaimValue(context.HttpContext.User, UserId), CultureInfo.InvariantCulture);
            var userName = GetClaimValue(context.HttpContext.User, UserDisplayName);

            return (userId, userName);
        }

        private static string GetClaimValue(ClaimsPrincipal user, string claimType)
        {
            var claim = user.Claims.FirstOrDefault(c => c.Type.Equals(claimType, StringComparison.OrdinalIgnoreCase));

            return claim is not null ? claim.Value : string.Empty;
        }
    }
}
