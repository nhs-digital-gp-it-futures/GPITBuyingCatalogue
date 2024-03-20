using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
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
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(TermsOfUseController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_TermsOfUse_ReturnsViewAndModel(
            TermsOfUseController controller)
        {
            var result = (await controller.TermsOfUse()).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeAssignableTo<TermsOfUseModel>();
        }

        [Theory]
        [CommonAutoData]
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
        [CommonAutoData]
        public static async Task Post_TermsOfUse_Accepts(
            TermsOfUseModel model,
            AspNetUser user,
            [Frozen] Mock<UserManager<AspNetUser>> userManager,
            [Frozen] TermsOfUseSettings settings)
        {
            user.AcceptedTermsOfUseDate = null;
            user.HasOptedInUserResearch = false;

            model.HasAcceptedTermsOfUse
                = model.HasAcceptedPrivacyPolicy
                = model.HasOptedInUserResearch = true;

            userManager.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new(ClaimTypes.Role, "Buyer") },
                "mock"));

            var controller = new TermsOfUseController(userManager.Object, settings)
            {
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
            userManager.Verify(um => um.UpdateAsync(It.Is<AspNetUser>(u => u.HasOptedInUserResearch == true)));
        }
    }
}
