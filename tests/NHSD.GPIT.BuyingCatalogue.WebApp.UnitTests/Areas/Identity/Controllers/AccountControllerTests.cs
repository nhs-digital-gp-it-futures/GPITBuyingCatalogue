using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Identity.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class AccountControllerTests
    {
        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {            
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AccountController(null,
                CreateDefaultMockSignInManager(),
                CreateDefaultMockUserManager(),
                Mock.Of<IPasswordService>(),
                Mock.Of<IPasswordResetCallback>(),
                new DisabledErrorMessageSettings()
                ));
        }

        [Test]
        public static void Constructor_NullSignInManager_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AccountController(Mock.Of<ILogger<AccountController>>(),
                null,
                CreateDefaultMockUserManager(),
                Mock.Of<IPasswordService>(),
                Mock.Of<IPasswordResetCallback>(),
                new DisabledErrorMessageSettings()
                ));
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

        private static SignInManager<AspNetUser> CreateDefaultMockSignInManager()
        {
            var mockSignInManager = new Mock<SignInManager<AspNetUser>>(
                CreateDefaultMockUserManager(),
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<AspNetUser>>(),
                null,
                null,
                null,
                null);

            return mockSignInManager.Object;
        }
    }
}
