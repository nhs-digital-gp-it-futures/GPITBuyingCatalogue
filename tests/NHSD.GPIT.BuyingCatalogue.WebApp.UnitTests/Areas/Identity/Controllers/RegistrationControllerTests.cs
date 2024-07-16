using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models.Registration;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Identity.Controllers
{
    public static class RegistrationControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(RegistrationController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Identity");
            typeof(RegistrationController).Should().BeDecoratedWith<RouteAttribute>(r => r.Template == "registration");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(RegistrationController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static void Get_Index_ReturnsDefaultView(
            RegistrationController systemUnderTest)
        {
            var result = systemUnderTest.Index();

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
        }

        [Theory]
        [MockAutoData]
        public static void Get_Confirmation_ReturnsDefaultView(
            RegistrationController systemUnderTest)
        {
            var result = systemUnderTest.Confirmation();

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
        }

        [Theory]
        [MockAutoData]
        public static void Get_Details_ReturnsDefaultView(
            RegistrationController systemUnderTest)
        {
            var result = systemUnderTest.Details();

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
            Assert.IsAssignableFrom<RegistrationDetailsModel>(((ViewResult)result).Model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Details_InvalidModelState_ReturnsDefaultView(
            RegistrationController systemUnderTest)
        {
            systemUnderTest.ModelState.AddModelError("test", "test");

            var result = await systemUnderTest.Details(new RegistrationDetailsModel());

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
            Assert.IsAssignableFrom<RegistrationDetailsModel>(((ViewResult)result).Model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Details_ValidModelState_RedirectsToConfirmation(
            RegistrationDetailsModel expected,
            [Frozen] IRequestAccountService mockRequestAccountService,
            RegistrationController systemUnderTest)
        {
            NewAccountDetails actual = null;

            mockRequestAccountService
                .When(x => x.RequestAccount(Arg.Any<NewAccountDetails>()))
                .Do(x => actual = x.Arg<NewAccountDetails>());

            var result = await systemUnderTest.Details(expected);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(RegistrationController.Confirmation));

            actual.FullName.Should().Be(expected.FullName);
            actual.EmailAddress.Should().Be(expected.EmailAddress);
            actual.OrganisationName.Should().Be(expected.OrganisationName);
            actual.OdsCode.Should().Be(expected.OdsCode);
            actual.HasGivenUserResearchConsent.Should().Be(expected.HasGivenUserResearchConsent);
        }
    }
}
