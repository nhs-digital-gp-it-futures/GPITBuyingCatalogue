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
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ProcurementHubController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_ReturnsDefaultView(
            ProcurementHubController systemUnderTest)
        {
            var result = await systemUnderTest.Index();

            var actualResult = result.Should().BeAssignableTo<ViewResult>().Subject;

            actualResult.ViewName.Should().BeNull();
            actualResult.Model.Should().BeAssignableTo<ProcurementHubDetailsModel>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_UserLoggedIn_ReturnsPopulatedDefaultView(
            AspNetUser user,
            Organisation organisation,
            [Frozen] Mock<IUsersService> mockUsersService,
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            ProcurementHubController systemUnderTest)
        {
            const int userId = 1;
            const int organisationId = 1;

            user.PrimaryOrganisationId = organisationId;

            mockUsersService
                .Setup(x => x.GetUser(userId))
                .ReturnsAsync(user);

            mockOrganisationsService
                .Setup(x => x.GetOrganisation(organisationId))
                .ReturnsAsync(organisation);

            var result = await systemUnderTest.Index();

            mockUsersService.VerifyAll();
            mockOrganisationsService.VerifyAll();

            var actualResult = result.Should().BeAssignableTo<ViewResult>().Subject;

            actualResult.ViewName.Should().BeNull();

            var model = actualResult.Model.Should().BeAssignableTo<ProcurementHubDetailsModel>().Subject;

            model.FullName.Should().Be(user.FullName);
            model.EmailAddress.Should().Be(user.Email);
            model.OrganisationName.Should().Be(organisation.Name);
            model.OdsCode.Should().Be(organisation.ExternalIdentifier);
        }

        [Theory]
        [CommonAutoData]
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
        [CommonAutoData]
        public static async Task Post_Index_ValidModelState_RedirectsToConfirmation(
            ProcurementHubDetailsModel expected,
            [Frozen] Mock<IProcurementHubService> mockProcurementHubService,
            ProcurementHubController systemUnderTest)
        {
            ProcurementHubRequest actual = null;

            mockProcurementHubService
                .Setup(x => x.ContactProcurementHub(It.IsAny<ProcurementHubRequest>()))
                .Callback<ProcurementHubRequest>(x => actual = x)
                .Returns(Task.CompletedTask);

            var result = await systemUnderTest.Index(expected);

            mockProcurementHubService.VerifyAll();

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(ProcurementHubController.Confirmation));

            actual.FullName.Should().Be(expected.FullName);
            actual.Email.Should().Be(expected.EmailAddress);
            actual.OrganisationName.Should().Be(expected.OrganisationName);
            actual.OdsCode.Should().Be(expected.OdsCode);
            actual.Query.Should().Be(expected.Query);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_Confirmation_ReturnsDefaultView(
            ProcurementHubController systemUnderTest)
        {
            var result = systemUnderTest.Confirmation();

            var actualResult = result.Should().BeAssignableTo<ViewResult>().Subject;

            actualResult.ViewName.Should().BeNull();
        }
    }
}
