using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using BuyingCatalogueFunction.Notifications.Services;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace BuyingCatalogueFunctionTests.Notifications.Services;

public static class EmailPreferenceServiceTests
{
    [Fact]
    public static void Constructor_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(EmailPreferenceService).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task ShouldTriggerForUser_DefaultPreference_ReturnsExpected(
        AspNetUser user,
        EmailPreferenceType emailPreferenceType,
        [Frozen] BuyingCatalogueDbContext context,
        EmailPreferenceService service)
    {
        emailPreferenceType.RoleType = EmailPreferenceRoleType.All;

        context.UserEmailPreferences.RemoveRange(context.UserEmailPreferences);
        context.Add(emailPreferenceType);
        context.Add(user);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await service.ShouldTriggerForUser(emailPreferenceType, user.Id);

        result.Should().Be(emailPreferenceType.DefaultEnabled);
    }


    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task ShouldTriggerForUser_UserPreference_ReturnsExpected(
        AspNetUser user,
        EmailPreferenceType emailPreferenceType,
        [Frozen] BuyingCatalogueDbContext context,
        EmailPreferenceService service)
    {
        var expected = !emailPreferenceType.DefaultEnabled;

        emailPreferenceType.RoleType = EmailPreferenceRoleType.All;
        context.UserEmailPreferences.RemoveRange(context.UserEmailPreferences);

        context.UserEmailPreferences.Add(new()
        {
            EmailPreferenceTypeId = emailPreferenceType.Id, Enabled = expected, UserId = user.Id
        });

        context.Add(emailPreferenceType);
        context.Add(user);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await service.ShouldTriggerForUser(emailPreferenceType, user.Id);

        result.Should().Be(expected);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task ShouldTriggerForUser_MismatchedRole_ReturnsExpected(
        AspNetUser user,
        EmailPreferenceType emailPreferenceType,
        [Frozen] BuyingCatalogueDbContext context,
        EmailPreferenceService service)
    {
        var role = new AspNetRole
        {
            Name = OrganisationFunction.Buyer.Name,
            NormalizedName = OrganisationFunction.Buyer.Name.ToUpperInvariant()
        };

        context.Roles.Add(role);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        user.AspNetUserRoles = new List<AspNetUserRole> { new() { RoleId = role.Id } };

        emailPreferenceType.RoleType = EmailPreferenceRoleType.Admins;

        context.Add(emailPreferenceType);
        context.Add(user);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await service.ShouldTriggerForUser(emailPreferenceType, user.Id);

        result.Should().BeFalse();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetDefaultEmailPreferences_ReturnsExpected(
        EmailPreferenceTypeEnum emailPreferenceTypeEnum,
        EmailPreferenceType emailPreferenceType,
        [Frozen] BuyingCatalogueDbContext context,
        EmailPreferenceService service)
    {
        emailPreferenceType.Id = (int)emailPreferenceTypeEnum;

        context.EmailPreferenceTypes.Add(emailPreferenceType);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await service.GetDefaultEmailPreference(emailPreferenceTypeEnum);

        result.Should().BeEquivalentTo(emailPreferenceType, opt => opt.Excluding(x => x.UserPreferences));
    }
}
