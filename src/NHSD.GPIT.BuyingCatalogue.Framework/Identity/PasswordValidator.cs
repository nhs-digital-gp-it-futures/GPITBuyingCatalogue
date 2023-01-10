using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Identity
{
    public sealed class PasswordValidator : PasswordValidator<AspNetUser>
    {
        public const string InvalidPasswordCode = "InvalidPassword";
        public const string PasswordConditionsNotMet = "The password you’ve entered does not meet the password policy";

        public static void ConfigurePasswordOptions(PasswordOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            options.RequireDigit = true;
            options.RequireUppercase = true;
            options.RequireLowercase = true;
            options.RequireNonAlphanumeric = true;
            options.RequiredLength = 10;
            options.RequiredUniqueChars = 1;
        }

        public override async Task<IdentityResult> ValidateAsync(
            UserManager<AspNetUser> manager,
            AspNetUser user,
            string password)
        {
            var result = await base.ValidateAsync(manager, user, password);

            if (result.Succeeded)
                return result;

            return IdentityResult.Failed(
                new IdentityError { Code = InvalidPasswordCode, Description = PasswordConditionsNotMet });
        }
    }
}
