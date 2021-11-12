using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.DevelopmentPlans;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DevelopmentPlans;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class DevelopmentPlansControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(DevelopmentPlansController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DevelopmentPlans_ValidId_ReturnsViewWithExpectedModel(
            Solution solution,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            DevelopmentPlansController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var expected = new DevelopmentPlanModel(solution.CatalogueItem)
            {
                BackLink = "testUrl",
            };

            var actual = (await controller.DevelopmentPlans(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DevelopmentPlans_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            DevelopmentPlansController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.DevelopmentPlans(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DevelopmentPlans_CallsSavesSolutionDevelopmentPlans(
            CatalogueItemId catalogueItemId,
            DevelopmentPlanModel model,
            [Frozen] Mock<IDevelopmentPlansService> mockService,
            DevelopmentPlansController controller)
        {
            await controller.DevelopmentPlans(catalogueItemId, model);

            mockService.Verify(s => s.SaveDevelopmentPlans(catalogueItemId, model.Link));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DevelopmentPlans_RedirectsToManageCatalogueSolution(
            CatalogueItemId catalogueItemId,
            DevelopmentPlanModel model,
            DevelopmentPlansController controller)
        {
            var actual = (await controller.DevelopmentPlans(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DevelopmentPlans_InvalidId_ReturnsBadRequestResult(
            Solution solution,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            DevelopmentPlansController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var expected = new DevelopmentPlanModel(solution.CatalogueItem)
            {
                BackLink = "testUrl",
            };

            var actual = (await controller.DevelopmentPlans(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(expected);
        }
    }
}
