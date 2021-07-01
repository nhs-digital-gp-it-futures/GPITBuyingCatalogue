using System.Linq;
using System.Security.Claims;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetPrimaryOrganisationName(this ClaimsPrincipal user)
        {
            return GetClaimValue(user, Constants.Claims.PrimaryOrganisationName);
        }

        public static string GetUserDisplayName(this ClaimsPrincipal user)
        {
            return GetClaimValue(user, Constants.Claims.UserDisplayName);
        }

        public static string GetPrimaryOdsCode(this ClaimsPrincipal user)
        {
            return GetClaimValue(user, Constants.Claims.PrimaryOrganisationOdsCode);
        }

        public static string[] GetSecondaryOdsCodes(this ClaimsPrincipal user)
        {
            return user.Claims.Where(x => x.Type.EqualsIgnoreCase(Constants.Claims.SecondaryOrganisationOdsCode)).Select(x => x.Value).ToArray();
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return GetClaimValue(user, Constants.Claims.OrganisationFunction).Equals(OrganisationFunction.Authority.DisplayName);
        }

        public static bool IsBuyer(this ClaimsPrincipal user)
        {
            return GetClaimValue(user, Constants.Claims.OrganisationFunction).Equals(OrganisationFunction.Buyer.DisplayName);
        }

        private static string GetClaimValue(ClaimsPrincipal user, string claimType)
        {
            var claim = user.Claims.FirstOrDefault(x => x.Type.EqualsIgnoreCase(claimType));

            return claim != null ? claim.Value : string.Empty;
        }
    }
}
