using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.Services.Email;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Email
{
    public static class EmailPreferenceServiceTests
    {
        [Theory]
        [MockInMemoryDbInlineAutoData(true)]
        [MockInMemoryDbInlineAutoData(false)]
        public static async Task Get_When_No_User_Preference_Returns_Default(
            bool defaultEnabled,
            AspNetUser user,
            EmailPreferenceType emailPreferenceType,
            [Frozen] BuyingCatalogueDbContext context,
            EmailPreferenceService service)
        {
            emailPreferenceType.RoleType = EmailPreferenceRoleType.All;
            user.LockoutEnabled = user.Disabled = false;

            emailPreferenceType.DefaultEnabled = defaultEnabled;
            emailPreferenceType.UserPreferences.Clear();

            context.EmailPreferenceTypes.Add(emailPreferenceType);
            context.AspNetUsers.Add(user);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await service.Get(user.Id);

            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result[0].DefaultEnabled.Should().Be(emailPreferenceType.DefaultEnabled);
            result[0].Enabled.Should().Be(emailPreferenceType.DefaultEnabled);
            result[0].UserEnabled.Should().BeNull();
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(true, false)]
        [MockInMemoryDbInlineAutoData(false, true)]
        public static async Task Get_Returns_User_Preference(
            bool defaultEnabled,
            bool userEnabled,
            AspNetUser user,
            EmailPreferenceType emailPreferenceType,
            [Frozen] BuyingCatalogueDbContext context,
            EmailPreferenceService service,
            UserEmailPreference userPreference)
        {
            emailPreferenceType.RoleType = EmailPreferenceRoleType.All;
            user.LockoutEnabled = user.Disabled = false;
            userPreference.UserId = user.Id;
            userPreference.Enabled = userEnabled;
            emailPreferenceType.DefaultEnabled = defaultEnabled;

            emailPreferenceType.UserPreferences.Clear();
            emailPreferenceType.UserPreferences.Add(userPreference);

            context.EmailPreferenceTypes.Add(emailPreferenceType);
            context.AspNetUsers.Add(user);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await service.Get(userPreference.UserId);

            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result[0].DefaultEnabled.Should().Be(emailPreferenceType.DefaultEnabled);
            result[0].Enabled.Should().Be(userEnabled);
            result[0].UserEnabled.Should().Be(userEnabled);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task Get_ReturnsPreferencesForUserRole(
            AspNetUser user,
            EmailPreferenceType firstEmailPreferenceType,
            EmailPreferenceType secondEmailPreferenceType,
            EmailPreferenceType thirdEmailPreferenceType,
            [Frozen] BuyingCatalogueDbContext context,
            EmailPreferenceService service)
        {
            var role = new AspNetRole { Name = OrganisationFunction.Buyer.Name, NormalizedName = OrganisationFunction.Buyer.Name.ToUpperInvariant() };

            context.Roles.Add(role);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            user.AspNetUserRoles = new List<AspNetUserRole> { new() { RoleId = role.Id } };

            firstEmailPreferenceType.RoleType = EmailPreferenceRoleType.All;
            secondEmailPreferenceType.RoleType = EmailPreferenceRoleType.Buyers;
            thirdEmailPreferenceType.RoleType = EmailPreferenceRoleType.Admins;

            context.EmailPreferenceTypes.AddRange(
                firstEmailPreferenceType,
                secondEmailPreferenceType,
                thirdEmailPreferenceType);

            context.AspNetUsers.Add(user);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await service.Get(user.Id);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(true)]
        [MockInMemoryDbInlineAutoData(false)]
        public static async Task Save_When_No_User_Preference_Exists(
            bool enabled,
            EmailPreferenceType emailPreferenceType,
            [Frozen] BuyingCatalogueDbContext context,
            EmailPreferenceService service,
            int userId,
            UserEmailPreferenceModel model)
        {
            emailPreferenceType.UserPreferences.Clear();
            context.EmailPreferenceTypes.Add(emailPreferenceType);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            model.EmailPreferenceType = emailPreferenceType.EmailPreferenceTypeAsEnum;
            model.Enabled = enabled;
            await service.Save(userId, new List<UserEmailPreferenceModel> { model });
            context.ChangeTracker.Clear();

            var preference = context.UserEmailPreferences.First(
                e => e.UserId == userId && e.EmailPreferenceTypeId == emailPreferenceType.Id);

            preference.Should().NotBeNull();
            preference.Enabled.Should().Be(enabled);
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(true)]
        [MockInMemoryDbInlineAutoData(false)]
        public static async Task Save_Updates_User_Preference(
            bool enabled,
            EmailPreferenceType emailPreferenceType,
            [Frozen] BuyingCatalogueDbContext context,
            EmailPreferenceService service,
            UserEmailPreference userPreference,
            UserEmailPreferenceModel model)
        {
            userPreference.Enabled = enabled;
            emailPreferenceType.UserPreferences.Clear();
            emailPreferenceType.UserPreferences.Add(userPreference);
            context.EmailPreferenceTypes.Add(emailPreferenceType);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            model.EmailPreferenceType = emailPreferenceType.EmailPreferenceTypeAsEnum;
            model.Enabled = !enabled;
            await service.Save(userPreference.UserId, new List<UserEmailPreferenceModel> { model });
            context.ChangeTracker.Clear();

            var preference = context.UserEmailPreferences.First(
                e => e.UserId == userPreference.UserId && e.EmailPreferenceTypeId == emailPreferenceType.Id);

            preference.Should().NotBeNull();
            preference.Enabled.Should().Be(!enabled);
        }
    }
}
