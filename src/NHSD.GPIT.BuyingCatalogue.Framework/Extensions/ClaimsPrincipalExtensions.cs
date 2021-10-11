using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetPrimaryOrganisationName(this ClaimsPrincipal user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            return GetClaimValue(user, Constants.CatalogueClaims.PrimaryOrganisationName);
        }

        public static string GetUserDisplayName(this ClaimsPrincipal user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            return GetClaimValue(user, Constants.CatalogueClaims.UserDisplayName);
        }

        public static string GetPrimaryOdsCode(this ClaimsPrincipal user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            return GetClaimValue(user, Constants.CatalogueClaims.PrimaryOrganisationOdsCode);
        }

        public static IReadOnlyList<string> GetSecondaryOdsCodes(this ClaimsPrincipal user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            return user.Claims.Where(c => c.Type.EqualsIgnoreCase(Constants.CatalogueClaims.SecondaryOrganisationOdsCode))
                .Select(c => c.Value)
                .ToList();
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return HasOrganisationClaim(user, OrganisationFunction.Authority.DisplayName);
        }

        public static bool IsBuyer(this ClaimsPrincipal user)
        {
            return HasOrganisationClaim(user, OrganisationFunction.Buyer.DisplayName);
        }

        public static int UserId(this ClaimsPrincipal user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            var idValue = GetClaimValue(user, Constants.CatalogueClaims.UserId);

            return int.Parse(idValue, NumberStyles.Integer, CultureInfo.InvariantCulture);
        }

        private static string GetClaimValue(ClaimsPrincipal user, string claimType)
        {
            var claim = user.Claims.FirstOrDefault(c => c.Type.EqualsIgnoreCase(claimType));

            return claim is not null ? claim.Value : string.Empty;
        }

        private static bool HasOrganisationClaim(ClaimsPrincipal user, string displayName) =>
            GetClaimValue(user, Constants.CatalogueClaims.OrganisationFunction).EqualsIgnoreCase(displayName);
    }
}
