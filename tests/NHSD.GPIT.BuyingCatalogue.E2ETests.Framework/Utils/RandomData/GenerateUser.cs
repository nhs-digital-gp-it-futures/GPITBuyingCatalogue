using Bogus;
using Microsoft.AspNetCore.Identity;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Utils.TestModels;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Utils.RandomData
{
    public static class GenerateUser
    {
        public static User Generate()
        {
            return new Faker<User>("en_GB")
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.EmailAddress, (f, u) => f.Internet.Email(u.FirstName, u.LastName, "nhs.net"))
                .Generate();
        }

        public static AspNetUser GenerateAspNetUser(BuyingCatalogueDbContext context, int organisationId, string password, bool isEnabled, string accountType = "Buyer")
        {
            var role = context.Roles.First(r => r.Name == accountType);

            var user = new Faker<AspNetUser>("en_GB")
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(u => u.UserName, (_, u) => u.Email)
                .RuleFor(u => u.NormalizedEmail, (_, u) => u.Email.ToUpper())
                .RuleFor(u => u.NormalizedUserName, (_, u) => u.NormalizedEmail)
                .RuleFor(u => u.Disabled, _ => !isEnabled)
                .RuleFor(u => u.EmailConfirmed, _ => true)
                .RuleFor(u => u.CatalogueAgreementSigned, _ => true)
                .RuleFor(u => u.PrimaryOrganisationId, _ => organisationId)
                .RuleFor(u => u.SecurityStamp, f => f.Random.Guid().ToString())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.PhoneNumberConfirmed, _ => false)
                .Generate();
            user.PasswordHash = new PasswordHasher<AspNetUser>().HashPassword(user, password);
            user.AspNetUserRoles = new List<AspNetUserRole>
            {
                new() { Role = role },
            };

            return user;
        }
    }
}
