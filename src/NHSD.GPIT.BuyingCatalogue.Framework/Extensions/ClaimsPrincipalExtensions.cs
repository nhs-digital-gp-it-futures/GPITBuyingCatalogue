﻿using System.Globalization;
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
            return user.Claims.Where(c => c.Type.EqualsIgnoreCase(Constants.Claims.SecondaryOrganisationOdsCode)).Select(c => c.Value).ToArray();
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return GetClaimValue(user, Constants.Claims.OrganisationFunction).Equals(OrganisationFunction.Authority.DisplayName);
        }

        public static bool IsBuyer(this ClaimsPrincipal user)
        {
            return GetClaimValue(user, Constants.Claims.OrganisationFunction).Equals(OrganisationFunction.Buyer.DisplayName);
        }

        public static int UserId(this ClaimsPrincipal user)
        {
            var idValue = GetClaimValue(user, Constants.Claims.UserId);

            return int.Parse(idValue, NumberStyles.Integer, CultureInfo.InvariantCulture);
        }

        private static string GetClaimValue(ClaimsPrincipal user, string claimType)
        {
            var claim = user.Claims.FirstOrDefault(c => c.Type.EqualsIgnoreCase(claimType));

            return claim is not null ? claim.Value : string.Empty;
        }
    }
}
