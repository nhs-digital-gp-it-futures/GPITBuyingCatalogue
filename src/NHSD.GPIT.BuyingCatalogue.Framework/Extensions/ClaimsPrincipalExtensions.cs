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
        public static string GetPrimaryOrganisationInternalIdentifier(this ClaimsPrincipal user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            return GetClaimValue(user, Constants.CatalogueClaims.PrimaryOrganisationInternalIdentifier);
        }

        public static IReadOnlyList<string> GetSecondaryOrganisationInternalIdentifiers(this ClaimsPrincipal user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            return user.Claims.Where(c => c.Type.EqualsIgnoreCase(Constants.CatalogueClaims.SecondaryOrganisationInternalIdentifier))
                .Select(c => c.Value)
                .ToList();
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return HasOrganisationClaim(user, OrganisationFunction.Authority.Name);
        }

        public static bool IsBuyer(this ClaimsPrincipal user)
        {
            return HasOrganisationClaim(user, OrganisationFunction.Buyer.Name);
        }

        public static bool IsAccountManager(this ClaimsPrincipal user)
        {
            return HasOrganisationClaim(user, OrganisationFunction.AccountManager.Name);
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

        private static bool HasOrganisationClaim(ClaimsPrincipal user, string name) =>
            GetClaimValue(user, ClaimTypes.Role).EqualsIgnoreCase(name);
    }
}
