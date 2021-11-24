using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Identity.Controllers
{
    public static class AccountControllerTests
    {
        [Fact]
        public static void Constructor_AllServicesPresent_Success()
        {
            _ = new AccountController(
                CreateDefaultMockSignInManager(),
                CreateDefaultMockUserManager(),
                Mock.Of<IPasswordService>(),
                Mock.Of<IPasswordResetCallback>(),
                new DisabledErrorMessageSettings());
        }

        [Fact]
        public static void Constructor_NullSignInManager_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AccountController(
                null,
                CreateDefaultMockUserManager(),
                Mock.Of<IPasswordService>(),
                Mock.Of<IPasswordResetCallback>(),
                new DisabledErrorMessageSettings()));
        }

        [Fact]
        public static void Constructor_NullUserManager_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AccountController(
                        CreateDefaultMockSignInManager(),
                        null,
                        Mock.Of<IPasswordService>(),
                        Mock.Of<IPasswordResetCallback>(),
                        new DisabledErrorMessageSettings()));
        }

        [Fact]
        public static void Constructor_NullPasswordService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AccountController(
                        CreateDefaultMockSignInManager(),
                        CreateDefaultMockUserManager(),
                        null,
                        Mock.Of<IPasswordResetCallback>(),
                        new DisabledErrorMessageSettings()));
        }

        [Fact]
        public static void Constructor_NullPasswordResetCallback_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AccountController(
                        CreateDefaultMockSignInManager(),
                        CreateDefaultMockUserManager(),
                        Mock.Of<IPasswordService>(),
                        null,
                        new DisabledErrorMessageSettings()));
        }

        [Fact]
        public static void Constructor_NullDisabledErrorSettings_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AccountController(
                    CreateDefaultMockSignInManager(),
                    CreateDefaultMockUserManager(),
                    Mock.Of<IPasswordService>(),
                    Mock.Of<IPasswordResetCallback>(),
                    null));
        }

        [Fact]
        public static void Get_Login_ReturnsDefaultViewWithReturnUrlSet()
        {
            var controller = CreateValidController();

            var result = controller.Login("ReturnLink");

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
            Assert.IsAssignableFrom<LoginViewModel>(((ViewResult)result).Model);
            Assert.Equal("ReturnLink", ((LoginViewModel)((ViewResult)result).Model).ReturnUrl);
        }

        [Fact]
        public static async Task Post_Login_InvalidModelState_ReturnsDefaultView()
        {
            var controller = CreateValidController();
            controller.ModelState.AddModelError("test", "test");

            var result = await controller.Login(new LoginViewModel());

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
            Assert.IsAssignableFrom<LoginViewModel>(((ViewResult)result).Model);
        }

        [Fact]
        public static async Task Get_Logout_WhenNotLoggedIn_RedirectsHome()
        {
            var controller = CreateValidController();

            var result = await controller.Logout();

            Assert.IsAssignableFrom<LocalRedirectResult>(result);
            Assert.Equal("~/", ((LocalRedirectResult)result).Url);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_Registration_ReturnsDefaultView(AccountController controller)
        {
            var result = controller.Registration();

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
        }

        [Fact]
        public static void Get_ForgotPassword_ReturnsDefaultView()
        {
            var controller = CreateValidController();

            var result = controller.ForgotPassword();

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
        }

        [Fact]
        public static async Task Post_ForgotPassword_InvalidModelState_ReturnsDefaultView()
        {
            var controller = CreateValidController();
            controller.ModelState.AddModelError("test", "test");

            var result = await controller.ForgotPassword(new ForgotPasswordViewModel());

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
            Assert.IsAssignableFrom<ForgotPasswordViewModel>(((ViewResult)result).Model);
        }

        [Fact]
        public static void Get_ForgotPasswordLinkSent_ReturnsDefaultView()
        {
            var controller = CreateValidController();

            var result = controller.ForgotPasswordLinkSent();

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
        }

        [Fact]
        public static async Task Post_ResetPassword_InvalidModelState_ReturnsDefaultView()
        {
            var controller = CreateValidController();
            controller.ModelState.AddModelError("test", "test");

            var result = await controller.ResetPassword(new ResetPasswordViewModel());

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
            Assert.IsAssignableFrom<ResetPasswordViewModel>(((ViewResult)result).Model);
        }

        [Fact]
        public static void Get_ResetPasswordConfirmation_ReturnsDefaultView()
        {
            var controller = CreateValidController();

            var result = controller.ResetPasswordConfirmation();

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
        }

        [Fact]
        public static void Get_ResetPasswordExpired_ReturnsDefaultView()
        {
            var controller = CreateValidController();

            var result = controller.ResetPasswordExpired();

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
        }

        private static AccountController CreateValidController()
        {
            return new(
                CreateDefaultMockSignInManager(),
                CreateDefaultMockUserManager(),
                Mock.Of<IPasswordService>(),
                Mock.Of<IPasswordResetCallback>(),
                new DisabledErrorMessageSettings());
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
