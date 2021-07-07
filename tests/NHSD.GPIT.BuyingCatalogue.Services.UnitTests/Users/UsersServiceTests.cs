using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Users;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.TestData;
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
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static async Task GetUser_InvalidUserId_ThrowsException(string userId)
        {
            var service = new UsersService(
                Mock.Of<IDbRepository<AspNetUser, GPITBuyingCatalogueDbContext>>());

            var actual = await Assert.ThrowsAsync<ArgumentException>(() => service.GetUser(userId));

            actual.ParamName.Should().Be("userId");
        }

        [Fact]
        public static async Task GetUser_CallsSingleAsync_OnRepository()
        {
            var mockUsersRepository = new Mock<IDbRepository<AspNetUser, GPITBuyingCatalogueDbContext>>();
            mockUsersRepository.Setup(x => x.SingleAsync(It.IsAny<Expression<Func<AspNetUser, bool>>>()))
                .ReturnsAsync(new AspNetUser());

            var service = new UsersService(
                mockUsersRepository.Object);

            await service.GetUser("123");

            mockUsersRepository.Verify(x => x.SingleAsync(It.IsAny<Expression<Func<AspNetUser, bool>>>()));
        }

        [Fact]
        public static async Task GetAllUsersForOrganisation_InvalidId_ThrowsException()
        {
            var service = new UsersService(
                Mock.Of<IDbRepository<AspNetUser, GPITBuyingCatalogueDbContext>>());

            var actual = await Assert.ThrowsAsync<ArgumentException>(() => service.GetAllUsersForOrganisation(Guid.Empty));

            actual.ParamName.Should().Be("organisationId");
        }

        [Fact]
        public static async Task GetAllUsersForOrganisation_CallsGetAllAsync_OnRepository()
        {
            var users = new AspNetUser[]
            {
                new() { UserName = "One" },
                new() { UserName = "Two" },
            };

            var mockUsersRepository = new Mock<IDbRepository<AspNetUser, GPITBuyingCatalogueDbContext>>();
            mockUsersRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<AspNetUser, bool>>>()))
                .ReturnsAsync(users);

            var service = new UsersService(
                mockUsersRepository.Object);

            await service.GetAllUsersForOrganisation(Guid.NewGuid());

            mockUsersRepository.Verify(x => x.GetAllAsync(It.IsAny<Expression<Func<AspNetUser, bool>>>()));
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static async Task EnableOrDisableUser_InvalidUserId_ThrowsException(string userId)
        {
            var service = new UsersService(
                Mock.Of<IDbRepository<AspNetUser, GPITBuyingCatalogueDbContext>>());

            var actual = await Assert.ThrowsAsync<ArgumentException>(() => service.EnableOrDisableUser(userId, true));

            actual.ParamName.Should().Be("userId");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public static async Task EnableOrDisableUser_GetsUser_SetsDisabled_AndUpdates(bool enabled)
        {
            var user = new AspNetUser { Disabled = !enabled };

            var mockUsersRepository = new Mock<IDbRepository<AspNetUser, GPITBuyingCatalogueDbContext>>();
            mockUsersRepository.Setup(x => x.SingleAsync(It.IsAny<Expression<Func<AspNetUser, bool>>>()))
                .ReturnsAsync(user);

            var service = new UsersService(
                mockUsersRepository.Object);

            await service.EnableOrDisableUser("123", enabled);

            Assert.Equal(enabled, user.Disabled);
            mockUsersRepository.Verify(x => x.SingleAsync(It.IsAny<Expression<Func<AspNetUser, bool>>>()));
            mockUsersRepository.Verify(x => x.SaveChangesAsync());
        }
    }
}
