using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.DevelopmentPlans;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.DevelopmentPlans;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
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
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(DevelopmentPlansController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DevelopmentPlans_ValidId_ReturnsViewWithExpectedModel(
            Solution solution,
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            DevelopmentPlansController controller)
        {
            mockService.GetSolutionWithWorkOffPlans(catalogueItemId).Returns(solution.CatalogueItem);

            var expected = new DevelopmentPlanModel(solution.CatalogueItem)
            {
                BackLink = "testUrl",
            };

            var actual = (await controller.DevelopmentPlans(catalogueItemId)).As<ViewResult>();

            await mockService.Received().GetSolutionWithWorkOffPlans(catalogueItemId);
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DevelopmentPlans_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            DevelopmentPlansController controller)
        {
            mockService.GetSolutionWithWorkOffPlans(catalogueItemId).Returns(default(CatalogueItem));

            var actual = (await controller.DevelopmentPlans(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DevelopmentPlans_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            DevelopmentPlanModel model,
            [Frozen] ISolutionsService mockService,
            DevelopmentPlansController controller)
        {
            mockService.GetSolutionWithWorkOffPlans(catalogueItemId).Returns(default(CatalogueItem));

            var actual = (await controller.DevelopmentPlans(catalogueItemId, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DevelopmentPlans_CallsSavesSolutionDevelopmentPlans(
            Solution solution,
            DevelopmentPlanModel model,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IDevelopmentPlansService developmentPlansService,
            DevelopmentPlansController controller)
        {
            solutionsService.GetSolutionWithWorkOffPlans(solution.CatalogueItemId).Returns(solution.CatalogueItem);
            await controller.DevelopmentPlans(solution.CatalogueItemId, model);

            await developmentPlansService.Received().SaveDevelopmentPlans(solution.CatalogueItemId, model.Link);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DevelopmentPlans_RedirectsToManageCatalogueSolution(
            Solution solution,
            DevelopmentPlanModel model,
            [Frozen] ISolutionsService solutionsService,
            DevelopmentPlansController controller)
        {
            solutionsService.GetSolutionWithWorkOffPlans(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var actual = (await controller.DevelopmentPlans(solution.CatalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
            actual.RouteValues["solutionId"].Should().Be(solution.CatalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DevelopmentPlans_InvalidId_ExpectedViewModel(
            Solution solution,
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            DevelopmentPlansController controller)
        {
            mockService.GetSolutionWithWorkOffPlans(catalogueItemId).Returns(solution.CatalogueItem);

            var expected = new DevelopmentPlanModel(solution.CatalogueItem);

            var actual = (await controller.DevelopmentPlans(catalogueItemId)).As<ViewResult>();

            await mockService.Received().GetSolutionWithWorkOffPlans(catalogueItemId);

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(expected, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddWorkOffPlan_InvalidId_ReturnsBadRequestResult(
            [Frozen] ISolutionsService mockService,
            DevelopmentPlansController controller,
            CatalogueItemId id)
        {
            mockService.GetSolutionWithWorkOffPlans(id).Returns(default(CatalogueItem));

            var actual = (await controller.DevelopmentPlans(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Solution found for Id: {id}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddWorkOffPlan_ValidSolutionForId_ReturnsExpectedViewResult(
            [Frozen] ISolutionsService mockService,
            DevelopmentPlansController controller,
            Solution solution,
            List<Standard> standards)
        {
            var expectedModel = new EditWorkOffPlanModel(solution.CatalogueItem, standards);

            mockService.GetSolutionWithWorkOffPlans(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            mockService.GetSolutionStandardsForEditing(solution.CatalogueItemId).Returns(standards);

            var actual = (await controller.AddWorkOffPlan(solution.CatalogueItemId)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().NotBeNull();
            actual.ViewName.Should().Be("EditWorkOffPlan");

            actual.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddWorkOffPlan_InvalidId_ReturnsBadRequestResult(
            [Frozen] ISolutionsService mockService,
            DevelopmentPlansController controller,
            EditWorkOffPlanModel model,
            CatalogueItemId id)
        {
            mockService.GetSolutionWithWorkOffPlans(id).Returns(default(CatalogueItem));

            var actual = (await controller.AddWorkOffPlan(id, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Solution found for Id: {id}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddWorkOffPlan_ValidIdForSolution_CallsSaveWorkOffPlans(
            [Frozen] ISolutionsService mockService,
            [Frozen] IDevelopmentPlansService mockDPService,
            DevelopmentPlansController controller,
            Solution solution)
        {
            mockService.GetSolutionWithWorkOffPlans(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var model = new EditWorkOffPlanModel()
            {
                SolutionId = solution.CatalogueItemId,
                SelectedStandard = "Standard",
                Details = "Details",
                Day = "1",
                Month = "1",
                Year = "2111",
            };

            await controller.AddWorkOffPlan(solution.CatalogueItemId, model);

            await mockDPService.Received().SaveWorkOffPlan(solution.CatalogueItemId, Arg.Any<SaveWorkOffPlanModel>());
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddWorkOffPlan_ValidIdForSolution_RedirectsToDevelopmentPlans(
            [Frozen] ISolutionsService mockService,
            DevelopmentPlansController controller,
            Solution solution)
        {
            mockService.GetSolutionWithWorkOffPlans(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var model = new EditWorkOffPlanModel()
            {
                SolutionId = solution.CatalogueItemId,
                SelectedStandard = "Standard",
                Details = "Details",
                Day = "1",
                Month = "1",
                Year = "2111",
            };

            var actual = (await controller.AddWorkOffPlan(solution.CatalogueItemId, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(DevelopmentPlansController.DevelopmentPlans));
            actual.RouteValues["solutionId"].Should().Be(solution.CatalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditWorkOffPlan_InvalidId_ReturnsBadRequestResult(
            [Frozen] ISolutionsService mockService,
            DevelopmentPlansController controller,
            CatalogueItemId id)
        {
            mockService.GetSolutionWithWorkOffPlans(id).Returns(default(CatalogueItem));

            var actual = (await controller.EditWorkOffPlan(id, 1)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Solution found for Id: {id}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditWorkOffPlan_InvalidWorkOffPlanId_ReturnsBadRequest(
            [Frozen] ISolutionsService mockService,
            DevelopmentPlansController controller,
            Solution solution,
            int workOffPlanId)
        {
            mockService.GetSolutionWithWorkOffPlans(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var actual = (await controller.EditWorkOffPlan(solution.CatalogueItemId, workOffPlanId)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Work-off Plan item found for Id: {workOffPlanId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditWorkOffPlan_ValidSolutionForId_ReturnsExpectedViewResult(
            [Frozen] ISolutionsService mockService,
            DevelopmentPlansController controller,
            Solution solution,
            List<Standard> standards)
        {
            var workOffPlan = solution.WorkOffPlans.FirstOrDefault();

            var expectedModel = new EditWorkOffPlanModel(solution.CatalogueItem, standards, workOffPlan);

            mockService.GetSolutionWithWorkOffPlans(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            mockService.GetSolutionStandardsForEditing(solution.CatalogueItemId).Returns(standards);

            var actual = (await controller.EditWorkOffPlan(solution.CatalogueItemId, workOffPlan.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();

            actual.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditWorkOffPlan_InvalidId_ReturnsBadRequestResult(
            [Frozen] ISolutionsService mockService,
            DevelopmentPlansController controller,
            EditWorkOffPlanModel model,
            CatalogueItemId id,
            int workOffPlanId)
        {
            mockService.GetSolutionWithWorkOffPlans(id).Returns(default(CatalogueItem));

            var actual = (await controller.EditWorkOffPlan(id, workOffPlanId, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Solution found for Id: {id}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditWorkOffPlan_InvalidWorkOffPlanId_ReturnsBadRequest(
            [Frozen] ISolutionsService mockService,
            DevelopmentPlansController controller,
            EditWorkOffPlanModel model,
            Solution solution,
            int workOffPlanId)
        {
            mockService.GetSolutionWithWorkOffPlans(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var actual = (await controller.EditWorkOffPlan(solution.CatalogueItemId, workOffPlanId, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Work-off Plan item found for Id: {workOffPlanId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditWorkOffPlan_ValidIdForSolution_CallsSaveWorkOffPlans(
            [Frozen] ISolutionsService mockService,
            [Frozen] IDevelopmentPlansService mockDPService,
            DevelopmentPlansController controller,
            Solution solution)
        {
            var workOffPlan = solution.WorkOffPlans.FirstOrDefault();

            mockService.GetSolutionWithWorkOffPlans(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var model = new EditWorkOffPlanModel()
            {
                SolutionId = solution.CatalogueItemId,
                SelectedStandard = "Standard",
                Details = "Details",
                Day = "1",
                Month = "1",
                Year = "2111",
            };

            await controller.EditWorkOffPlan(solution.CatalogueItemId, workOffPlan.Id, model);

            await mockDPService.Received().UpdateWorkOffPlan(workOffPlan.Id, Arg.Any<SaveWorkOffPlanModel>());
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditWorkOffPlan_ValidIdForSolution_RedirectsToDevelopmentPlans(
            [Frozen] ISolutionsService mockService,
            DevelopmentPlansController controller,
            Solution solution)
        {
            var workOffPlan = solution.WorkOffPlans.FirstOrDefault();

            mockService.GetSolutionWithWorkOffPlans(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var model = new EditWorkOffPlanModel()
            {
                SolutionId = solution.CatalogueItemId,
                SelectedStandard = "Standard",
                Details = "Details",
                Day = "1",
                Month = "1",
                Year = "2111",
            };

            var actual = (await controller.EditWorkOffPlan(solution.CatalogueItemId, workOffPlan.Id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(DevelopmentPlansController.DevelopmentPlans));
            actual.RouteValues["solutionId"].Should().Be(solution.CatalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteWorkOffPlan_InvalidId_ReturnsBadRequestResult(
            [Frozen] ISolutionsService mockService,
            DevelopmentPlansController controller,
            CatalogueItemId id)
        {
            mockService.GetSolutionWithWorkOffPlans(id).Returns(default(CatalogueItem));

            var actual = (await controller.DeleteWorkOffPlan(id, 1)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Solution found for Id: {id}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteWorkOffPlan_InvalidWorkOffPlanId_ReturnsBadRequest(
            [Frozen] ISolutionsService mockService,
            DevelopmentPlansController controller,
            Solution solution,
            int workOffPlanId)
        {
            mockService.GetSolutionWithWorkOffPlans(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var actual = (await controller.DeleteWorkOffPlan(solution.CatalogueItemId, workOffPlanId)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Work-off Plan item found for Id: {workOffPlanId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteWorkOffPlan_ValidSolutionForId_ReturnsExpectedViewResult(
            [Frozen] ISolutionsService mockService,
            DevelopmentPlansController controller,
            Solution solution)
        {
            var workOffPlan = solution.WorkOffPlans.FirstOrDefault();

            var expectedModel = new EditWorkOffPlanModel(solution.CatalogueItem, workOffPlan);

            mockService.GetSolutionWithWorkOffPlans(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var actual = (await controller.DeleteWorkOffPlan(solution.CatalogueItemId, workOffPlan.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();

            actual.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteWorkOffPlan_InvalidId_ReturnsBadRequestResult(
            [Frozen] ISolutionsService mockService,
            DevelopmentPlansController controller,
            EditWorkOffPlanModel model,
            CatalogueItemId id,
            int workOffPlanId)
        {
            mockService.GetSolutionWithWorkOffPlans(id).Returns(default(CatalogueItem));

            var actual = (await controller.DeleteWorkOffPlan(id, workOffPlanId, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Solution found for Id: {id}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteWorkOffPlan_InvalidWorkOffPlanId_ReturnsBadRequest(
            [Frozen] ISolutionsService mockService,
            DevelopmentPlansController controller,
            EditWorkOffPlanModel model,
            Solution solution,
            int workOffPlanId)
        {
            mockService.GetSolutionWithWorkOffPlans(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var actual = (await controller.DeleteWorkOffPlan(solution.CatalogueItemId, workOffPlanId, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Work-off Plan item found for Id: {workOffPlanId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteWorkOffPlan_ValidIdForSolution_CallsSaveWorkOffPlans(
            [Frozen] ISolutionsService mockService,
            [Frozen] IDevelopmentPlansService mockDPService,
            DevelopmentPlansController controller,
            Solution solution)
        {
            var workOffPlan = solution.WorkOffPlans.FirstOrDefault();

            mockService.GetSolutionWithWorkOffPlans(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var model = new EditWorkOffPlanModel();

            await controller.DeleteWorkOffPlan(solution.CatalogueItemId, workOffPlan.Id, model);

            await mockDPService.Received().DeleteWorkOffPlan(workOffPlan.Id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteWorkOffPlan_ValidIdForSolution_RedirectsToDevelopmentPlans(
            [Frozen] ISolutionsService mockService,
            DevelopmentPlansController controller,
            Solution solution)
        {
            var workOffPlan = solution.WorkOffPlans.FirstOrDefault();

            mockService.GetSolutionWithWorkOffPlans(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var model = new EditWorkOffPlanModel();

            var actual = (await controller.DeleteWorkOffPlan(solution.CatalogueItemId, workOffPlan.Id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(DevelopmentPlansController.DevelopmentPlans));
            actual.RouteValues["solutionId"].Should().Be(solution.CatalogueItemId);
        }
    }
}
