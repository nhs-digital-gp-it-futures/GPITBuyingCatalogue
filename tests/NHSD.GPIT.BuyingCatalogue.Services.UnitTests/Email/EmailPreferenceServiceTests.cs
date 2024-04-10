using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.Services.Email;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Email
{
    public class EmailPreferenceServiceTests
    {
        [Theory]
        [MockInMemoryDbInlineAutoData(true)]
        [MockInMemoryDbInlineAutoData(false)]
        public static async Task Get_When_No_User_Preference_Returns_Default(
            bool defaultEnabled,
            EmailPreferenceType emailPreferenceType,
            [Frozen] BuyingCatalogueDbContext context,
            EmailPreferenceService service,
            int userId)
        {
            emailPreferenceType.DefaultEnabled = defaultEnabled;
            emailPreferenceType.UserPreferences.Clear();
            context.EmailPreferenceTypes.Add(emailPreferenceType);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await service.Get(userId);

            result.Should().NotBeNull();
            result.Count().Should().Be(1);
            result[0].DefaultEnabled.Should().Be(emailPreferenceType.DefaultEnabled);
            result[0].Enabled.Should().Be(emailPreferenceType.DefaultEnabled);
            result[0].UserEnabled.Should().BeNull();
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(true, false)]
        [MockInMemoryDbInlineAutoData(false, true)]
        public static async Task Get_Returens_User_Preference(
            bool defaultEnabled,
            bool userEnabled,
            EmailPreferenceType emailPreferenceType,
            [Frozen] BuyingCatalogueDbContext context,
            EmailPreferenceService service,
            UserEmailPreference userPreference)
        {
            userPreference.Enabled = userEnabled;
            emailPreferenceType.DefaultEnabled = defaultEnabled;
            emailPreferenceType.UserPreferences.Clear();
            emailPreferenceType.UserPreferences.Add(userPreference);
            context.EmailPreferenceTypes.Add(emailPreferenceType);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await service.Get(userPreference.UserId);

            result.Should().NotBeNull();
            result.Count().Should().Be(1);
            result[0].DefaultEnabled.Should().Be(emailPreferenceType.DefaultEnabled);
            result[0].Enabled.Should().Be(userEnabled);
            result[0].UserEnabled.Should().Be(userEnabled);
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

            var preference = context.UserEmailPreferences.First(e => e.UserId == userId && e.EmailPreferenceTypeId == emailPreferenceType.Id);

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

            var preference = context.UserEmailPreferences.First(e => e.UserId == userPreference.UserId && e.EmailPreferenceTypeId == emailPreferenceType.Id);

            preference.Should().NotBeNull();
            preference.Enabled.Should().Be(!enabled);
        }
    }
}
