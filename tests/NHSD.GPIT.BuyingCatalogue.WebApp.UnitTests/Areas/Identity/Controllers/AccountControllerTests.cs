﻿using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models;
using Xunit;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Identity.Controllers
{
    public static class AccountControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(AccountController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Identity");
            typeof(AccountController).Should().BeDecoratedWith<RouteAttribute>(r => r.Template == "Identity/Account");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(AccountController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_Login_ReturnsDefaultViewWithReturnUrlSet(
            AccountController controller)
        {
            var result = controller.Login("ReturnLink");
            var actualResult = result.Should().BeAssignableTo<ViewResult>().Subject;

            actualResult.ViewName.Should().BeNull();

            var model = actualResult.Model.Should().BeAssignableTo<LoginViewModel>().Subject;

            model.ReturnUrl.Should().Be("ReturnLink");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Login_InvalidModelState_ReturnsDefaultView(
            AccountController controller)
        {
            controller.ModelState.AddModelError("test", "test");

            var result = await controller.Login(new LoginViewModel());
            var actualResult = result.Should().BeAssignableTo<ViewResult>().Subject;

            actualResult.ViewName.Should().BeNull();
            actualResult.Model.Should().BeAssignableTo<LoginViewModel>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Login_UserNotFound_ReturnsDefaultView(
            LoginViewModel model,
            Mock<UserManager<AspNetUser>> mockUserManager,
            Mock<SignInManager<AspNetUser>> mockSignInManager)
        {
            const string expectedErrorMessage = "The username or password were not recognised. Please try again.";

            mockUserManager
                .Setup(x => x.FindByNameAsync(model.EmailAddress))
                .ReturnsAsync((AspNetUser)null);

            var controller = CreateController(mockUserManager.Object, mockSignInManager.Object);

            var result = await controller.Login(model);

            mockUserManager.VerifyAll();
            mockSignInManager.VerifyAll();

            controller.ModelState.Should().ContainKey(nameof(model.EmailAddress));
            controller.ModelState[nameof(model.EmailAddress)]?.Errors.Should().Contain(x => x.ErrorMessage == expectedErrorMessage);

            controller.ModelState.Should().ContainKey(nameof(model.Password));
            controller.ModelState[nameof(model.Password)]?.Errors.Should().Contain(x => x.ErrorMessage == expectedErrorMessage);

            var actualResult = result.Should().BeAssignableTo<ViewResult>().Subject;

            actualResult.ViewName.Should().BeNull();
            actualResult.Model.Should().BeAssignableTo<LoginViewModel>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Login_FailedSignIn_ReturnsDefaultView(
            AspNetUser user,
            LoginViewModel model,
            Mock<UserManager<AspNetUser>> mockUserManager,
            Mock<SignInManager<AspNetUser>> mockSignInManager)
        {
            const string expectedErrorMessage = "The username or password were not recognised. Please try again.";

            user.Disabled = false;

            mockUserManager
                .Setup(x => x.FindByNameAsync(model.EmailAddress))
                .ReturnsAsync(user);

            mockSignInManager
                .Setup(x => x.PasswordSignInAsync(user, model.Password, false, true))
                .ReturnsAsync(SignInResult.Failed);

            var controller = CreateController(mockUserManager.Object, mockSignInManager.Object);

            var result = await controller.Login(model);

            mockUserManager.VerifyAll();
            mockSignInManager.VerifyAll();

            controller.ModelState.Should().ContainKey(nameof(model.EmailAddress));
            controller.ModelState[nameof(model.EmailAddress)]?.Errors.Should().Contain(x => x.ErrorMessage == expectedErrorMessage);

            controller.ModelState.Should().ContainKey(nameof(model.Password));
            controller.ModelState[nameof(model.Password)]?.Errors.Should().Contain(x => x.ErrorMessage == expectedErrorMessage);

            var actualResult = result.Should().BeAssignableTo<ViewResult>().Subject;

            actualResult.ViewName.Should().BeNull();
            actualResult.Model.Should().BeAssignableTo<LoginViewModel>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Login_UserAccountDisabled_ReturnsDefaultView(
            AspNetUser user,
            LoginViewModel model,
            Mock<UserManager<AspNetUser>> mockUserManager,
            Mock<SignInManager<AspNetUser>> mockSignInManager)
        {
            const string expectedErrorMessage = "There is a problem accessing your account.";

            user.Disabled = true;

            mockUserManager
                .Setup(x => x.FindByNameAsync(model.EmailAddress))
                .ReturnsAsync(user);

            var controller = CreateController(mockUserManager.Object, mockSignInManager.Object);

            var result = await controller.Login(model);

            mockUserManager.VerifyAll();

            controller.ModelState.Should().ContainKey(nameof(model.DisabledError));
            controller.ModelState[nameof(model.DisabledError)]?.Errors.Should().Contain(x => x.ErrorMessage.Contains(expectedErrorMessage));

            var actualResult = result.Should().BeAssignableTo<ViewResult>().Subject;

            actualResult.ViewName.Should().BeNull();
            actualResult.Model.Should().BeAssignableTo<LoginViewModel>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Login_UserAccountLocked_ReturnsLockedView(
            AspNetUser user,
            LoginViewModel model,
            Mock<UserManager<AspNetUser>> mockUserManager,
            Mock<SignInManager<AspNetUser>> mockSignInManager,
            Mock<IUrlHelper> mockUrlHelper)
        {
            user.Disabled = false;

            mockUserManager
                .Setup(x => x.FindByNameAsync(model.EmailAddress))
                .ReturnsAsync(user);

            mockSignInManager
                .Setup(x => x.PasswordSignInAsync(user, model.Password, false, true))
                .ReturnsAsync(SignInResult.LockedOut);

            var controller = CreateController(mockUserManager.Object, mockSignInManager.Object);

            controller.Url = mockUrlHelper.Object;

            var result = await controller.Login(model);

            mockUserManager.VerifyAll();
            mockSignInManager.VerifyAll();
            mockUrlHelper.VerifyAll();

            var actualResult = result.Should().BeAssignableTo<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(AccountController.LockedAccount));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Login_UserAccountActive_ReturnsRedirect(
            string odsCode,
            AspNetUser user,
            LoginViewModel model,
            Mock<UserManager<AspNetUser>> mockUserManager,
            Mock<SignInManager<AspNetUser>> mockSignInManager,
            Mock<IOdsService> mockOdsService)
        {
            user.Disabled = false;
            user.PrimaryOrganisation = new Organisation { ExternalIdentifier = odsCode };

            mockUserManager
                .Setup(x => x.FindByNameAsync(model.EmailAddress))
                .ReturnsAsync(user);

            mockSignInManager
                .Setup(x => x.PasswordSignInAsync(user, model.Password, false, true))
                .ReturnsAsync(SignInResult.Success);

            mockOdsService
                .Setup(x => x.UpdateOrganisationDetails(odsCode))
                .Verifiable();

            var controller = CreateController(mockUserManager.Object, mockSignInManager.Object, mockOdsService.Object);

            var result = await controller.Login(model);

            mockUserManager.VerifyAll();
            mockSignInManager.VerifyAll();
            mockOdsService.VerifyAll();

            var actualResult = result.Should().BeAssignableTo<RedirectResult>().Subject;

            actualResult.Url.Should().Be(model.ReturnUrl);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_LockedAccount_ReturnsDefaultView(AccountController controller)
        {
            var result = controller.LockedAccount();

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Logout_WhenNotLoggedIn_RedirectsHome(AccountController controller)
        {
            var result = await controller.Logout();

            Assert.IsAssignableFrom<LocalRedirectResult>(result);
            Assert.Equal("~/", ((LocalRedirectResult)result).Url);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_ForgotPassword_ReturnsDefaultView(AccountController controller)
        {
            var result = controller.ForgotPassword();

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ForgotPassword_InvalidModelState_ReturnsDefaultView(AccountController controller)
        {
            controller.ModelState.AddModelError("test", "test");

            var result = await controller.ForgotPassword(new ForgotPasswordViewModel());

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
            Assert.IsAssignableFrom<ForgotPasswordViewModel>(((ViewResult)result).Model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ForgotPassword_ValidModelState_ReturnsRedirect(
            PasswordResetToken token,
            Uri uri,
            ForgotPasswordViewModel model,
            Mock<UserManager<AspNetUser>> mockUserManager,
            Mock<SignInManager<AspNetUser>> mockSignInManager,
            Mock<IPasswordService> mockPasswordService,
            Mock<IPasswordResetCallback> mockPasswordResetCallback)
        {
            mockPasswordService
                .Setup(x => x.GeneratePasswordResetTokenAsync(model.EmailAddress)).ReturnsAsync(token)
                .Verifiable();

            mockPasswordResetCallback
                .Setup(x => x.GetPasswordResetCallback(token)).Returns(uri)
                .Verifiable();

            mockPasswordService
                .Setup(x => x.SendResetEmailAsync(token.User, uri)).Returns(Task.CompletedTask)
                .Verifiable();

            var controller = CreateController(mockUserManager.Object, mockSignInManager.Object, passwordService:mockPasswordService.Object, passwordResetCallback: mockPasswordResetCallback.Object);

            var result = await controller.ForgotPassword(model);

            mockPasswordService.VerifyAll();
            mockPasswordResetCallback.VerifyAll();

            var actualResult = result.Should().BeAssignableTo<RedirectToActionResult>();

            actualResult.Subject.ActionName.Should().Be(nameof(AccountController.ForgotPasswordLinkSent));
        }

        [Theory]
        [CommonAutoData]
        public static void Get_ForgotPasswordLinkSent_ReturnsDefaultView(AccountController controller)
        {
            var result = controller.ForgotPasswordLinkSent();

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
        }

        [Theory]
        [CommonAutoData]
        public static async void Get_ResetPassword_ReturnsDefaultView(
            string email,
            string token,
            Mock<UserManager<AspNetUser>> mockUserManager,
            Mock<SignInManager<AspNetUser>> mockSignInManager,
            Mock<IPasswordService> mockPasswordService)
        {
            mockPasswordService
                .Setup(x => x.IsValidPasswordResetTokenAsync(email, token)).ReturnsAsync(true)
                .Verifiable();

            var controller = CreateController(mockUserManager.Object, mockSignInManager.Object, passwordService: mockPasswordService.Object);

            var result = await controller.ResetPassword(email, token);

            var actualResult = result.Should().BeAssignableTo<ViewResult>().Subject;

            actualResult.ViewName.Should().BeNull();
            actualResult.Model.Should().BeAssignableTo<ResetPasswordViewModel>();
            Assert.NotNull(actualResult.Model);
            Assert.Equal(email, ((ResetPasswordViewModel)actualResult.Model).Email);
            Assert.Equal(token, ((ResetPasswordViewModel)actualResult.Model).Token);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ResetPassword_InvalidModelState_ReturnsDefaultView(AccountController controller)
        {
            controller.ModelState.AddModelError("test", "test");

            var result = await controller.ResetPassword(new ResetPasswordViewModel());

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
            Assert.IsAssignableFrom<ResetPasswordViewModel>(((ViewResult)result).Model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task ResetPassword_WithPreviouslyUsedPassword_ReturnsModelError(
            AspNetUser user,
            ResetPasswordViewModel model,
            Mock<UserManager<AspNetUser>> mockUserManager,
            Mock<SignInManager<AspNetUser>> mockSignInManager,
            Mock<IUsersService> mockIUsersService)
        {
            const string expectedErrorMessage = "Password was used previously. Enter a different password";
            user.Disabled = true;
            mockUserManager
                .Setup(x => x.FindByEmailAsync(model.Email))
                .ReturnsAsync(user);

            mockIUsersService
                .Setup(x => x.IsDuplicatePassword(It.IsAny<AspNetUser>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            var controller = CreateController(mockUserManager.Object, mockSignInManager.Object, null, mockIUsersService.Object);

            var result = await controller.ResetPassword(model);
            mockUserManager.VerifyAll();
            mockIUsersService.VerifyAll();
            controller.ModelState.Should().ContainKey(nameof(ResetPasswordViewModel.Password));
            controller.ModelState[nameof(ResetPasswordViewModel.Password)]?.Errors.Should().Contain(x => x.ErrorMessage.Contains(expectedErrorMessage));

            var actualResult = result.Should().BeAssignableTo<ViewResult>().Subject;

            actualResult.ViewName.Should().BeNull();
            actualResult.Model.Should().BeAssignableTo<ResetPasswordViewModel>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task PostResetPassword_InvalidPasswordError_ReturnsModelError(
            AspNetUser user,
            Mock<UserManager<AspNetUser>> mockUserManager,
            Mock<SignInManager<AspNetUser>> mockSignInManager,
            ResetPasswordViewModel model,
            Mock<IPasswordService> mockPasswordService,
            Mock<IUsersService> mockUsersService)
        {
            mockUserManager
                .Setup(x => x.FindByEmailAsync(model.Email))
                .ReturnsAsync(user);

            mockUsersService
                .Setup(x => x.IsDuplicatePassword(user, model.Password))
                .Returns(Task.FromResult(false));

            IdentityError[] expectedErrorMessages = new IdentityError[]
            {
                new IdentityError() { Code = PasswordValidator.InvalidPasswordCode, Description = PasswordValidator.PasswordConditionsNotMet },
            };

            mockPasswordService
                .Setup(x => x.ResetPasswordAsync(model.Email, model.Token, model.Password))
                .ReturnsAsync(IdentityResult.Failed(expectedErrorMessages));

            var controller = CreateController(
                mockUserManager.Object,
                mockSignInManager.Object,
                userServices: mockUsersService.Object,
                passwordService: mockPasswordService.Object);

            var result = await controller.ResetPassword(model);

            mockUserManager.VerifyAll();
            mockUsersService.VerifyAll();
            mockPasswordService.VerifyAll();
            controller.ModelState.Should().ContainKey(nameof(ResetPasswordViewModel.Password));
            controller.ModelState[nameof(ResetPasswordViewModel.Password)]?.Errors.Should().Contain(x => x.ErrorMessage.Contains(PasswordValidator.PasswordConditionsNotMet));

            var actualResult = result.Should().BeAssignableTo<ViewResult>().Subject;

            actualResult.ViewName.Should().BeNull();
            actualResult.Model.Should().BeAssignableTo<ResetPasswordViewModel>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task PostResetPassword_InvalidTokenError_ReturnsRedirectToAction(
            AspNetUser user,
            Mock<UserManager<AspNetUser>> mockUserManager,
            Mock<SignInManager<AspNetUser>> mockSignInManager,
            ResetPasswordViewModel model,
            Mock<IPasswordService> mockPasswordService,
            Mock<IUsersService> mockUsersService)
        {
            mockUserManager
                .Setup(x => x.FindByEmailAsync(model.Email))
                .ReturnsAsync(user);

            mockUsersService
                .Setup(x => x.IsDuplicatePassword(user, model.Password))
                .Returns(Task.FromResult(false));

            IdentityError[] expectedErrorMessages = new IdentityError[]
            {
                new IdentityError() { Code = IPasswordService.InvalidTokenCode },
            };

            mockPasswordService
                .Setup(x => x.ResetPasswordAsync(model.Email, model.Token, model.Password))
                .ReturnsAsync(IdentityResult.Failed(expectedErrorMessages));

            var controller = CreateController(
                mockUserManager.Object,
                mockSignInManager.Object,
                userServices: mockUsersService.Object,
                passwordService: mockPasswordService.Object);

            var result = await controller.ResetPassword(model);

            mockUserManager.VerifyAll();
            mockUsersService.VerifyAll();
            mockPasswordService.VerifyAll();

            var actualResult = result.Should().BeAssignableTo<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(AccountController.ResetPasswordExpired));
        }

        [Theory]
        [CommonAutoData]
        public static async Task PostResetPassword_ResetSuccess_ReturnsRedirectToAction(
            AspNetUser user,
            Mock<UserManager<AspNetUser>> mockUserManager,
            Mock<SignInManager<AspNetUser>> mockSignInManager,
            ResetPasswordViewModel model,
            Mock<IPasswordService> mockPasswordService,
            Mock<IUsersService> mockUsersService)
        {
            mockUserManager
                .Setup(x => x.FindByEmailAsync(model.Email))
                .ReturnsAsync(user);

            mockUsersService
                .Setup(x => x.IsDuplicatePassword(user, model.Password))
                .Returns(Task.FromResult(false));

            mockPasswordService
                .Setup(x => x.ResetPasswordAsync(model.Email, model.Token, model.Password))
                .ReturnsAsync(IdentityResult.Success);

            var controller = CreateController(
                mockUserManager.Object,
                mockSignInManager.Object,
                userServices: mockUsersService.Object,
                passwordService: mockPasswordService.Object);

            var result = await controller.ResetPassword(model);

            mockUserManager.VerifyAll();
            mockUsersService.VerifyAll();
            mockPasswordService.VerifyAll();

            var actualResult = result.Should().BeAssignableTo<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(AccountController.ResetPasswordConfirmation));
        }

        [Theory]
        [CommonAutoData]
        public static void UpdatePassword_UnexpectedErrors_ThrowsException(
            AspNetUser user,
            Mock<UserManager<AspNetUser>> mockUserManager,
            Mock<SignInManager<AspNetUser>> mockSignInManager,
            ResetPasswordViewModel model,
            Mock<IPasswordService> mockPasswordService,
            Mock<IUsersService> mockUsersService)
        {
            const string code = "Code";
            const string errorMessage = "Error";

            mockUserManager
                .Setup(x => x.FindByEmailAsync(model.Email))
                .ReturnsAsync(user);

            mockUsersService
                .Setup(x => x.IsDuplicatePassword(user, model.Password))
                .Returns(Task.FromResult(false));

            IdentityError[] expectedErrorMessages = new IdentityError[] { new IdentityError() { Code = code, Description = errorMessage }, };

            mockPasswordService
                .Setup(x => x.ResetPasswordAsync(model.Email, model.Token, model.Password))
                .ReturnsAsync(IdentityResult.Failed(expectedErrorMessages));

            var controller = CreateController(
                mockUserManager.Object,
                mockSignInManager.Object,
                userServices: mockUsersService.Object,
                passwordService: mockPasswordService.Object);

            var result = Assert.ThrowsAsync<InvalidOperationException>(async () => await controller.ResetPassword(model));
            result.Result.Message.Should().Be("Unexpected errors whilst resetting password: " + errorMessage);

            mockUserManager.VerifyAll();
            mockUsersService.VerifyAll();
            mockPasswordService.VerifyAll();
        }

        [Theory]
        [CommonAutoData]
        public static void Get_ResetPasswordConfirmation_ReturnsDefaultView(AccountController controller)
        {
            var result = controller.ResetPasswordConfirmation();

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_ResetPasswordExpired_ReturnsDefaultView(AccountController controller)
        {
            var result = controller.ResetPasswordExpired();

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
        }

        private static AccountController CreateController(
            UserManager<AspNetUser> userManager,
            SignInManager<AspNetUser> signInManager,
            IOdsService odsService = null,
            IUsersService userServices = null,
            IPasswordService passwordService = null,
            IPasswordResetCallback passwordResetCallback = null)
        {
            return new AccountController(
                signInManager,
                userManager,
                odsService ?? Mock.Of<IOdsService>(),
                userServices ?? Mock.Of<IUsersService>(),
                passwordService ?? Mock.Of<IPasswordService>(),
                passwordResetCallback ?? Mock.Of<IPasswordResetCallback>(),
                new DisabledErrorMessageSettings());
        }
    }
}
