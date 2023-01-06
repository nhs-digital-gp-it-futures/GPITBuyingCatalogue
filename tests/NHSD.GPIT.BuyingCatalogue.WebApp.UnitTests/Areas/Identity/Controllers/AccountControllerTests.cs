using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using Xunit;
using static NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing.Constants;
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
                .Setup(x => x.IsPasswordValid(It.IsAny<AspNetUser>(), It.IsAny<string>()))
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
        public static void Get_ForgotPasswordLinkSent_ReturnsDefaultView(AccountController controller)
        {
            var result = controller.ForgotPasswordLinkSent();

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
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
            IUsersService userServices = null)
        {
            return new AccountController(
                signInManager,
                userManager,
                odsService ?? Mock.Of<IOdsService>(),
                userServices ?? Mock.Of<IUsersService>(),
                Mock.Of<IPasswordService>(),
                Mock.Of<IPasswordResetCallback>(),
                new DisabledErrorMessageSettings());
        }
    }
}
