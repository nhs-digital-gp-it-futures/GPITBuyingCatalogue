using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.AccountManagement.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.AccountManagement
{
    public static class ManageAccountControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(ManageAccountController).Should().BeDecoratedWith<AuthorizeAttribute>(a => a.Policy == "AccountManager");
            typeof(ManageAccountController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "AccountManagement");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ManageAccountController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_RedirectsToUserOrgDetails(
            string internalOrgId,
            Organisation organisation,
            [Frozen] Mock<IOrganisationsService> mockOrganisationService,
            ManageAccountController controller)
        {
            mockOrganisationService
                .Setup(o => o.GetOrganisationByInternalIdentifier(internalOrgId))
                .ReturnsAsync(organisation);

            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new("primaryOrganisationInternalIdentifier", internalOrgId) },
                "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userPrincipal, },
            };

            var result = await controller.Index();

            mockOrganisationService.VerifyAll();

            var actualResult = result.Should().BeAssignableTo<RedirectToActionResult>();
            actualResult.Subject.ActionName.Should().Be(nameof(ManageAccountController.Details));
            actualResult.Subject.RouteValues.Should().NotBeNull();
            actualResult.Subject.RouteValues.ContainsKey("organisationId");
            actualResult.Subject.RouteValues["organisationId"].Should().Be(organisation.Id);
        }
    }
}
