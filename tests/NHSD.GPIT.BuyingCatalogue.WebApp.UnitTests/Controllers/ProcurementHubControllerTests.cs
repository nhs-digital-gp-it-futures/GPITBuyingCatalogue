using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ProcurementHub;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.ProcurementHub;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Controllers
{
    public static class ProcurementHubControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(ProcurementHubController).Should().BeDecoratedWith<RouteAttribute>(r => r.Template == "contact-procurement-hub");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ProcurementHubController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_ReturnsDefaultView(
            ProcurementHubController systemUnderTest)
        {
            var result = await systemUnderTest.Index();

            var actualResult = result.Should().BeAssignableTo<ViewResult>().Subject;

            actualResult.ViewName.Should().BeNull();
            actualResult.Model.Should().BeAssignableTo<ProcurementHubDetailsModel>();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_UserLoggedIn_ReturnsPopulatedDefaultView(
            AspNetUser user,
            Organisation organisation,
            [Frozen] IUsersService mockUsersService,
            [Frozen] IOrganisationsService mockOrganisationsService,
            ProcurementHubController systemUnderTest)
        {
            const int userId = 1;
            const int organisationId = 1;

            user.PrimaryOrganisationId = organisationId;

            mockUsersService.GetUser(userId).Returns(user);

            mockOrganisationsService.GetOrganisation(organisationId).Returns(organisation);

            var result = await systemUnderTest.Index();

            var actualResult = result.Should().BeAssignableTo<ViewResult>().Subject;

            actualResult.ViewName.Should().BeNull();

            var model = actualResult.Model.Should().BeAssignableTo<ProcurementHubDetailsModel>().Subject;

            model.FullName.Should().Be(user.FullName);
            model.EmailAddress.Should().Be(user.Email);
            model.OrganisationName.Should().Be(organisation.Name);
            model.OdsCode.Should().Be(organisation.ExternalIdentifier);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Index_InvalidModelState_ReturnsDefaultView(
            ProcurementHubController systemUnderTest)
        {
            systemUnderTest.ModelState.AddModelError("test", "test");

            var result = await systemUnderTest.Index(new ProcurementHubDetailsModel());

            var actualResult = result.Should().BeAssignableTo<ViewResult>().Subject;

            actualResult.ViewName.Should().BeNull();
            actualResult.Model.Should().BeAssignableTo<ProcurementHubDetailsModel>();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Index_ValidModelState_RedirectsToConfirmation(
            ProcurementHubDetailsModel expected,
            [Frozen] IProcurementHubService mockProcurementHubService,
            ProcurementHubController systemUnderTest)
        {
            ProcurementHubRequest actual = null;

            mockProcurementHubService
                .When(x => x.ContactProcurementHub(Arg.Any<ProcurementHubRequest>()))
                .Do(x => actual = x.Arg<ProcurementHubRequest>());

            var result = await systemUnderTest.Index(expected);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(ProcurementHubController.Confirmation));

            actual.FullName.Should().Be(expected.FullName);
            actual.Email.Should().Be(expected.EmailAddress);
            actual.OrganisationName.Should().Be(expected.OrganisationName);
            actual.OdsCode.Should().Be(expected.OdsCode);
            actual.Query.Should().Be(expected.Query);
        }

        [Theory]
        [MockAutoData]
        public static void Get_Confirmation_ReturnsDefaultView(
            ProcurementHubController systemUnderTest)
        {
            var result = systemUnderTest.Confirmation();

            var actualResult = result.Should().BeAssignableTo<ViewResult>().Subject;

            actualResult.ViewName.Should().BeNull();
        }
    }
}
