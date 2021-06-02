using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Identity
{
    public sealed class PasswordValidator : IPasswordValidator<AspNetUser>
    {
        public const string InvalidPasswordCode = "InvalidPassword";
        public const string PasswordConditionsNotMet = "The password you’ve entered does not meet the criteria";

        public Task<IdentityResult> ValidateAsync(UserManager<AspNetUser> manager, AspNetUser user, string password)
        {
            const string specialCharacters = "!@#$%^&*";

            if (password.Length < 10)
            {
                return Task.FromResult(
                    IdentityResult.Failed(
                        new IdentityError
                        {
                            Code = InvalidPasswordCode,
                            Description = PasswordConditionsNotMet,
                        }));
            }

            var validationRules = new List<Func<bool>>
            {
                () => password.Any(char.IsLower),
                () => password.Any(char.IsUpper),
                () => password.Any(char.IsDigit),
                () => password.Any(c => specialCharacters.Contains(c, StringComparison.InvariantCultureIgnoreCase)),
            };

            if (validationRules.Count(r => r()) < 3)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError
                {
                    Code = InvalidPasswordCode,
                    Description = PasswordConditionsNotMet,
                }));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
