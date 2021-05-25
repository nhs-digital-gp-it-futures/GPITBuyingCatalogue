using System.Linq;
using System.Security.Claims;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;

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

        public static string GetPrimaryOdsCode(this ClaimsPrincipal user)
        {
            return GetClaimValue(user, "primaryOrganisationOdsCode");
        }

        public static string[] GetSecondaryOdsCodes(this ClaimsPrincipal user)
        {
            return user.Claims.Where(x => x.Type.EqualsIgnoreCase("secondaryOrganisationOdsCode")).Select(x => x.Value).ToArray();
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return GetClaimValue(user, "organisationFunction").Equals(OrganisationFunction.Authority.DisplayName);
        }

        public static bool IsBuyer(this ClaimsPrincipal user)
        {
            return GetClaimValue(user, "organisationFunction").Equals(OrganisationFunction.Buyer.DisplayName);
        }

        private static string GetClaimValue(ClaimsPrincipal user, string claimType)
        {
            var claim = user.Claims.FirstOrDefault(x => x.Type.EqualsIgnoreCase(claimType));

            return claim != null ? claim.Value : string.Empty;
        }
    }
}
