using System;
using System.Linq;
using System.Security.Claims;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{    
    public static class ClaimsPrincipalExtensions
    {
        public static string GetPrimaryOrganisationName(this ClaimsPrincipal user)
        {
            return GetClaimValue(user, "primaryOrganisationName");
        }

        public static string GetUserDisplayName(this ClaimsPrincipal user)
        {
            return GetClaimValue(user, "userDisplayName");
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return GetClaimValue(user, "IsAdmin").Equals("True");
        }
        public static bool IsBuyer(this ClaimsPrincipal user)
        {
            return GetClaimValue(user, "IsBuyer").Equals("True");
        }

        private static string GetClaimValue(ClaimsPrincipal user, string claimType)
        {
            var claim = user.Claims.FirstOrDefault(x => x.Type.Equals(claimType, StringComparison.InvariantCultureIgnoreCase));

            return claim != null ? claim.Value : string.Empty;
        }
    }
}
