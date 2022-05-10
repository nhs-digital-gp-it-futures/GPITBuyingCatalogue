using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Users;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
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
                .Should().ThrowAsync<ArgumentNullException>();
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

            var actual = await context.AspNetUsers.SingleAsync(u => u.Id == user.Id);

            actual.Disabled.Should().BeTrue();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task EnableOrDisableUser_EnableUser_UpdatesDatabaseCorrectly(
            [Frozen] BuyingCatalogueDbContext context,
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

            var actual = await context.AspNetUsers.SingleAsync(u => u.Id == user.Id);

            actual.Disabled.Should().BeFalse();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdateUserAccountType_UpdatesDatabaseCorrectly(
            string accountType,
            [Frozen] BuyingCatalogueDbContext context,
            AspNetUser user,
            UsersService service)
        {
            context.AspNetUsers.Add(user);
            await context.SaveChangesAsync();

            await service.UpdateUserAccountType(user.Id, accountType);

            var actual = await context.AspNetUsers.SingleAsync(u => u.Id == user.Id);

            actual.OrganisationFunction.Should().Be(accountType);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdateUserDetails_UpdatesDatabaseCorrectly(
            string firstName,
            string lastName,
            string email,
            [Frozen] BuyingCatalogueDbContext context,
            AspNetUser user,
            UsersService service)
        {
            context.AspNetUsers.Add(user);
            await context.SaveChangesAsync();

            await service.UpdateUserDetails(user.Id, firstName, lastName, email);

            var actual = await context.AspNetUsers.SingleAsync(u => u.Id == user.Id);

            actual.FirstName.Should().Be(firstName);
            actual.LastName.Should().Be(lastName);
            actual.Email.Should().Be(email);
            actual.NormalizedEmail.Should().Be(email.ToUpperInvariant());
            actual.NormalizedUserName.Should().Be(email.ToUpperInvariant());
            actual.UserName.Should().Be(email);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdateUserOrganisation_UpdatesDatabaseCorrectly(
            int organisationId,
            [Frozen] BuyingCatalogueDbContext context,
            AspNetUser user,
            UsersService service)
        {
            context.AspNetUsers.Add(user);
            await context.SaveChangesAsync();

            await service.UpdateUserOrganisation(user.Id, organisationId);

            var actual = await context.AspNetUsers.SingleAsync(u => u.Id == user.Id);

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
    }
}
