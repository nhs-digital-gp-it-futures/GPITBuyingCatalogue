using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Identity
{
    public sealed class PasswordValidator : PasswordValidator<AspNetUser>
    {
        public const string PasswordMismatchCode = "PasswordMismatch";
        public const string InvalidPasswordCode = "InvalidPassword";
        public const string PasswordConditionsNotMet = "The password you’ve entered does not meet the password policy";
        public const string PasswordAlreadyUsedCode = "HistoricalPassword";
        public const string PasswordAlreadyUsed = "Password was used previously. Try a different password";

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
            ArgumentNullException.ThrowIfNull(options);

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
            // temporal tables screw up ordering, hence the use of a projection
            var history = await dbContext.AspNetUsers.TemporalAll()
                    .Where(x => x.Id == user.Id)
                    .Select(x => new { x.PasswordHash, x.LastUpdated })
                    .ToListAsync();

            var hashes = history
                .OrderByDescending(x => x.LastUpdated)
                .Select(x => x.PasswordHash)
                .Where(x => x != null)
                .Distinct()
                .Take(passwordSettings.NumOfPreviousPasswords)
                .ToList();

            if (!hashes.Any())
            {
                return false;
            }

            return hashes
                .Select(x => passwordHash.VerifyHashedPassword(user, x, newPassword))
                .Any(x => x != PasswordVerificationResult.Failed);
        }
    }
}
