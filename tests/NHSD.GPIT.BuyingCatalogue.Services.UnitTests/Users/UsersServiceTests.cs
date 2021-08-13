using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
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
        [CommonAutoData]
        public static async Task GetUser_CallsSingleAsync_OnRepository(
            [Frozen] Mock<IDbRepository<AspNetUser, BuyingCatalogueDbContext>> dbRepositoryMock,
            Guid userId,
            UsersService service)
        {
            dbRepositoryMock.Setup(r => r.SingleAsync(It.IsAny<Expression<Func<AspNetUser, bool>>>()))
                .ReturnsAsync(new AspNetUser());

            await service.GetUser(userId);

            dbRepositoryMock.Verify(r => r.SingleAsync(It.IsAny<Expression<Func<AspNetUser, bool>>>()));
        }

        [Fact]
        public static async Task GetAllUsersForOrganisation_CallsGetAllAsync_OnRepository()
        {
            var users = new AspNetUser[]
            {
                new() { UserName = "One" },
                new() { UserName = "Two" },
            };

            var mockUsersRepository = new Mock<IDbRepository<AspNetUser, BuyingCatalogueDbContext>>();
            mockUsersRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<AspNetUser, bool>>>()))
                .ReturnsAsync(users);

            var service = new UsersService(
                mockUsersRepository.Object);

            await service.GetAllUsersForOrganisation(27);

            mockUsersRepository.Verify(x => x.GetAllAsync(It.IsAny<Expression<Func<AspNetUser, bool>>>()));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public static async Task EnableOrDisableUser_GetsUser_SetsDisabled_AndUpdates(bool enabled)
        {
            var user = new AspNetUser { Disabled = !enabled };

            var mockUsersRepository = new Mock<IDbRepository<AspNetUser, BuyingCatalogueDbContext>>();
            mockUsersRepository.Setup(r => r.SingleAsync(It.IsAny<Expression<Func<AspNetUser, bool>>>()))
                .ReturnsAsync(user);

            var service = new UsersService(
                mockUsersRepository.Object);

            await service.EnableOrDisableUser(Guid.NewGuid(), enabled);

            Assert.Equal(enabled, user.Disabled);
            mockUsersRepository.Verify(r => r.SingleAsync(It.IsAny<Expression<Func<AspNetUser, bool>>>()));
            mockUsersRepository.Verify(r => r.SaveChangesAsync());
        }
    }
}
