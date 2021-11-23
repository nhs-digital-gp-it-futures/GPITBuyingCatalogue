using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Users;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Users
{
    public static class UsersServiceTests
    {
        [Fact]
        public static void Constructor_NullRepository_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new UsersService(
                null));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetUser_GetsUserFromDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            AspNetUser user,
            UsersService service)
        {
            context.AspNetUsers.Add(user);
            await context.SaveChangesAsync();

            var actual = await service.GetUser(user.Id);

            actual.Should().BeEquivalentTo(user);
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
    }
}
