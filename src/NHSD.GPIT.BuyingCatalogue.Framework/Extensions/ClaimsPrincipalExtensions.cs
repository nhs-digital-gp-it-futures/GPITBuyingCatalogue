using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;
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
            return HasOrganisationClaim(user, OrganisationFunction.Authority.Name) || HasOrganisationClaim(user, OrganisationFunction.Onboarding.Name) || HasOrganisationClaim(user, OrganisationFunction.View.Name);
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

        public static bool CanManageSolutions(this ClaimsPrincipal user)
        {
            ArgumentNullException.ThrowIfNull(user);

            return user.IsInRole(OrganisationFunction.Authority.Name) || user.HasClaim(c =>
                c.Value is CataloguePermissions.ManageCatalogueSolutions
                    or CataloguePermissions.ManageContractingVehicles
                    or CataloguePermissions.ManageSupplierDefinedEpics
                    or CataloguePermissions.ManageCapabilitiesAndEpics
                    or CataloguePermissions.ManageInteroperability);
        }

        public static bool CanManageOrganisations(this ClaimsPrincipal user)
        {
            ArgumentNullException.ThrowIfNull(user);

            return user.IsInRole(OrganisationFunction.Authority.Name) || user.HasClaim(c =>
                c.Value is CataloguePermissions.ManageBuyerOrganisations
                or CataloguePermissions.ManageSupplierOrganisations);
        }

        public static bool CanManageUsers(this ClaimsPrincipal user)
        {
            ArgumentNullException.ThrowIfNull(user);

            return user.IsInRole(OrganisationFunction.Authority.Name) || user.HasClaim(c =>
                c.Value is CataloguePermissions.ManageUsers
                or CataloguePermissions.ManageAccountCreationRequests
                or CataloguePermissions.ManageAllowedEmailDomains);
        }

        public static bool CanManageOrders(this ClaimsPrincipal user)
        {
            ArgumentNullException.ThrowIfNull(user);

            return user.IsInRole(OrganisationFunction.Authority.Name) || user.HasClaim(c =>
                c.Value is CataloguePermissions.ManageAllOrders);
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
