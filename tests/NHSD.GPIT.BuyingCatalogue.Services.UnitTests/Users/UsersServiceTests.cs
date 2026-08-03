using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.Services.Email;
using NHSD.GPIT.BuyingCatalogue.Services.Users;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Users
{
    public static class UsersServiceTests
    {
        private const int MaxNumberOfAccountManagers = 1;

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(UsersService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetUser_GetsUserFromDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            AspNetUser user,
            Organisation organisation,
            UsersService service)
        {
            user.PrimaryOrganisationId = organisation.Id;

            context.AspNetUsers.Add(user);
            context.Organisations.Add(organisation);
            await context.SaveChangesAsync();

            var actual = await service.GetUser(user.Id);

            actual.Should().BeEquivalentTo(user);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetUser_NoMatchingRecord_ReturnsNull(
            UsersService service)
        {
            var actual = await service.GetUser(int.MaxValue);

            actual.Should().BeNull();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetAllUsers_GetsCorrectUsersFromDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Organisation organisation,
            List<AspNetUser> users,
            UsersService service)
        {
            users.ForEach(x => x.PrimaryOrganisationId = organisation.Id);

            context.Organisations.Add(organisation);
            context.AspNetUsers.AddRange(users);
            await context.SaveChangesAsync();

            var actual = await service.GetAllUsers();

            actual.Should().BeEquivalentTo(users);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetAllUsers_GetsCorrectUsers_OrderedByStatus_FromDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Organisation organisation,
            AspNetUser user1,
            AspNetUser user2,
            AspNetUser user3,
            UsersService service)
        {
            context.Organisations.Add(organisation);
            user1.PrimaryOrganisationId = organisation.Id;
            user1.Disabled = true;
            user1.LastName = "Adams";
            user2.PrimaryOrganisationId = organisation.Id;
            user2.Disabled = false;
            user2.LastName = "Smith";
            user3.PrimaryOrganisationId = organisation.Id;
            user3.Disabled = false;
            user3.LastName = "Brown";

            context.AspNetUsers.Add(user1);
            context.AspNetUsers.Add(user2);
            context.AspNetUsers.Add(user3);
            await context.SaveChangesAsync();

            var actual = await service.GetAllUsers();

            actual.Count.Should().Be(3);
            actual[0].UserName.Should().Be(user3.UserName);
            actual[1].UserName.Should().Be(user2.UserName);
            actual[2].UserName.Should().Be(user1.UserName);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetAllUsersForOrganisation_GetsCorrectUsers_OrderedByStatus_FromDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Organisation organisation,
            AspNetUser user1,
            AspNetUser user2,
            AspNetUser user3,
            UsersService service)
        {
            context.Organisations.Add(organisation);
            user1.PrimaryOrganisationId = organisation.Id;
            user1.Disabled = true;
            user2.PrimaryOrganisationId = organisation.Id;
            user2.Disabled = false;
            context.AspNetUsers.Add(user1);
            context.AspNetUsers.Add(user2);
            context.AspNetUsers.Add(user3);
            await context.SaveChangesAsync();

            var actual = await service.GetAllUsersForOrganisation(organisation.Id);

            actual.Count.Should().Be(2);
            actual[0].UserName.Should().Be(user2.UserName);
            actual[1].UserName.Should().Be(user1.UserName);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static void GetAllUsersBySearchTerm_NullSearchTerm_GetsCorrectUsersFromDatabase(
            string searchTerm,
            UsersService service)
        {
            FluentActions
                .Awaiting(() => service.GetAllUsersBySearchTerm(searchTerm))
                .Should()
                .ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetAllUsersBySearchTerm_GetsCorrectUsersFromDatabase(
            string searchTerm,
            [Frozen] BuyingCatalogueDbContext context,
            Organisation organisation,
            AspNetUser noMatchUser,
            List<AspNetUser> matchUsers,
            UsersService service)
        {
            noMatchUser.PrimaryOrganisationId = organisation.Id;
            matchUsers.ForEach(x => x.PrimaryOrganisationId = organisation.Id);

            matchUsers[0].FirstName = searchTerm;
            matchUsers[1].LastName = searchTerm;
            matchUsers[2].Email = $"{searchTerm}@x.com";

            context.Organisations.Add(organisation);
            context.AspNetUsers.Add(noMatchUser);
            context.AspNetUsers.AddRange(matchUsers);
            await context.SaveChangesAsync();

            var actual = await service.GetAllUsersBySearchTerm(searchTerm);

            actual.Should().BeEquivalentTo(matchUsers);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task UpdateUser_UpdatesDatabaseCorrectly(
            string accountType,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] UserManager<AspNetUser> userManager,
            AspNetUser user,
            UsersService service)
        {
            user.Disabled = false;

            context.Roles.Add(new() { Name = accountType, NormalizedName = accountType.ToUpperInvariant() });
            context.AspNetUsers.Add(user);
            await context.SaveChangesAsync();

            await service.UpdateUser(new UpdateUserRequest()
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Disabled = false,
                OrganisationFunction = accountType,
                OrganisationId = user.PrimaryOrganisationId,
                ReactivationDate = null,
            });

            var actual = await userManager.Users.Include(u => u.AspNetUserRoles)
                .ThenInclude(r => r.Role)
                .FirstAsync(u => u.Id == user.Id);

            actual.FirstName.Should().Be(user.FirstName);
            actual.LastName.Should().Be(user.LastName);
            actual.Email.Should().Be(user.Email);
            actual.UserName.Should().Be(user.Email);
            actual.Disabled.Should().Be(false);
            actual.DeactivationReason.Should().BeNull();
            actual.AspNetUserRoles.Select(u => u.Role).Should().Contain(x => x.Name == accountType);
            actual.PrimaryOrganisationId.Should().Be(user.PrimaryOrganisationId);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task UpdateUser_DisableAccount_UpdatesDatabaseCorrectly(
            string accountType,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] UserManager<AspNetUser> userManager,
            AspNetUser user,
            UsersService service)
        {
            user.Disabled = false;

            context.Roles.Add(new() { Name = accountType, NormalizedName = accountType.ToUpperInvariant() });
            context.AspNetUsers.Add(user);
            await context.SaveChangesAsync();

            await service.UpdateUser(new UpdateUserRequest()
                {
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Disabled = true,
                    OrganisationFunction = accountType,
                    OrganisationId = user.PrimaryOrganisationId,
                    ReactivationDate = null,
                });

            var actual = await userManager.Users.Include(u => u.AspNetUserRoles)
                .ThenInclude(r => r.Role)
                .FirstAsync(u => u.Id == user.Id);

            actual.FirstName.Should().Be(user.FirstName);
            actual.LastName.Should().Be(user.LastName);
            actual.Email.Should().Be(user.Email);
            actual.UserName.Should().Be(user.Email);
            actual.Disabled.Should().Be(true);
            actual.DeactivationReason.Should().Be(AccountDeactivationReason.Manual);
            actual.AspNetUserRoles.Select(u => u.Role).Should().Contain(x => x.Name == accountType);
            actual.PrimaryOrganisationId.Should().Be(user.PrimaryOrganisationId);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task UpdateUser_ReactivatingAccount_UpdatesDatabaseCorrectly(
            string accountType,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] UserManager<AspNetUser> userManager,
            [Frozen] AccountTemplateSettings accountTemplateSettings,
            [Frozen] IGovNotifyEmailService govNotifyEmailService,
            AspNetUser user,
            UsersService service)
        {
            var reactivationDate = DateTime.UtcNow;
            accountTemplateSettings.AccountReactivationTemplateId = "TestTemplateId";
            user.Disabled = true;

            context.Roles.Add(new() { Name = accountType, NormalizedName = accountType.ToUpperInvariant() });
            context.AspNetUsers.Add(user);
            await context.SaveChangesAsync();

            await service.UpdateUser(new UpdateUserRequest()
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Disabled = false,
                OrganisationFunction = accountType,
                OrganisationId = user.PrimaryOrganisationId,
                ReactivationDate = reactivationDate,
            });

            await govNotifyEmailService.Received().SendEmailAsync(
                user.Email,
                accountTemplateSettings.AccountReactivationTemplateId,
                null);

            var actual = await userManager.Users.Include(u => u.AspNetUserRoles)
                .ThenInclude(r => r.Role)
                .FirstAsync(u => u.Id == user.Id);

            actual.FirstName.Should().Be(user.FirstName);
            actual.LastName.Should().Be(user.LastName);
            actual.Email.Should().Be(user.Email);
            actual.UserName.Should().Be(user.Email);
            actual.Disabled.Should().Be(false);
            actual.DeactivationReason.Should().Be(null);
            actual.AspNetUserRoles.Select(u => u.Role).Should().Contain(x => x.Name == accountType);
            actual.PrimaryOrganisationId.Should().Be(user.PrimaryOrganisationId);
            actual.ReactivationDate.Should().Be(reactivationDate);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task EmailAddressExists_ReturnsCorrectResult(
            string emailAddress,
            [Frozen] BuyingCatalogueDbContext context,
            AspNetUser user,
            UsersService service)
        {
            user.NormalizedEmail = emailAddress.ToUpperInvariant();
            context.AspNetUsers.Add(user);
            await context.SaveChangesAsync();

            var result = await service.EmailAddressExists(emailAddress);

            result.Should().BeTrue();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task EmailAddressExists_ForCurrentUser_ReturnsCorrectResult(
            string emailAddress,
            [Frozen] BuyingCatalogueDbContext context,
            AspNetUser user,
            UsersService service)
        {
            user.NormalizedEmail = emailAddress.ToUpperInvariant();
            context.AspNetUsers.Add(user);
            await context.SaveChangesAsync();

            var result = await service.EmailAddressExists(emailAddress, user.Id);

            result.Should().BeFalse();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task IsAccountManagerLimit_LessThanLimit_ReturnsFalse(
            int organisationId,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] AccountManagementSettings accountManagerSettings,
            UsersService service)
        {
            accountManagerSettings.MaximumNumberOfAccountManagers = MaxNumberOfAccountManagers;

            context.AspNetUsers.RemoveRange(context.AspNetUsers);
            await context.SaveChangesAsync();

            var result = await service.IsAccountManagerLimit(organisationId);

            result.Should().BeFalse();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task IsAccountManagerLimit_EqualsLimit_ReturnsTrue(
            int organisationId,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] AccountManagementSettings accountManagerSettings,
            AspNetUser user,
            UsersService service)
        {
            context.AspNetUsers.RemoveRange(context.AspNetUsers);
            await AddAccountManagerToOrganisation(organisationId, context, user);

            accountManagerSettings.MaximumNumberOfAccountManagers = MaxNumberOfAccountManagers;

            var result = await service.IsAccountManagerLimit(organisationId);

            result.Should().BeTrue();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task IsAccountManagerLimit_OverLimit_ReturnsTrue(
            int organisationId,
            [Frozen] BuyingCatalogueDbContext context,
            AspNetUser user,
            AspNetUser user2,
            [Frozen] AccountManagementSettings accountManagerSettings,
            UsersService service)
        {
            context.AspNetUsers.RemoveRange(context.AspNetUsers);

            accountManagerSettings.MaximumNumberOfAccountManagers = 1;
            await AddAccountManagerToOrganisation(organisationId, context, user);
            await AddAccountManagerToOrganisation(organisationId, context, user2);

            var result = await service.IsAccountManagerLimit(organisationId);

            result.Should().BeTrue();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task IsAccountManagerLimit_InactiveLimit_ReturnsFalse(
            int organisationId,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] AccountManagementSettings accountManagerSettings,
            AspNetUser user,
            UsersService service)
        {
            context.AspNetUsers.RemoveRange(context.AspNetUsers);
            accountManagerSettings.MaximumNumberOfAccountManagers = 1;

            await AddAccountManagerToOrganisation(organisationId, context, user, true);

            var result = await service.IsAccountManagerLimit(organisationId);

            result.Should().BeFalse();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task IsAccountManagerLimit_ForCurrentUser_ReturnsFalse(
            int organisationId,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] AccountManagementSettings accountManagerSettings,
            AspNetUser user,
            UsersService service)
        {
            context.AspNetUsers.RemoveRange(context.AspNetUsers);
            accountManagerSettings.MaximumNumberOfAccountManagers = 1;

            await AddAccountManagerToOrganisation(organisationId, context, user);

            var result = await service.IsAccountManagerLimit(organisationId, user.Id);

            result.Should().BeFalse();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task IsAccountManagerLimit_EqualsLimit_DifferentOrg_ReturnsFalse(
            int accountManagerOrgId,
            int testOrgId,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] AccountManagementSettings accountManagerSettings,
            AspNetUser user,
            UsersService service)
        {
            context.AspNetUsers.RemoveRange(context.AspNetUsers);
            await AddAccountManagerToOrganisation(accountManagerOrgId, context, user);

            accountManagerSettings.MaximumNumberOfAccountManagers = MaxNumberOfAccountManagers;

            var result = await service.IsAccountManagerLimit(testOrgId);

            result.Should().BeFalse();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SendDeactivatedUserEmail_SendsCorrectEmailTemplate(
            AspNetUser user,
            [Frozen] AccountTemplateSettings accountTemplateSettings,
            [Frozen] IGovNotifyEmailService govNotifyEmailService,
            UsersService service)
        {
            accountTemplateSettings.AccountDeactivationTemplateId = "TestTemplateId";

            await service.SendDeactivatedUserEmail(user.Email);
            await govNotifyEmailService.Received().SendEmailAsync(
                user.Email,
                accountTemplateSettings.AccountDeactivationTemplateId,
                null);
        }

        private static async Task AddAccountManagerToOrganisation(
            int organisationId,
            BuyingCatalogueDbContext context,
            AspNetUser user,
            bool disabled = false)
        {
            user.PrimaryOrganisationId = organisationId;
            user.Disabled = disabled;
            user.AspNetUserRoles.Add(
                new()
                {
                    Role = new()
                    {
                        Name = OrganisationFunction.AccountManager.Name,
                        NormalizedName = OrganisationFunction.AccountManager.Name.ToUpperInvariant(),
                    },
                });
            context.AspNetUsers.Add(user);
            await context.SaveChangesAsync();
        }
    }
}
