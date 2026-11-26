using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Controllers
{
    public static class TermsOfUseControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(TermsOfUseController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_TermsOfUse_ReturnsViewAndModel(
            TermsOfUseController controller)
        {
            var result = (await controller.TermsOfUse()).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeAssignableTo<TermsOfUseModel>();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_TermsOfUse_InvalidModel_ReturnsViewAndModel(
            TermsOfUseModel model,
            TermsOfUseController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.TermsOfUse(model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_TermsOfUse_Accepts(
            TermsOfUseModel model,
            AspNetUser user,
            [Frozen] UserManager<AspNetUser> userManager,
            [Frozen] TermsOfUseSettings settings,
            IUrlHelper urlHelper)
        {
            user.AcceptedTermsOfUseDate = null;
            user.HasOptedInUserResearch = false;

            model.HasAcceptedTermsOfUse
                = model.HasAcceptedPrivacyPolicy
                = model.HasOptedInUserResearch = true;

            userManager.FindByIdAsync(Arg.Any<string>()).Returns(user);

            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new(ClaimTypes.Role, "Buyer") },
                "mock"));

            var controller = new TermsOfUseController(userManager, settings)
            {
                Url = urlHelper,
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = userPrincipal,
                    },
                },
            };

            var result = (await controller.TermsOfUse(model)).As<RedirectResult>();

            result.Should().NotBeNull();
            await userManager.Received().UpdateAsync(Arg.Is<AspNetUser>(u => u.HasOptedInUserResearch == true));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_TermsOfUse_Accepts_WithLocalReturnUrl(
            TermsOfUseModel model,
            AspNetUser user,
            [Frozen] UserManager<AspNetUser> userManager,
            [Frozen] TermsOfUseSettings settings,
            IUrlHelper urlHelper)
        {
            user.AcceptedTermsOfUseDate = null;
            user.HasOptedInUserResearch = false;

            model.HasAcceptedTermsOfUse
                = model.HasAcceptedPrivacyPolicy
                = model.HasOptedInUserResearch = true;
            model.ReturnUrl = "/orders/dashboard";

            urlHelper.IsLocalUrl(model.ReturnUrl).Returns(true);

            userManager.FindByIdAsync(Arg.Any<string>()).Returns(user);

            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new(ClaimTypes.Role, "Buyer") },
                "mock"));

            var controller = new TermsOfUseController(userManager, settings)
            {
                Url = urlHelper,
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = userPrincipal,
                    },
                },
            };

            var result = (await controller.TermsOfUse(model)).As<LocalRedirectResult>();

            result.Should().NotBeNull();
            result.Url.Should().Be(model.ReturnUrl);
            await userManager.Received().UpdateAsync(Arg.Is<AspNetUser>(u => u.HasOptedInUserResearch));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_TermsOfUse_Accepts_WithExternalReturnUrl_RedirectsHome(
            TermsOfUseModel model,
            AspNetUser user,
            [Frozen] UserManager<AspNetUser> userManager,
            [Frozen] TermsOfUseSettings settings,
            IUrlHelper urlHelper)
        {
            user.AcceptedTermsOfUseDate = null;

            model.HasAcceptedTermsOfUse = model.HasAcceptedPrivacyPolicy = true;
            model.ReturnUrl = "https://evil.example/phish";

            urlHelper.IsLocalUrl(model.ReturnUrl).Returns(false);

            userManager.FindByIdAsync(Arg.Any<string>()).Returns(user);

            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new(ClaimTypes.Role, "Buyer") },
                "mock"));

            var controller = new TermsOfUseController(userManager, settings)
            {
                Url = urlHelper,
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = userPrincipal },
                },
            };

            var result = (await controller.TermsOfUse(model)).As<RedirectResult>();

            result.Should().NotBeNull();
            result.Url.Should().Be("~/");
        }
    }
}
