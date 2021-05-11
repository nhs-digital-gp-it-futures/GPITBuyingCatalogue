using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.Services.Users;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Users
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class UsersServiceTests
    {
        private static string[] InvalidStrings = { null, string.Empty, "    " };

        [Test]
        public static void Constructor_NullLogger_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new UsersService(
                null,
                Mock.Of<IUsersDbRepository<AspNetUser>>()));
        }

        [Test]
        public static void Constructor_NullRepository_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new UsersService(
                Mock.Of<ILogWrapper<UsersService>>(),
                null));
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void GetUser_InvalidUserId_ThrowsException(string userId)
        {
            var service = new UsersService(Mock.Of<ILogWrapper<UsersService>>(),
                Mock.Of<IUsersDbRepository<AspNetUser>>());

            var actual = Assert.ThrowsAsync<ArgumentException>(() => service.GetUser(userId));

            actual.ParamName.Should().Be("userId");
        }

        [Test]
        public static async Task GetUser_CallsSingleAsync_OnRepository()
        {
            var mockUsersRepository = new Mock<IUsersDbRepository<AspNetUser>>();
            mockUsersRepository.Setup(x => x.SingleAsync(It.IsAny<Expression<Func<AspNetUser, bool>>>()))
                .ReturnsAsync(new AspNetUser());

            var service = new UsersService(Mock.Of<ILogWrapper<UsersService>>(),
                mockUsersRepository.Object);

            await service.GetUser("123");

            mockUsersRepository.Verify(x => x.SingleAsync(It.IsAny<Expression<Func<AspNetUser, bool>>>()));
        }

        [Test]        
        public static void GetAllUsersForOrganisation_InvalidId_ThrowsException()
        {
            var service = new UsersService(Mock.Of<ILogWrapper<UsersService>>(),
                Mock.Of<IUsersDbRepository<AspNetUser>>());

            var actual = Assert.ThrowsAsync<ArgumentException>(() => service.GetAllUsersForOrganisation(Guid.Empty));

            actual.ParamName.Should().Be("organisationId");
        }

        [Test]
        public static async Task GetAllUsersForOrganisation_CallsGetAllAsync_OnRepository()
        {
            var users = new AspNetUser[]
            {
                new AspNetUser{UserName = "One" },
                new AspNetUser{UserName = "Two" },
            };

            var mockUsersRepository = new Mock<IUsersDbRepository<AspNetUser>>();
            mockUsersRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<AspNetUser, bool>>>()))
                .ReturnsAsync(users);

            var service = new UsersService(Mock.Of<ILogWrapper<UsersService>>(),
                mockUsersRepository.Object);

            await service.GetAllUsersForOrganisation(Guid.NewGuid());

            mockUsersRepository.Verify(x => x.GetAllAsync(It.IsAny<Expression<Func<AspNetUser, bool>>>()));
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void EnableOrDisableUser_InvalidUserId_ThrowsException(string userId)
        {
            var service = new UsersService(Mock.Of<ILogWrapper<UsersService>>(),
                Mock.Of<IUsersDbRepository<AspNetUser>>());

            var actual = Assert.ThrowsAsync<ArgumentException>(() => service.EnableOrDisableUser(userId, true));

            actual.ParamName.Should().Be("userId");
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async static Task EnableOrDisableUser_GetsUser_SetsDisabled_AndUpdates(bool enabled)
        {
            var user = new AspNetUser { Disabled = !enabled };

            var mockUsersRepository = new Mock<IUsersDbRepository<AspNetUser>>();
            mockUsersRepository.Setup(x => x.SingleAsync(It.IsAny<Expression<Func<AspNetUser, bool>>>()))
                .ReturnsAsync(user);

            var service = new UsersService(Mock.Of<ILogWrapper<UsersService>>(),
                mockUsersRepository.Object);

            await service.EnableOrDisableUser("123", enabled);

            Assert.AreEqual(enabled, user.Disabled);
            mockUsersRepository.Verify(x => x.SingleAsync(It.IsAny<Expression<Func<AspNetUser, bool>>>()));
            mockUsersRepository.Verify(x => x.SaveChangesAsync());
        }
    }
}
