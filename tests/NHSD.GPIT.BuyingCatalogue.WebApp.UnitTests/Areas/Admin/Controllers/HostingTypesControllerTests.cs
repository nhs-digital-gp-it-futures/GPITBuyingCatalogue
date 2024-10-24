﻿using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class HostingTypesControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(HostingTypesController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_HostingType_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            mockService.GetSolutionThin(catalogueItemId).Returns(catalogueItem);

            await controller.HostingType(catalogueItemId);

            await mockService.Received().GetSolutionThin(catalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_HostingType_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            mockService.GetSolutionThin(catalogueItemId).Returns(catalogueItem);

            var actual = (await controller.HostingType(catalogueItemId)).As<ViewResult>();

            await mockService.Received().GetSolutionThin(catalogueItemId);
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new HostingTypeSectionModel(catalogueItem), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_HostingType_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            mockService.GetSolutionThin(catalogueItemId).Returns(default(CatalogueItem));

            var actual = (await controller.HostingType(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_HostingType_CallsSaveHosting(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            var model = new HostingTypeSectionModel(catalogueItem);

            mockService.GetSolutionThin(catalogueItemId).Returns(catalogueItem);
            await controller.HostingType(catalogueItemId, model);

            await mockService.Received().SaveHosting(catalogueItemId, Arg.Any<Hosting>());
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_HostingType_RedirectsToManageCatalogueSolution(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            var model = new HostingTypeSectionModel(catalogueItem);

            mockService.GetSolutionThin(catalogueItemId).Returns(catalogueItem);

            var actual = (await controller.HostingType(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_HostingType_InvalidId_ReturnsBadRequestResult(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            mockService.GetSolutionThin(catalogueItemId).Returns(catalogueItem);

            var actual = (await controller.HostingType(catalogueItemId)).As<ViewResult>();

            await mockService.Received().GetSolutionThin(catalogueItemId);
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new HostingTypeSectionModel(catalogueItem), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddHostingType_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            mockService.GetSolutionThin(catalogueItemId).Returns(catalogueItem);

            await controller.AddHostingType(catalogueItemId);

            await mockService.Received().GetSolutionThin(catalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddHostingType_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            mockService.GetSolutionThin(catalogueItemId).Returns(catalogueItem);

            var actual = (await controller.AddHostingType(catalogueItemId)).As<ViewResult>();

            await mockService.Received().GetSolutionThin(catalogueItemId);
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new HostingTypeSelectionModel(catalogueItem), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddHostingType_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            mockService.GetSolutionThin(catalogueItemId).Returns(default(CatalogueItem));

            var actual = (await controller.AddHostingType(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddHostingType_RedirectsToPublicCloud(
            CatalogueItemId catalogueItemId,
            HostingTypeSelectionModel model,
            HostingTypesController controller)
        {
            model.SelectedHostingType = HostingType.PublicCloud;
            var actual = (await controller.AddHostingType(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(HostingTypesController.PublicCloud));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddHostingType_RedirectsToPrivateCloud(
            CatalogueItemId catalogueItemId,
            HostingTypeSelectionModel model,
            HostingTypesController controller)
        {
            model.SelectedHostingType = HostingType.PrivateCloud;
            var actual = (await controller.AddHostingType(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(HostingTypesController.PrivateCloud));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddHostingType_RedirectsToHybridCloud(
            CatalogueItemId catalogueItemId,
            HostingTypeSelectionModel model,
            HostingTypesController controller)
        {
            model.SelectedHostingType = HostingType.Hybrid;
            var actual = (await controller.AddHostingType(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(HostingTypesController.Hybrid));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddHostingType_RedirectsToOnPremiseCloud(
            CatalogueItemId catalogueItemId,
            HostingTypeSelectionModel model,
            HostingTypesController controller)
        {
            model.SelectedHostingType = HostingType.OnPremise;
            var actual = (await controller.AddHostingType(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(HostingTypesController.OnPremise));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_PublicCloud_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            Solution solution,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            mockService.GetSolutionThin(catalogueItemId).Returns(solution.CatalogueItem);

            await controller.PublicCloud(catalogueItemId);

            await mockService.Received().GetSolutionThin(catalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_PublicCloud_ValidId_ReturnsViewWithExpectedModel(
            Solution solution,
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            mockService.GetSolutionThin(catalogueItemId).Returns(catalogueItem);

            var actual = (await controller.PublicCloud(catalogueItemId)).As<ViewResult>();

            await mockService.Received().GetSolutionThin(catalogueItemId);
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new PublicCloudModel(catalogueItem), opt => opt.Excluding(member => member.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_PublicCloud_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            mockService.GetSolutionThin(catalogueItemId).Returns(default(CatalogueItem));
            var actual = (await controller.PublicCloud(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_PublicCloud_CallsSaveHosting(
            CatalogueItem catalogueItem,
            Solution solution,
            PublicCloudModel model,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            catalogueItem.Solution = solution;

            mockService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            await controller.PublicCloud(catalogueItem.Id, model);

            await mockService.Received().SaveHosting(catalogueItem.Id, solution.Hosting);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_PublicCloud_RedirectsToHostingType(
            CatalogueItem catalogueItem,
            Solution solution,
            PublicCloudModel model,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            catalogueItem.Solution = solution;

            mockService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            var actual = (await controller.PublicCloud(catalogueItem.Id, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(HostingTypesController.HostingType));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItem.Id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_PublicCloud_InvalidId_ReturnsBadRequestResult(
            Solution solution,
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            mockService.GetSolutionThin(catalogueItemId).Returns(catalogueItem);

            var actual = (await controller.PublicCloud(catalogueItemId)).As<ViewResult>();

            await mockService.Received().GetSolutionThin(catalogueItemId);
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new PublicCloudModel(catalogueItem), opt => opt.Excluding(member => member.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_PrivateCloud_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            Solution solution,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            mockService.GetSolutionThin(catalogueItemId).Returns(solution.CatalogueItem);

            await controller.PrivateCloud(catalogueItemId);

            await mockService.Received().GetSolutionThin(catalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_PrivateCloud_ValidId_ReturnsViewWithExpectedModel(
            Solution solution,
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            mockService.GetSolutionThin(catalogueItemId).Returns(catalogueItem);

            var actual = (await controller.PrivateCloud(catalogueItemId)).As<ViewResult>();

            await mockService.Received().GetSolutionThin(catalogueItemId);
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new PrivateCloudModel(catalogueItem), opt => opt.Excluding(member => member.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_PrivateCloud_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            mockService.GetSolutionThin(catalogueItemId).Returns(default(CatalogueItem));

            var actual = (await controller.PrivateCloud(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_PrivateCloud_CallsSaveHosting(
            CatalogueItem catalogueItem,
            Solution solution,
            PrivateCloudModel model,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            catalogueItem.Solution = solution;

            mockService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            await controller.PrivateCloud(catalogueItem.Id, model);

            await mockService.Received().SaveHosting(catalogueItem.Id, solution.Hosting);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_PrivateCloud_RedirectsToHostingType(
            CatalogueItem catalogueItem,
            Solution solution,
            PrivateCloudModel model,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            catalogueItem.Solution = solution;

            mockService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            var actual = (await controller.PrivateCloud(catalogueItem.Id, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(HostingTypesController.HostingType));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItem.Id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_PrivateCloud_InvalidId_ReturnsBadRequestResult(
            Solution solution,
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            mockService.GetSolutionThin(catalogueItemId).Returns(catalogueItem);

            var actual = (await controller.PrivateCloud(catalogueItemId)).As<ViewResult>();

            await mockService.Received().GetSolutionThin(catalogueItemId);
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new PrivateCloudModel(catalogueItem), opt => opt.Excluding(member => member.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_HybridCloud_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            Solution solution,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            mockService.GetSolutionThin(catalogueItemId).Returns(solution.CatalogueItem);

            await controller.Hybrid(catalogueItemId);

            await mockService.Received().GetSolutionThin(catalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_HybridCloud_ValidId_ReturnsViewWithExpectedModel(
            Solution solution,
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            mockService.GetSolutionThin(catalogueItemId).Returns(catalogueItem);

            var actual = (await controller.Hybrid(catalogueItemId)).As<ViewResult>();

            await mockService.Received().GetSolutionThin(catalogueItemId);
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new HybridModel(catalogueItem), opt => opt.Excluding(member => member.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_HybridCloud_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            mockService.GetSolutionThin(catalogueItemId).Returns(default(CatalogueItem));

            var actual = (await controller.Hybrid(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_HybridCloud_CallsSaveHosting(
            CatalogueItem catalogueItem,
            Solution solution,
            HybridModel model,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            catalogueItem.Solution = solution;

            mockService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);
            await controller.Hybrid(catalogueItem.Id, model);

            await mockService.Received().SaveHosting(catalogueItem.Id, solution.Hosting);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_HybridCloud_RedirectsToHostingType(
            CatalogueItem catalogueItem,
            Solution solution,
            HybridModel model,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            catalogueItem.Solution = solution;

            mockService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            var actual = (await controller.Hybrid(catalogueItem.Id, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(HostingTypesController.HostingType));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItem.Id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_HybridCloud_InvalidId_ReturnsBadRequestResult(
            Solution solution,
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            mockService.GetSolutionThin(catalogueItemId).Returns(catalogueItem);

            var actual = (await controller.Hybrid(catalogueItemId)).As<ViewResult>();

            await mockService.Received().GetSolutionThin(catalogueItemId);
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new HybridModel(catalogueItem), opt => opt.Excluding(member => member.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_OnPremiseCloud_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            Solution solution,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            mockService.GetSolutionThin(catalogueItemId).Returns(solution.CatalogueItem);

            await controller.OnPremise(catalogueItemId);

            await mockService.Received().GetSolutionThin(catalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_OnPremise_ValidId_ReturnsViewWithExpectedModel(
            Solution solution,
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            mockService.GetSolutionThin(catalogueItemId).Returns(catalogueItem);

            var actual = (await controller.OnPremise(catalogueItemId)).As<ViewResult>();

            await mockService.Received().GetSolutionThin(catalogueItemId);
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new OnPremiseModel(catalogueItem), opt => opt.Excluding(member => member.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_OnPremise_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            mockService.GetSolutionThin(catalogueItemId).Returns(default(CatalogueItem));

            var actual = (await controller.OnPremise(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_OnPremise_CallsSaveHosting(
            CatalogueItem catalogueItem,
            Solution solution,
            OnPremiseModel model,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            catalogueItem.Solution = solution;

            mockService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            await controller.OnPremise(catalogueItem.Id, model);

            await mockService.Received().SaveHosting(catalogueItem.Id, solution.Hosting);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_OnPremise_RedirectsToHostingType(
            CatalogueItem catalogueItem,
            Solution solution,
            OnPremiseModel model,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            catalogueItem.Solution = solution;

            mockService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            var actual = (await controller.OnPremise(catalogueItem.Id, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(HostingTypesController.HostingType));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItem.Id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_OnPremise_InvalidId_ReturnsBadRequestResult(
            Solution solution,
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            HostingTypesController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            mockService.GetSolutionThin(catalogueItemId).Returns(catalogueItem);

            var actual = (await controller.OnPremise(catalogueItemId)).As<ViewResult>();

            await mockService.Received().GetSolutionThin(catalogueItemId);
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new OnPremiseModel(catalogueItem), opt => opt.Excluding(member => member.BackLink));
        }

        [Theory]
        [MockInlineAutoData(HostingType.Hybrid)]
        [MockInlineAutoData(HostingType.OnPremise)]
        [MockInlineAutoData(HostingType.PrivateCloud)]
        [MockInlineAutoData(HostingType.PublicCloud)]
        public static async Task Get_DeleteHostingType_ReturnsModel(
            HostingType hostingType,
            CatalogueItem catalogueItem,
            [Frozen] ISolutionsService solutionsService,
            HostingTypesController controller)
        {
            var expectedModel = new DeleteHostingTypeConfirmationModel
            {
                HostingType = hostingType,
                SolutionId = catalogueItem.Id,
                SolutionName = catalogueItem.Name,
            };

            solutionsService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            var result = await controller.DeleteHostingType(catalogueItem.Id, hostingType);

            result.Should().NotBeNull();
            var viewResult = result.As<ViewResult>();
            viewResult.Should().NotBeNull();

            var model = result.As<ViewResult>().Model.As<DeleteHostingTypeConfirmationModel>();
            model.Should().NotBeNull();
            model.Should().BeEquivalentTo(expectedModel, opts => opts.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteHostingType_InvalidModel_ReturnsViewWithModel(
            HostingType hostingType,
            CatalogueItem catalogueItem,
            [Frozen] ISolutionsService solutionsService,
            HostingTypesController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var model = new DeleteHostingTypeConfirmationModel
            {
                HostingType = hostingType,
                SolutionId = catalogueItem.Id,
                SolutionName = catalogueItem.Name,
            };

            solutionsService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            var result = await controller.DeleteHostingType(catalogueItem.Id, hostingType, model);

            result.Should().NotBeNull();
            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteHybridHostingType_ValidModel_DeletesHostingType(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            HostingTypesController controller)
        {
            const HostingType hostingType = HostingType.Hybrid;

            var catalogueItem = solution.CatalogueItem;
            var model = new DeleteHostingTypeConfirmationModel
            {
                HostingType = hostingType,
                SolutionId = catalogueItem.Id,
                SolutionName = catalogueItem.Name,
            };

            solutionsService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            await controller.DeleteHostingType(catalogueItem.Id, hostingType, model);

            await solutionsService.Received().SaveHosting(
                catalogueItem.Id,
                Arg.Is<Hosting>(hosting => hosting.HybridHostingType.HostingModel == null
                    && hosting.HybridHostingType.Link == null
                    && hosting.HybridHostingType.RequiresHscn == null
                    && hosting.HybridHostingType.Summary == null));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteOnPremiseHostingType_ValidModel_DeletesHostingType(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            HostingTypesController controller)
        {
            const HostingType hostingType = HostingType.OnPremise;

            var catalogueItem = solution.CatalogueItem;
            var model = new DeleteHostingTypeConfirmationModel
            {
                HostingType = hostingType,
                SolutionId = catalogueItem.Id,
                SolutionName = catalogueItem.Name,
            };

            solutionsService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            await controller.DeleteHostingType(catalogueItem.Id, hostingType, model);

            await solutionsService.Received().SaveHosting(
                catalogueItem.Id,
                Arg.Is<Hosting>(hosting => hosting.OnPremise.HostingModel == null
                    && hosting.OnPremise.Link == null
                    && hosting.OnPremise.RequiresHscn == null
                    && hosting.OnPremise.Summary == null));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeletePrivateCloudHostingType_ValidModel_DeletesHostingType(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            HostingTypesController controller)
        {
            const HostingType hostingType = HostingType.PrivateCloud;

            var catalogueItem = solution.CatalogueItem;
            var model = new DeleteHostingTypeConfirmationModel
            {
                HostingType = hostingType,
                SolutionId = catalogueItem.Id,
                SolutionName = catalogueItem.Name,
            };

            solutionsService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            await controller.DeleteHostingType(catalogueItem.Id, hostingType, model);

            await solutionsService.Received().SaveHosting(
                catalogueItem.Id,
                Arg.Is<Hosting>(hosting => hosting.PrivateCloud.HostingModel == null
                    && hosting.PrivateCloud.Link == null
                    && hosting.PrivateCloud.RequiresHscn == null
                    && hosting.PrivateCloud.Summary == null));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeletePublicCloudHostingType_ValidModel_DeletesHostingType(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            HostingTypesController controller)
        {
            const HostingType hostingType = HostingType.PublicCloud;

            var catalogueItem = solution.CatalogueItem;
            var model = new DeleteHostingTypeConfirmationModel
            {
                HostingType = hostingType,
                SolutionId = catalogueItem.Id,
                SolutionName = catalogueItem.Name,
            };

            solutionsService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            await controller.DeleteHostingType(catalogueItem.Id, hostingType, model);

            await solutionsService.Received().SaveHosting(
                catalogueItem.Id,
                Arg.Is<Hosting>(hosting => hosting.PublicCloud.Link == null
                    && hosting.PublicCloud.RequiresHscn == null
                    && hosting.PublicCloud.Summary == null));
        }
    }
}
