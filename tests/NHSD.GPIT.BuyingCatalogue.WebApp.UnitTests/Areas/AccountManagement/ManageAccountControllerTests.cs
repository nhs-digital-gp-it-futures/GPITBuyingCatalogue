using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.AccountManagement.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.AccountManagement
{
    public static class ManageAccountControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ManageAccountController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_RedirectsToUserOrgDetails(
            string internalOrgId,
            Organisation organisation,
            [Frozen] IOrganisationsService mockOrganisationService,
            ManageAccountController controller)
        {
            mockOrganisationService.GetOrganisationByInternalIdentifier(internalOrgId)
                .Returns(organisation);

            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new("primaryOrganisationInternalIdentifier", internalOrgId) },
                "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userPrincipal, },
            };

            var result = await controller.Index();

            var actualResult = result.Should().BeAssignableTo<RedirectToActionResult>();
            actualResult.Subject.ActionName.Should().Be(nameof(ManageAccountController.Details));
            actualResult.Subject.RouteValues.Should().NotBeNull();
            actualResult.Subject.RouteValues.ContainsKey("organisationId");
            actualResult.Subject.RouteValues["organisationId"].Should().Be(organisation.Id);
        }
    }
}
