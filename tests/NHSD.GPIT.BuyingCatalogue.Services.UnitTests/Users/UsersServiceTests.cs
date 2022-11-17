﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.Services.Users;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.SharedMocks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Users
{
    public static class UsersServiceTests
    {
        [Fact]
        public static void Constructor_NullRepository_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new UsersService(null));
        }

        [Theory]
        [InMemoryDbAutoData]
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
        [InMemoryDbAutoData]
        public static async Task GetUser_NoMatchingRecord_ReturnsNull(
            UsersService service)
        {
            var actual = await service.GetUser(int.MaxValue);

            actual.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
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
        [InMemoryDbAutoData]
        public static async Task GetAllUsersForOrganisation_GetsCorrectUsersFromDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Organisation organisation,
            AspNetUser user1,
            AspNetUser user2,
            AspNetUser user3,
            UsersService service)
        {
            context.Organisations.Add(organisation);
            user1.PrimaryOrganisationId = organisation.Id;
            user2.PrimaryOrganisationId = organisation.Id;
            context.AspNetUsers.Add(user1);
            context.AspNetUsers.Add(user2);
            context.AspNetUsers.Add(user3);
            await context.SaveChangesAsync();

            var actual = await service.GetAllUsersForOrganisation(organisation.Id);

            actual.Count.Should().Be(2);
            actual[0].UserName.Should().Be(user1.UserName);
            actual[1].UserName.Should().Be(user2.UserName);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
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
        [InMemoryDbAutoData]
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
        [InMemoryDbAutoData]
        public static async Task EnableOrDisableUser_DisableUser_UpdatesDatabaseCorrectly(
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] UserManager<AspNetUser> userManager,
            Organisation organisation,
            AspNetUser user,
            UsersService service)
        {
            context.Organisations.Add(organisation);
            user.Disabled = false;
            user.PrimaryOrganisationId = organisation.Id;
            context.AspNetUsers.Add(user);
            await context.SaveChangesAsync();

            await service.EnableOrDisableUser(user.Id, true);

            var actual = await userManager.Users.FirstAsync(u => u.Id == user.Id);

            actual.Disabled.Should().BeTrue();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task EnableOrDisableUser_EnableUser_UpdatesDatabaseCorrectly(
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] UserManager<AspNetUser> userManager,
            Organisation organisation,
            AspNetUser user,
            UsersService service)
        {
            context.Organisations.Add(organisation);
            user.Disabled = true;
            user.PrimaryOrganisationId = organisation.Id;
            context.AspNetUsers.Add(user);
            await context.SaveChangesAsync();

            await service.EnableOrDisableUser(user.Id, false);

            var actual = await userManager.Users.FirstAsync(u => u.Id == user.Id);

            actual.Disabled.Should().BeFalse();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdateUserAccountType_UpdatesDatabaseCorrectly(
            string accountType,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] UserManager<AspNetUser> userManager,
            AspNetUser user,
            UsersService service)
        {
            context.Roles.Add(new() { Name = accountType, NormalizedName = accountType.ToUpperInvariant() });
            context.AspNetUsers.Add(user);
            await context.SaveChangesAsync();

            await service.UpdateUserAccountType(user.Id, accountType);

            var actual = await userManager.Users.Include(u => u.AspNetUserRoles)
                .ThenInclude(r => r.Role)
                .FirstAsync(u => u.Id == user.Id);

            actual.AspNetUserRoles.Select(u => u.Role).Should().Contain(x => x.Name == accountType);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdateUserDetails_UpdatesDatabaseCorrectly(
            string firstName,
            string lastName,
            string email,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] UserManager<AspNetUser> userManager,
            AspNetUser user,
            UsersService service)
        {
            context.AspNetUsers.Add(user);
            await context.SaveChangesAsync();

            await service.UpdateUserDetails(user.Id, firstName, lastName, email);

            var actual = await userManager.Users.FirstAsync(u => u.Id == user.Id);

            actual.FirstName.Should().Be(firstName);
            actual.LastName.Should().Be(lastName);
            actual.Email.Should().Be(email);
            actual.UserName.Should().Be(email);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdateUserOrganisation_UpdatesDatabaseCorrectly(
            int organisationId,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] UserManager<AspNetUser> userManager,
            AspNetUser user,
            UsersService service)
        {
            context.AspNetUsers.Add(user);
            await context.SaveChangesAsync();

            await service.UpdateUserOrganisation(user.Id, organisationId);

            var actual = await userManager.Users.FirstAsync(u => u.Id == user.Id);

            actual.PrimaryOrganisationId.Should().Be(organisationId);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdateUser_UpdatesDatabaseCorrectly(
            string firstName,
            string lastName,
            string email,
            string accountType,
            int organisationId,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] UserManager<AspNetUser> userManager,
            AspNetUser user,
            UsersService service)
        {
            user.Disabled = true;

            context.Roles.Add(new() { Name = accountType, NormalizedName = accountType.ToUpperInvariant() });
            context.AspNetUsers.Add(user);
            await context.SaveChangesAsync();

            await service.UpdateUser(user.Id, firstName, lastName, email, false, accountType, organisationId);

            var actual = await userManager.Users.Include(u => u.AspNetUserRoles)
                .ThenInclude(r => r.Role)
                .FirstAsync(u => u.Id == user.Id);

            actual.FirstName.Should().Be(firstName);
            actual.LastName.Should().Be(lastName);
            actual.Email.Should().Be(email);
            actual.UserName.Should().Be(email);
            actual.Disabled.Should().Be(false);
            actual.AspNetUserRoles.Select(u => u.Role).Should().Contain(x => x.Name == accountType);
            actual.PrimaryOrganisationId.Should().Be(organisationId);
        }

        [Theory]
        [InMemoryDbAutoData]
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
        [InMemoryDbAutoData]
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
        [InMemoryDbAutoData]
        public static async Task IsAccountManagerLimit_NoActiveAccountManagers_ReturnsFalse(
            int organisationId,
            [Frozen] BuyingCatalogueDbContext context,
            UsersService service)
        {
            context.AspNetUsers.RemoveRange(context.AspNetUsers);
            await context.SaveChangesAsync();

            var result = await service.IsAccountManagerLimit(organisationId);

            result.Should().BeFalse();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task IsAccountManagerLimit_OneActiveAccountManager_ReturnsFalse(
            int organisationId,
            [Frozen] BuyingCatalogueDbContext context,
            AspNetUser user,
            UsersService service)
        {
            context.AspNetUsers.RemoveRange(context.AspNetUsers);
            await AddAccountManagerToOrganisation(organisationId, context, user);

            var result = await service.IsAccountManagerLimit(organisationId);

            result.Should().BeFalse();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task IsAccountManagerLimit_TwoActiveAccountManagers_ReturnsTrue(
            int organisationId,
            [Frozen] BuyingCatalogueDbContext context,
            AspNetUser user,
            AspNetUser user2,
            UsersService service)
        {
            context.AspNetUsers.RemoveRange(context.AspNetUsers);
            await AddAccountManagerToOrganisation(organisationId, context, user);
            await AddAccountManagerToOrganisation(organisationId, context, user2);

            var result = await service.IsAccountManagerLimit(organisationId);

            result.Should().BeTrue();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task IsAccountManagerLimit_OneActiveOneInactiveAccountManagers_ReturnsFalse(
            int organisationId,
            [Frozen] BuyingCatalogueDbContext context,
            AspNetUser user,
            AspNetUser user2,
            UsersService service)
        {
            context.AspNetUsers.RemoveRange(context.AspNetUsers);
            await AddAccountManagerToOrganisation(organisationId, context, user);
            await AddAccountManagerToOrganisation(organisationId, context, user2, true);

            var result = await service.IsAccountManagerLimit(organisationId);

            result.Should().BeFalse();
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
