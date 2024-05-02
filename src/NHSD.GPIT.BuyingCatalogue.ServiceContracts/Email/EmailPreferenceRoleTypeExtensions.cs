using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;

public static class EmailPreferenceRoleTypeExtensions
{
    public static bool IsRoleMatch(this EmailPreferenceRoleType type, IEnumerable<AspNetRole> roles)
    {
        return type switch
        {
            EmailPreferenceRoleType.All => true,
            EmailPreferenceRoleType.Admins =>
                roles.Any(x => string.Equals(x.Name, OrganisationFunction.Authority.Name)),
            EmailPreferenceRoleType.Buyers
                => roles.Any(
                    x => string.Equals(x.Name, OrganisationFunction.Buyer.Name)
                        || string.Equals(x.Name, OrganisationFunction.AccountManager.Name)),
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };
    }
}
