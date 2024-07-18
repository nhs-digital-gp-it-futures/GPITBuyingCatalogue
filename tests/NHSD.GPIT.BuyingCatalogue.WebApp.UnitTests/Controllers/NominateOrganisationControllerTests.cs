using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
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
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(NominateOrganisationController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_ReturnsDefaultView(
            [Frozen] INominateOrganisationService nominateOrganisationService,
            NominateOrganisationController systemUnderTest)
        {
            nominateOrganisationService.IsGpPractice(Arg.Any<int>()).Returns(false);

            var result = await systemUnderTest.Index();

            await nominateOrganisationService.Received().IsGpPractice(Arg.Any<int>());
            Assert.Null(((ViewResult)result).ViewName);
            Assert.IsAssignableFrom<NominateOrganisationDetailsModel>(((ViewResult)result).Model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_GP_Redirects(
            [Frozen] INominateOrganisationService nominateOrganisationService,
            NominateOrganisationController systemUnderTest)
        {
            nominateOrganisationService.IsGpPractice(Arg.Any<int>()).Returns(true);

            var result = await systemUnderTest.Index() as RedirectToActionResult;

            await nominateOrganisationService.Received().IsGpPractice(Arg.Any<int>());
            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(NominateOrganisationController.Unavailable));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Index_InvalidModelState_ReturnsDefaultView(
            NominateOrganisationController systemUnderTest)
        {
            systemUnderTest.ModelState.AddModelError("test", "test");

            var result = await systemUnderTest.Index(new NominateOrganisationDetailsModel());

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
            Assert.IsAssignableFrom<NominateOrganisationDetailsModel>(((ViewResult)result).Model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Index_ValidModelState_RedirectsToConfirmation(
            NominateOrganisationDetailsModel expected,
            [Frozen] INominateOrganisationService nominateOrganisationService,
            NominateOrganisationController systemUnderTest)
        {
            NominateOrganisationRequest actual = null;

            nominateOrganisationService.NominateOrganisation(Arg.Any<int>(), Arg.Any<NominateOrganisationRequest>())
                    .Returns(Task.CompletedTask)
                    .AndDoes(callInfo =>
                    {
                        actual = callInfo.Arg<NominateOrganisationRequest>();
                    });
            var result = await systemUnderTest.Index(expected);

            await nominateOrganisationService.Received().NominateOrganisation(Arg.Any<int>(), Arg.Any<NominateOrganisationRequest>());

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(NominateOrganisationController.Confirmation));

            actual.OrganisationName.Should().Be(expected.OrganisationName);
            actual.OdsCode.Should().Be(expected.OdsCode);
        }

        [Theory]
        [MockAutoData]
        public static void Get_Confirmation_ReturnsDefaultView(
            NominateOrganisationController systemUnderTest)
        {
            var result = systemUnderTest.Confirmation();

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
        }

        [Theory]
        [MockAutoData]
        public static void Get_Unavailable_ReturnsDefaultView(
            NominateOrganisationController systemUnderTest)
        {
            var result = systemUnderTest.Unavailable() as ViewResult;

            result.Should().NotBeNull();
            result.Model.Should().NotBeNull();
            result.Model.Should().BeAssignableTo<NavBaseModel>();
            result.ViewName.Should().BeNull();
        }
    }
}
