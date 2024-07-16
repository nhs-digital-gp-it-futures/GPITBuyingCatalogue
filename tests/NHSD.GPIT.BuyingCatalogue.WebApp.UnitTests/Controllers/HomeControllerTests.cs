using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Controllers
{
    public static class HomeControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(HomeController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static void Get_Index_ReturnsDefaultView(
            HomeController controller)
        {
            var result = controller.Index().As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();
        }

        [Theory]
        [MockAutoData]
        public static void Get_PrivacyPolicy_ReturnsDefaultView(
            HomeController controller)
        {
            var result = controller.PrivacyPolicy().As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();
        }

        [Theory]
        [MockAutoData]
        public static void Get_Error500_ReturnsDefaultErrorView(
            HomeController controller)
        {
            var result = controller.Error(500).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();
        }

        [Theory]
        [MockAutoData]
        public static void Get_ErrorNullStatus_ReturnsDefaultErrorView(
            HomeController controller)
        {
            var result = controller.Error(null).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();
        }

        [Theory]
        [MockAutoData]
        public static void Get_Error404_ReturnsPageNotFound(
            [Frozen] IFeatureCollection features,
            HomeController controller)
        {
            features.Get<IStatusCodeReExecuteFeature>().Returns(new StatusCodeReExecuteFeature { OriginalPath = "BAD" });

            var result = controller.Error(404).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be("PageNotFound");
            result.ViewData.Should().Contain(d => string.Equals(d.Key, "BadUrl") && string.Equals(d.Value, "Incorrect url BAD"));
        }

        [Theory]
        [MockAutoData]
        public static void Get_Error404_NullFeature_ReturnsPageNotFound(
            [Frozen] HttpContext httpContextMock,
            HomeController controller)
        {
            httpContextMock.Features.Get<IStatusCodeReExecuteFeature>().Returns((StatusCodeReExecuteFeature)null);

            var result = controller.Error(404).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be("PageNotFound");
            result.ViewData.Should().Contain(d => string.Equals(d.Key, "BadUrl") && string.Equals(d.Value, "Incorrect url "));
        }

        [Theory]
        [MockAutoData]
        public static void Get_ErrorWithErrorValue_ReturnsErrorViewModel(
            string error,
            HomeController controller)
        {
            var expectedModel = new ErrorModel(error);

            var result = controller.Error(error: error).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.As<ErrorModel>().Should().BeEquivalentTo(expectedModel);
        }

        [Theory]
        [MockAutoData]
        public static void Get_ContactUs_ReturnsViewWithModel(
            HomeController controller)
            => controller
                .ContactUs()
                .As<ViewResult>()
                ?.Model
                ?.Should()
                .NotBeNull();

        [Theory]
        [MockAutoData]
        public static async Task Post_ContactUs_InvalidModel_ReturnsViewWithModel(
            ContactUsModel model,
            HomeController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.ContactUs(model)).As<ViewResult>();

            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_ContactUs_ValidModel_SubmitsQuery(
            ContactUsModel model,
            [Frozen] IContactUsService service,
            HomeController controller)
        {
            _ = await controller.ContactUs(model);

            await service.Received().SubmitQuery(
                model.FullName,
                model.EmailAddress,
                model.Message);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_ContactUs_ValidModel_RedirectsContactUsConfirmation(
            ContactUsModel model,
            HomeController controller)
        {
            var result = (await controller.ContactUs(model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.ContactUsConfirmation));
        }

        [Theory]
        [MockAutoData]
        public static void Get_ContactUsConfirmation_ReturnsView(
            HomeController controller)
        {
            var result = controller.ContactUsConfirmation().As<ViewResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static void Get_ContactUsConfirmation_ReturnsExpectedModel(
            HomeController controller)
        {
            var model = new ContactUsConfirmationModel();

            var result = controller.ContactUsConfirmation().As<ViewResult>();

            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static void Get_AccessibilityStatement_ReturnsDefaultView(
            HomeController controller)
        {
            var result = controller.AccessibilityStatement().As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();
        }

        [Theory]
        [MockAutoData]
        public static void Get_NotAuthorised_ReturnsDefaultView(
            HomeController controller)
        {
            var result = controller.NotAuthorized().As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();
        }
    }
}
