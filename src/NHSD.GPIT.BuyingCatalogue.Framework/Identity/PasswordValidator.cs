using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Identity
{
    public sealed class PasswordValidator : PasswordValidator<AspNetUser>
    {
        public const string PasswordMismatchCode = "PasswordMismatch";
        public const string InvalidPasswordCode = "InvalidPassword";
        public const string PasswordConditionsNotMet = "The password you’ve entered does not meet the password policy";
        public const string PasswordAlreadyUsedCode = "HistoricalPassword";
        public const string PasswordAlreadyUsed = "Password was used previously. Enter a different password";

        private readonly BuyingCatalogueDbContext dbContext;
        private readonly IPasswordHasher<AspNetUser> passwordHash;
        private readonly PasswordSettings passwordSettings;

        public PasswordValidator()
        {
        }

        public PasswordValidator(
            BuyingCatalogueDbContext dbContext,
            IPasswordHasher<AspNetUser> passwordHash,
            PasswordSettings passwordSettings)
        {
            this.passwordSettings = passwordSettings ?? throw new ArgumentNullException(nameof(passwordSettings));
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.passwordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        }

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
            {
                return await IsDuplicatePassword(user, password)
                    ? IdentityResult.Failed(
                        new IdentityError { Code = PasswordAlreadyUsedCode, Description = PasswordAlreadyUsed })
                    : result;
            }

            return IdentityResult.Failed(
                new IdentityError { Code = InvalidPasswordCode, Description = PasswordConditionsNotMet });
        }

        private async Task<bool> IsDuplicatePassword(AspNetUser user, string newPassword)
        {
            var history = await dbContext.AspNetUsers.TemporalAll()
                .Where(x => x.Id == user.Id)
                .OrderByDescending(x => x.LastUpdated)
                .Select(x => x.PasswordHash)
                .Where(x => x != null)
                .Distinct()
                .Take(passwordSettings.NumOfPreviousPasswords)
                .ToListAsync();

            if (!history.Any())
            {
                return false;
            }

            return history
                .Select(x => passwordHash.VerifyHashedPassword(user, x, newPassword))
                .Any(x => x != PasswordVerificationResult.Failed);
        }
    }
}
