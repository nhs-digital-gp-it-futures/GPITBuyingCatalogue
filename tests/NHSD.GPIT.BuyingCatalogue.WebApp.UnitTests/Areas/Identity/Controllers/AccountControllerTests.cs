using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Identity.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class AccountControllerTests
    {
        [Test]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(AccountController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Identity");
        }

        #region Constructor Tests

        [Test]
        public static void Constructor_AllServicesPresent_Success()
        {
            new AccountController(
                Mock.Of<ILogWrapper<AccountController>>(),
                CreateDefaultMockSignInManager(),
                CreateDefaultMockUserManager(),
                Mock.Of<IPasswordService>(),
                Mock.Of<IPasswordResetCallback>(),
                new DisabledErrorMessageSettings()
            );
        }

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
                _ = new AccountController(Mock.Of<ILogWrapper<AccountController>>(),
                null,
                CreateDefaultMockUserManager(),
                Mock.Of<IPasswordService>(),
                Mock.Of<IPasswordResetCallback>(),
                new DisabledErrorMessageSettings()
                ));
        }

        [Test]
        public static void Constructor_NullUserManager_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AccountController(
                    Mock.Of<ILogWrapper<AccountController>>(),
                    CreateDefaultMockSignInManager(),
                    null,
                    Mock.Of<IPasswordService>(),
                    Mock.Of<IPasswordResetCallback>(),
                    new DisabledErrorMessageSettings()
                ));
        }

        [Test]
        public static void Constructor_NullPasswordService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AccountController(
                    Mock.Of<ILogWrapper<AccountController>>(),
                    CreateDefaultMockSignInManager(),
                    CreateDefaultMockUserManager(),
                    null,
                    Mock.Of<IPasswordResetCallback>(),
                    new DisabledErrorMessageSettings()
                ));
        }

        [Test]
        public static void Constructor_NullPasswordResetCallback_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AccountController(
                    Mock.Of<ILogWrapper<AccountController>>(),
                    CreateDefaultMockSignInManager(),
                    CreateDefaultMockUserManager(),
                    Mock.Of<IPasswordService>(),
                    null,
                    new DisabledErrorMessageSettings()
                ));
        }

        [Test]
        public static void Constructor_NullDisabledErrorSettings_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AccountController(
                    Mock.Of<ILogWrapper<AccountController>>(),
                    CreateDefaultMockSignInManager(),
                    CreateDefaultMockUserManager(),
                    Mock.Of<IPasswordService>(),
                    Mock.Of<IPasswordResetCallback>(),
                    null
                ));
        }

        #endregion Constructor Tests

        [Test]
        public static void Get_Login_ReturnsDefaultViewWithReturnUrlSet()
        {
            var controller = CreateValidController();

            var result = controller.Login("ReturnLink");

            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.IsNull(((ViewResult)result).ViewName);
            Assert.That(((ViewResult)result).Model, Is.InstanceOf(typeof(LoginViewModel)));
            Assert.AreEqual("ReturnLink", ((LoginViewModel)((ViewResult)result).Model).ReturnUrl);
        }

        [Test]
        public static void Post_Login_NullModel_ThrowsException()
        {
            var controller = CreateValidController();

            Assert.ThrowsAsync<ArgumentNullException>(() =>
              controller.Login((LoginViewModel)null)
             );
        }

        [Test]
        public static async Task Post_Login_InvalidModelState_ReturnsDefaultView()
        {
            var controller = CreateValidController();
            controller.ModelState.AddModelError("test", "test");

            var result = await controller.Login(new LoginViewModel());

            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.IsNull(((ViewResult)result).ViewName);
            Assert.That(((ViewResult)result).Model, Is.InstanceOf(typeof(LoginViewModel)));
        }

        [Test]
        public static async Task Get_Logout_WhenNotLoggedIn_RedirectsHome()
        {
            var controller = CreateValidController();

            var result = await controller.Logout();

            Assert.That(result, Is.InstanceOf(typeof(LocalRedirectResult)));
            Assert.AreEqual("~/", ((LocalRedirectResult)result).Url);
        }

        [Test]
        public static void Get_Registration_ReturnsDefaultView()
        {
            var controller = CreateValidController();

            var result = controller.Registration();

            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.IsNull(((ViewResult)result).ViewName);
        }

        [Test]
        public static void Get_ForgotPassword_ReturnsDefaultView()
        {
            var controller = CreateValidController();

            var result = controller.ForgotPassword();

            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.IsNull(((ViewResult)result).ViewName);
        }

        [Test]
        public static void Post_ForgotPassword_NullModel_ThrowsException()
        {
            var controller = CreateValidController();

            Assert.ThrowsAsync<ArgumentNullException>(() =>
              controller.ForgotPassword((ForgotPasswordViewModel)null)
             );
        }

        [Test]
        public static async Task Post_ForgotPassword_InvalidModelState_ReturnsDefaultView()
        {
            var controller = CreateValidController();
            controller.ModelState.AddModelError("test", "test");

            var result = await controller.ForgotPassword(new ForgotPasswordViewModel());

            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.IsNull(((ViewResult)result).ViewName);
            Assert.That(((ViewResult)result).Model, Is.InstanceOf(typeof(ForgotPasswordViewModel)));
        }

        [Test]
        public static void Get_ForgotPasswordLinkSent_ReturnsDefaultView()
        {
            var controller = CreateValidController();

            var result = controller.ForgotPasswordLinkSent();

            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.IsNull(((ViewResult)result).ViewName);
        }

        [Test]
        public static void Post_ResetPassword_NullModel_ThrowsException()
        {
            var controller = CreateValidController();

            Assert.ThrowsAsync<ArgumentNullException>(() =>
              controller.ResetPassword((ResetPasswordViewModel)null)
             );
        }

        [Test]
        public static async Task Post_ResetPassword_InvalidModelState_ReturnsDefaultView()
        {
            var controller = CreateValidController();
            controller.ModelState.AddModelError("test", "test");

            var result = await controller.ResetPassword(new ResetPasswordViewModel());

            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.IsNull(((ViewResult)result).ViewName);
            Assert.That(((ViewResult)result).Model, Is.InstanceOf(typeof(ResetPasswordViewModel)));
        }

        [Test]
        public static void Get_ResetPasswordConfirmation_ReturnsDefaultView()
        {
            var controller = CreateValidController();

            var result = controller.ResetPasswordConfirmation();

            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.IsNull(((ViewResult)result).ViewName);
        }

        [Test]
        public static void Get_ResetPasswordExpired_ReturnsDefaultView()
        {
            var controller = CreateValidController();

            var result = controller.ResetPasswordExpired();

            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.IsNull(((ViewResult)result).ViewName);
        }

        private static AccountController CreateValidController()
        {
            return new AccountController(
                Mock.Of<ILogWrapper<AccountController>>(),
                CreateDefaultMockSignInManager(),
                CreateDefaultMockUserManager(),
                Mock.Of<IPasswordService>(),
                Mock.Of<IPasswordResetCallback>(),
                new DisabledErrorMessageSettings()
            );
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
