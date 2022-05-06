﻿using System.Collections.Generic;
using System.Linq;
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
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.DevelopmentPlans;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.DevelopmentPlans;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
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
            mockService.Setup(s => s.GetSolutionWithWorkOffPlans(catalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var expected = new DevelopmentPlanModel(solution.CatalogueItem)
            {
                BackLink = "testUrl",
            };

            var actual = (await controller.DevelopmentPlans(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionWithWorkOffPlans(catalogueItemId));
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
            mockService.Setup(s => s.GetSolutionWithWorkOffPlans(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.DevelopmentPlans(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DevelopmentPlans_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            DevelopmentPlanModel model,
            [Frozen] Mock<ISolutionsService> mockService,
            DevelopmentPlansController controller)
        {
            mockService.Setup(s => s.GetSolutionWithWorkOffPlans(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.DevelopmentPlans(catalogueItemId, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
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
            actual.ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DevelopmentPlans_InvalidId_ExpectedViewModel(
            Solution solution,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            DevelopmentPlansController controller)
        {
            mockService.Setup(s => s.GetSolutionWithWorkOffPlans(catalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var expected = new DevelopmentPlanModel(solution.CatalogueItem);

            var actual = (await controller.DevelopmentPlans(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionWithWorkOffPlans(catalogueItemId));

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(expected, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddWorkOffPlan_InvalidId_ReturnsBadRequestResult(
            [Frozen] Mock<ISolutionsService> mockService,
            DevelopmentPlansController controller,
            CatalogueItemId id)
        {
            mockService.Setup(s => s.GetSolutionWithWorkOffPlans(id))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.DevelopmentPlans(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Solution found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddWorkOffPlan_ValidSolutionForId_ReturnsExpectedViewResult(
            [Frozen] Mock<ISolutionsService> mockService,
            DevelopmentPlansController controller,
            Solution solution,
            List<Standard> standards)
        {
            var expectedModel = new EditWorkOffPlanModel(solution.CatalogueItem, standards);

            mockService.Setup(s => s.GetSolutionWithWorkOffPlans(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            mockService.Setup(s => s.GetSolutionStandardsForEditing(solution.CatalogueItemId))
                .ReturnsAsync(standards);

            var actual = (await controller.AddWorkOffPlan(solution.CatalogueItemId)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().NotBeNull();
            actual.ViewName.Should().Be("EditWorkOffPlan");

            actual.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddWorkOffPlan_InvalidId_ReturnsBadRequestResult(
            [Frozen] Mock<ISolutionsService> mockService,
            DevelopmentPlansController controller,
            EditWorkOffPlanModel model,
            CatalogueItemId id)
        {
            mockService.Setup(s => s.GetSolutionWithWorkOffPlans(id))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.AddWorkOffPlan(id, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Solution found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddWorkOffPlan_ValidIdForSolution_CallsSaveWorkOffPlans(
            [Frozen] Mock<ISolutionsService> mockService,
            [Frozen] Mock<IDevelopmentPlansService> mockDPService,
            DevelopmentPlansController controller,
            Solution solution)
        {
            mockService.Setup(s => s.GetSolutionWithWorkOffPlans(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

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

            mockDPService.Verify(dp => dp.SaveWorkOffPlan(solution.CatalogueItemId, It.IsAny<SaveWorkOffPlanModel>()));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddWorkOffPlan_ValidIdForSolution_RedirectsToDevelopmentPlans(
            [Frozen] Mock<ISolutionsService> mockService,
            DevelopmentPlansController controller,
            Solution solution)
        {
            mockService.Setup(s => s.GetSolutionWithWorkOffPlans(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

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
        [CommonAutoData]
        public static async Task Get_EditWorkOffPlan_InvalidId_ReturnsBadRequestResult(
            [Frozen] Mock<ISolutionsService> mockService,
            DevelopmentPlansController controller,
            CatalogueItemId id)
        {
            mockService.Setup(s => s.GetSolutionWithWorkOffPlans(id))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.EditWorkOffPlan(id, 1)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Solution found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditWorkOffPlan_InvalidWorkOffPlanId_ReturnsBadRequest(
            [Frozen] Mock<ISolutionsService> mockService,
            DevelopmentPlansController controller,
            Solution solution,
            int workOffPlanId)
        {
            mockService.Setup(s => s.GetSolutionWithWorkOffPlans(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var actual = (await controller.EditWorkOffPlan(solution.CatalogueItemId, workOffPlanId)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Work-off Plan item found for Id: {workOffPlanId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditWorkOffPlan_ValidSolutionForId_ReturnsExpectedViewResult(
            [Frozen] Mock<ISolutionsService> mockService,
            DevelopmentPlansController controller,
            Solution solution,
            List<Standard> standards)
        {
            var workOffPlan = solution.WorkOffPlans.FirstOrDefault();

            var expectedModel = new EditWorkOffPlanModel(solution.CatalogueItem, standards, workOffPlan);

            mockService.Setup(s => s.GetSolutionWithWorkOffPlans(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            mockService.Setup(s => s.GetSolutionStandardsForEditing(solution.CatalogueItemId))
                .ReturnsAsync(standards);

            var actual = (await controller.EditWorkOffPlan(solution.CatalogueItemId, workOffPlan.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();

            actual.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditWorkOffPlan_InvalidId_ReturnsBadRequestResult(
            [Frozen] Mock<ISolutionsService> mockService,
            DevelopmentPlansController controller,
            EditWorkOffPlanModel model,
            CatalogueItemId id,
            int workOffPlanId)
        {
            mockService.Setup(s => s.GetSolutionWithWorkOffPlans(id))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.EditWorkOffPlan(id, workOffPlanId, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Solution found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditWorkOffPlan_InvalidWorkOffPlanId_ReturnsBadRequest(
            [Frozen] Mock<ISolutionsService> mockService,
            DevelopmentPlansController controller,
            EditWorkOffPlanModel model,
            Solution solution,
            int workOffPlanId)
        {
            mockService.Setup(s => s.GetSolutionWithWorkOffPlans(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var actual = (await controller.EditWorkOffPlan(solution.CatalogueItemId, workOffPlanId, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Work-off Plan item found for Id: {workOffPlanId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditWorkOffPlan_ValidIdForSolution_CallsSaveWorkOffPlans(
            [Frozen] Mock<ISolutionsService> mockService,
            [Frozen] Mock<IDevelopmentPlansService> mockDPService,
            DevelopmentPlansController controller,
            Solution solution)
        {
            var workOffPlan = solution.WorkOffPlans.FirstOrDefault();

            mockService.Setup(s => s.GetSolutionWithWorkOffPlans(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

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

            mockDPService.Verify(dp => dp.UpdateWorkOffPlan(workOffPlan.Id, It.IsAny<SaveWorkOffPlanModel>()));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditWorkOffPlan_ValidIdForSolution_RedirectsToDevelopmentPlans(
            [Frozen] Mock<ISolutionsService> mockService,
            DevelopmentPlansController controller,
            Solution solution)
        {
            var workOffPlan = solution.WorkOffPlans.FirstOrDefault();

            mockService.Setup(s => s.GetSolutionWithWorkOffPlans(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

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
        [CommonAutoData]
        public static async Task Get_DeleteWorkOffPlan_InvalidId_ReturnsBadRequestResult(
            [Frozen] Mock<ISolutionsService> mockService,
            DevelopmentPlansController controller,
            CatalogueItemId id)
        {
            mockService.Setup(s => s.GetSolutionWithWorkOffPlans(id))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.DeleteWorkOffPlan(id, 1)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Solution found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteWorkOffPlan_InvalidWorkOffPlanId_ReturnsBadRequest(
            [Frozen] Mock<ISolutionsService> mockService,
            DevelopmentPlansController controller,
            Solution solution,
            int workOffPlanId)
        {
            mockService.Setup(s => s.GetSolutionWithWorkOffPlans(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var actual = (await controller.DeleteWorkOffPlan(solution.CatalogueItemId, workOffPlanId)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Work-off Plan item found for Id: {workOffPlanId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteWorkOffPlan_ValidSolutionForId_ReturnsExpectedViewResult(
            [Frozen] Mock<ISolutionsService> mockService,
            DevelopmentPlansController controller,
            Solution solution)
        {
            var workOffPlan = solution.WorkOffPlans.FirstOrDefault();

            var expectedModel = new EditWorkOffPlanModel(solution.CatalogueItem, workOffPlan);

            mockService.Setup(s => s.GetSolutionWithWorkOffPlans(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var actual = (await controller.DeleteWorkOffPlan(solution.CatalogueItemId, workOffPlan.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();

            actual.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteWorkOffPlan_InvalidId_ReturnsBadRequestResult(
            [Frozen] Mock<ISolutionsService> mockService,
            DevelopmentPlansController controller,
            EditWorkOffPlanModel model,
            CatalogueItemId id,
            int workOffPlanId)
        {
            mockService.Setup(s => s.GetSolutionWithWorkOffPlans(id))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.DeleteWorkOffPlan(id, workOffPlanId, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Solution found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteWorkOffPlan_InvalidWorkOffPlanId_ReturnsBadRequest(
            [Frozen] Mock<ISolutionsService> mockService,
            DevelopmentPlansController controller,
            EditWorkOffPlanModel model,
            Solution solution,
            int workOffPlanId)
        {
            mockService.Setup(s => s.GetSolutionWithWorkOffPlans(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var actual = (await controller.DeleteWorkOffPlan(solution.CatalogueItemId, workOffPlanId, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Work-off Plan item found for Id: {workOffPlanId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteWorkOffPlan_ValidIdForSolution_CallsSaveWorkOffPlans(
            [Frozen] Mock<ISolutionsService> mockService,
            [Frozen] Mock<IDevelopmentPlansService> mockDPService,
            DevelopmentPlansController controller,
            Solution solution)
        {
            var workOffPlan = solution.WorkOffPlans.FirstOrDefault();

            mockService.Setup(s => s.GetSolutionWithWorkOffPlans(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var model = new EditWorkOffPlanModel();

            await controller.DeleteWorkOffPlan(solution.CatalogueItemId, workOffPlan.Id, model);

            mockDPService.Verify(dp => dp.DeleteWorkOffPlan(workOffPlan.Id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteWorkOffPlan_ValidIdForSolution_RedirectsToDevelopmentPlans(
            [Frozen] Mock<ISolutionsService> mockService,
            DevelopmentPlansController controller,
            Solution solution)
        {
            var workOffPlan = solution.WorkOffPlans.FirstOrDefault();

            mockService.Setup(s => s.GetSolutionWithWorkOffPlans(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var model = new EditWorkOffPlanModel();

            var actual = (await controller.DeleteWorkOffPlan(solution.CatalogueItemId, workOffPlan.Id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(DevelopmentPlansController.DevelopmentPlans));
            actual.RouteValues["solutionId"].Should().Be(solution.CatalogueItemId);
        }
    }
}
