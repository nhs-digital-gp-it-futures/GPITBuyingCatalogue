using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Identity
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class UserClaimsPrincipalFactoryExTests
    {
        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new UserClaimsPrincipalFactoryEx<AspNetUser>(
                CreateDefaultMockUserManager(),
                Mock.Of<IOptions<IdentityOptions>>(),
                null));
        }

        private static UserManager<AspNetUser> CreateDefaultMockUserManager()
        {
            var mockUserManager = new Mock<UserManager<AspNetUser>>(
                Mock.Of<IUserStore<AspNetUser>>(),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            return mockUserManager.Object;
        }
    }
}
