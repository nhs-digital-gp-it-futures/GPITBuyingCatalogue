using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.NominateOrganisation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Controllers
{
    public static class NominateOrganisationControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(NominateOrganisationController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(NominateOrganisationController).Should().BeDecoratedWith<RouteAttribute>(r => r.Template == "nominate-organisation");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(NominateOrganisationController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_ReturnsDefaultView(
            [Frozen] Mock<INominateOrganisationService> mockNominateOrganisationService,
            NominateOrganisationController systemUnderTest)
        {
            mockNominateOrganisationService
                .Setup(x => x.IsGpPractice(It.IsAny<int>()))
                .ReturnsAsync(false);

            var result = await systemUnderTest.Index();

            mockNominateOrganisationService.VerifyAll();
            Assert.Null(((ViewResult)result).ViewName);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_GP_Redirects(
            [Frozen] Mock<INominateOrganisationService> mockNominateOrganisationService,
            NominateOrganisationController systemUnderTest)
        {
            mockNominateOrganisationService
                .Setup(x => x.IsGpPractice(It.IsAny<int>()))
                .ReturnsAsync(true);

            var result = await systemUnderTest.Index() as RedirectToActionResult;

            mockNominateOrganisationService.VerifyAll();
            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(NominateOrganisationController.Unavailable));
        }

        [Theory]
        [CommonAutoData]
        public static void Get_Confirmation_ReturnsDefaultView(
            NominateOrganisationController systemUnderTest)
        {
            var result = systemUnderTest.Confirmation();

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_Unavailable_ReturnsDefaultView(
            NominateOrganisationController systemUnderTest)
        {
            var result = systemUnderTest.Unavailable() as ViewResult;

            result.Should().NotBeNull();
            result.Model.Should().NotBeNull();
            result.Model.Should().BeAssignableTo<NavBaseModel>();
            result.ViewName.Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void Get_Details_ReturnsDefaultView(
            NominateOrganisationController systemUnderTest)
        {
            var result = systemUnderTest.Details();

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
            Assert.IsAssignableFrom<NominateOrganisationDetailsModel>(((ViewResult)result).Model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Details_InvalidModelState_ReturnsDefaultView(
            NominateOrganisationController systemUnderTest)
        {
            systemUnderTest.ModelState.AddModelError("test", "test");

            var result = await systemUnderTest.Details(new NominateOrganisationDetailsModel());

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
            Assert.IsAssignableFrom<NominateOrganisationDetailsModel>(((ViewResult)result).Model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Details_ValidModelState_RedirectsToConfirmation(
            NominateOrganisationDetailsModel expected,
            [Frozen] Mock<INominateOrganisationService> mockNominateOrganisationService,
            NominateOrganisationController systemUnderTest)
        {
            NominateOrganisationRequest actual = null;

            mockNominateOrganisationService
                .Setup(x => x.NominateOrganisation(It.IsAny<int>(), It.IsAny<NominateOrganisationRequest>()))
                .Callback<int, NominateOrganisationRequest>((_, x) => actual = x)
                .Returns(Task.CompletedTask);

            var result = await systemUnderTest.Details(expected);

            mockNominateOrganisationService.VerifyAll();

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(NominateOrganisationController.Confirmation));

            actual.OrganisationName.Should().Be(expected.OrganisationName);
            actual.OdsCode.Should().Be(expected.OdsCode);
        }
    }
}
