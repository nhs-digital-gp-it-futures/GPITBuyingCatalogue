using System;
using Bogus;
using Microsoft.AspNetCore.Identity;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestModels;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.RandomData
{
    internal static class GenerateUser
    {
        internal static User Generate()
        {
            return new Faker<User>("en_GB")
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.TelephoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.EmailAddress, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .Generate();
        }

        internal static AspNetUser GenerateAspNetUser(Guid organisationId, string password, bool isEnabled)
        {
            var user = new Faker<AspNetUser>("en_GB")
                .RuleFor(u => u.Id, f => f.Random.Guid().ToString())
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(u => u.UserName, (f, u) => u.Email)
                .RuleFor(u => u.NormalizedEmail, (f, u) => u.Email.ToUpper())
                .RuleFor(u => u.NormalizedUserName, (f, u) => u.NormalizedEmail)
                .RuleFor(u => u.Disabled, f => !isEnabled)
                .RuleFor(u => u.EmailConfirmed, f => true)
                .RuleFor(u => u.CatalogueAgreementSigned, f => true)
                .RuleFor(u => u.OrganisationFunction, f => "Buyer")
                .RuleFor(u => u.PrimaryOrganisationId, f => organisationId)
                .RuleFor(u => u.SecurityStamp, f => f.Random.Guid().ToString())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.PhoneNumberConfirmed, f => false)
                .Generate();
            user.PasswordHash = new PasswordHasher<AspNetUser>().HashPassword(user, password);

            return user;
        }
    }
}
