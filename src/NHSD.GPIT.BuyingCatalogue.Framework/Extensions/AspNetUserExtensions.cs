using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

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
    }
}
