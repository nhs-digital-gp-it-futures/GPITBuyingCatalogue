using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class AspNetUserExtensions
    {
        public static string GetDisplayName(this AspNetUser user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            return $"{user.FirstName} {user.LastName}";
        }

        public static string GetRoleName(this AspNetUser user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            var roleName = user.AspNetUserRoles.First()?.Role?.Name;
            return OrganisationFunction.FromName(roleName)!.DisplayName;
        }
    }
}
