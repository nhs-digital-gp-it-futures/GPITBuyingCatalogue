﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Caching;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.BrowserBasedModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels;
using Xunit;
using PublicationStatus = NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models.PublicationStatus;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class CatalogueSolutionsControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(CatalogueSolutionsController).Should()
                .BeDecoratedWith<AuthorizeAttribute>(x => x.Policy == "AdminOnly");
            typeof(CatalogueSolutionsController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Admin");
            typeof(CatalogueSolutionsController).Should()
                .BeDecoratedWith<RouteAttribute>(x => x.Template == "admin/catalogue-solutions");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CatalogueSolutionsController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Features_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(new CatalogueItem());

            await controller.Features(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Features_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.Features(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new FeaturesModel().FromCatalogueItem(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Features_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.Features(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Features_CallsSavesSolutionFeatures(
            CatalogueItemId catalogueItemId,
            FeaturesModel model,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            await controller.Features(catalogueItemId, model);

            mockService.Verify(s => s.SaveSolutionFeatures(catalogueItemId, model.AllFeatures));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Features_RedirectsToManageCatalogueSolution(
            CatalogueItemId catalogueItemId,
            FeaturesModel model,
            CatalogueSolutionsController controller)
        {
            var actual = (await controller.Features(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Features_InvalidId_ReturnsBadRequestResult(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.Features(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new FeaturesModel().FromCatalogueItem(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_SolutionsInDatabase_ReturnsViewWithExpectedModel(
            List<CatalogueItem> solutions,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            var expected = solutions.Select(s => new CatalogueModel(s)).ToList();
            mockService.Setup(s => s.GetAllSolutions(null))
                .ReturnsAsync(solutions);

            var actual = (await controller.Index()).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            var model = actual.Model.As<CatalogueSolutionsModel>();
            model.Solutions.Should().BeEquivalentTo(expected);
            model.SelectedPublicationStatus.Should().BeNullOrWhiteSpace();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Index_StatusInput_SetsSelectedOnModel_ReturnsViewWithModel(
            List<CatalogueItem> solutions,
            PublicationStatus status,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            var model = new CatalogueSolutionsModel { SelectedPublicationStatus = status.ToString() };
            mockService.Setup(s => s.GetAllSolutions(status))
                .ReturnsAsync(solutions);

            var actual = (await controller.Index(model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            model.SetSolutions(solutions);
            actual.Model.As<CatalogueSolutionsModel>().Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ManageSolution_ReturnsViewWithExpectedModel(
            [Frozen] AspNetUser aspNetUser,
            CatalogueItem expected,
            int userId,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            CatalogueSolutionsController controller)
        {
            expected.Solution.LastUpdatedBy = userId;
            aspNetUser.Id = userId;

            mockSolutionService.Setup(s => s.GetSolution(expected.Id))
                .ReturnsAsync(expected);

            var actual = (await controller.ManageCatalogueSolution(expected.Id)).As<ViewResult>();

            mockSolutionService.Verify(s => s.GetSolution(expected.Id));
            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            var model = actual.Model.As<ManageCatalogueSolutionModel>();
            model.Solution.Should().BeEquivalentTo(expected);
            model.LastUpdatedByName.Should().BeEquivalentTo($"{aspNetUser.FirstName} {aspNetUser.LastName}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Description_ReturnsViewWithExpectedModel(
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            CatalogueSolutionsController controller)
        {
            var expected = new CatalogueItem
            {
                Id = new CatalogueItemId(999999, "999"),
                Solution = new Solution
                {
                    Summary = "XYZ Summary",
                    FullDescription = "XYZ description",
                    AboutUrl = "Fake url",
                },
                Name = "Fake Solution",
            };
            mockSolutionService.Setup(s => s.GetSolution(It.IsAny<CatalogueItemId>()))
                .ReturnsAsync(expected);

            var actual = (await controller.Description(expected.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            var model = actual.Model.As<DescriptionModel>();
            model.SolutionName.Should().BeEquivalentTo("Fake Solution");
            model.Summary.Should().BeEquivalentTo("XYZ Summary");
            model.Description.Should().BeEquivalentTo("XYZ description");
            model.Link.Should().BeEquivalentTo("Fake url");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Description_RedirectsToManageCatalogueSolution(
            CatalogueItemId catalogueItemId,
            DescriptionModel model,
            CatalogueSolutionsController controller)
        {
            var actual = (await controller.Description(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SolutionDescription_InvalidModel_DoesNotCallService(
            CatalogueItemId id,
            Mock<ISolutionsService> mockSolutionService,
            CatalogueSolutionsController controller)
        {
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.Description(id, new Mock<DescriptionModel>().Object);

            mockSolutionService.Verify(
                s => s.SaveSolutionDescription(It.IsAny<CatalogueItemId>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditDescription_InvalidModel_ReturnsViewWithModel(
            CatalogueItemId id,
            DescriptionModel editDescriptionModel,
            CatalogueSolutionsController controller)
        {
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.Description(id, editDescriptionModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(editDescriptionModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditDescription_ValidModel_CallsSaveSolutionDescriptionOnService(
            CatalogueItemId id,
            DescriptionModel model,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            CatalogueSolutionsController controller)
        {
            await controller.Description(id, model);

            mockSolutionService.Verify(s => s.SaveSolutionDescription(id, model.Summary, model.Description, model.Link));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditDescription_ValidModel_RedirectsToExpectedAction(
            DescriptionModel model,
            CatalogueSolutionsController controller)
        {
            var actual = (await controller.Description(model.SolutionId!.Value, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.RouteValues["solutionId"].Should().Be(model.SolutionId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Implementation_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(new CatalogueItem());

            await controller.Implementation(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Implementation_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.Implementation(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new ImplementationTimescaleModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Implementation_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.Implementation(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Implementation_CallsSaveImplementationDetail(
            CatalogueItemId catalogueItemId,
            ImplementationTimescaleModel model,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            await controller.Implementation(catalogueItemId, model);

            mockService.Verify(s => s.SaveImplementationDetail(catalogueItemId, It.IsAny<string>()));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Implementation_RedirectsToManageCatalogueSolution(
            CatalogueItemId catalogueItemId,
            ImplementationTimescaleModel model,
            CatalogueSolutionsController controller)
        {
            var actual = (await controller.Implementation(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Implementation_InvalidId_ReturnsBadRequestResult(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.Implementation(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new ImplementationTimescaleModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Roadmap_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(new CatalogueItem());

            await controller.Roadmap(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Roadmap_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.Roadmap(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new RoadmapModel().FromCatalogueItem(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Roadmap_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.Roadmap(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Roadmap_CallsSavesSolutionRoadmap(
            CatalogueItemId catalogueItemId,
            RoadmapModel model,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            await controller.Roadmap(catalogueItemId, model);

            mockService.Verify(s => s.SaveRoadMap(catalogueItemId, model.Link));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Roadmap_RedirectsToManageCatalogueSolution(
            CatalogueItemId catalogueItemId,
            RoadmapModel model,
            CatalogueSolutionsController controller)
        {
            var actual = (await controller.Roadmap(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Roadmap_InvalidId_ReturnsBadRequestResult(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.Roadmap(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new RoadmapModel().FromCatalogueItem(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HostingType_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            await controller.HostingType(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HostingType_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.HostingType(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new HostingTypeSectionModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HostingType_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.HostingType(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HostingType_CallsSaveHosting(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            var model = new HostingTypeSectionModel(catalogueItem);

            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            await controller.HostingType(catalogueItemId, model);

            mockService.Verify(s => s.SaveHosting(catalogueItemId, It.IsAny<Hosting>()));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HostingType_RedirectsToManageCatalogueSolution(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            var model = new HostingTypeSectionModel(catalogueItem);

            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.HostingType(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HostingType_InvalidId_ReturnsBadRequestResult(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.HostingType(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new HostingTypeSectionModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddHostingType_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            await controller.AddHostingType(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddHostingType_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.AddHostingType(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new HostingTypeSelectionModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddHostingType_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.AddHostingType(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddHostingType_RedirectsToPublicCloud(
            CatalogueItemId catalogueItemId,
            HostingTypeSelectionModel model,
            CatalogueSolutionsController controller)
        {
            model.SelectedHostingType = HostingType.PublicCloud;
            var actual = (await controller.AddHostingType(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.PublicCloud));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddHostingType_RedirectsToPrivateCloud(
            CatalogueItemId catalogueItemId,
            HostingTypeSelectionModel model,
            CatalogueSolutionsController controller)
        {
            model.SelectedHostingType = HostingType.PrivateCloud;
            var actual = (await controller.AddHostingType(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.PrivateCloud));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddHostingType_RedirectsToHybridCloud(
            CatalogueItemId catalogueItemId,
            HostingTypeSelectionModel model,
            CatalogueSolutionsController controller)
        {
            model.SelectedHostingType = HostingType.Hybrid;
            var actual = (await controller.AddHostingType(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.Hybrid));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddHostingType_RedirectsToOnPremiseCloud(
            CatalogueItemId catalogueItemId,
            HostingTypeSelectionModel model,
            CatalogueSolutionsController controller)
        {
            model.SelectedHostingType = HostingType.OnPremise;
            var actual = (await controller.AddHostingType(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.OnPremise));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PublicCloud_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            await controller.PublicCloud(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PublicCloud_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.PublicCloud(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new PublicCloudModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PublicCloud_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));
            var actual = (await controller.PublicCloud(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PublicCloud_CallsSaveHosting(
            CatalogueItemId catalogueItemId,
            PublicCloudModel model,
            Hosting hosting,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetHosting(catalogueItemId))
                .ReturnsAsync(hosting);
            await controller.PublicCloud(catalogueItemId, model);

            hosting.PublicCloud = new PublicCloud { Summary = model.Summary, Link = model.Link, RequiresHscn = model.RequiresHscn };

            mockService.Verify(s => s.SaveHosting(catalogueItemId, hosting));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PublicCloud_RedirectsToHostingType(
            CatalogueItemId catalogueItemId,
            Hosting hosting,
            PublicCloudModel model,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetHosting(catalogueItemId))
                .ReturnsAsync(hosting);

            var actual = (await controller.PublicCloud(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.HostingType));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PublicCloud_InvalidId_ReturnsBadRequestResult(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.PublicCloud(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new PublicCloudModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PrivateCloud_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            await controller.PrivateCloud(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PrivateCloud_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.PrivateCloud(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new PrivateCloudModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PrivateCloud_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.PrivateCloud(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PrivateCloud_CallsSaveHosting(
            CatalogueItemId catalogueItemId,
            PrivateCloudModel model,
            Hosting hosting,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetHosting(catalogueItemId))
                .ReturnsAsync(hosting);
            await controller.PrivateCloud(catalogueItemId, model);

            hosting.PrivateCloud = new PrivateCloud { Summary = model.Summary, Link = model.Link, RequiresHscn = model.RequiresHscn, HostingModel = model.HostingModel };

            mockService.Verify(s => s.SaveHosting(catalogueItemId, hosting));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PrivateCloud_RedirectsToHostingType(
            CatalogueItemId catalogueItemId,
            Hosting hosting,
            PrivateCloudModel model,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetHosting(catalogueItemId))
                .ReturnsAsync(hosting);

            var actual = (await controller.PrivateCloud(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.HostingType));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PrivateCloud_InvalidId_ReturnsBadRequestResult(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.PrivateCloud(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new PrivateCloudModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HybridCloud_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            await controller.Hybrid(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HybridCloud_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.Hybrid(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new HybridModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HybridCloud_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.Hybrid(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HybridCloud_CallsSaveHosting(
            CatalogueItemId catalogueItemId,
            HybridModel model,
            Hosting hosting,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetHosting(catalogueItemId))
                .ReturnsAsync(hosting);
            await controller.Hybrid(catalogueItemId, model);

            hosting.HybridHostingType = new HybridHostingType { Summary = model.Summary, Link = model.Link, RequiresHscn = model.RequiresHscn, HostingModel = model.HostingModel };

            mockService.Verify(s => s.SaveHosting(catalogueItemId, hosting));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HybridCloud_RedirectsToHostingType(
            CatalogueItemId catalogueItemId,
            Hosting hosting,
            HybridModel model,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetHosting(catalogueItemId))
                .ReturnsAsync(hosting);

            var actual = (await controller.Hybrid(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.HostingType));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HybridCloud_InvalidId_ReturnsBadRequestResult(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.Hybrid(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new HybridModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_OnPremiseCloud_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            await controller.OnPremise(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_OnPremise_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.OnPremise(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new OnPremiseModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_OnPremise_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.OnPremise(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_OnPremise_CallsSaveHosting(
            CatalogueItemId catalogueItemId,
            OnPremiseModel model,
            Hosting hosting,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetHosting(catalogueItemId))
                .ReturnsAsync(hosting);
            await controller.OnPremise(catalogueItemId, model);

            hosting.OnPremise = new OnPremise { Summary = model.Summary, Link = model.Link, RequiresHscn = model.RequiresHscn, HostingModel = model.HostingModel };

            mockService.Verify(s => s.SaveHosting(catalogueItemId, hosting));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_OnPremise_RedirectsToHostingType(
            CatalogueItemId catalogueItemId,
            Hosting hosting,
            OnPremiseModel model,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetHosting(catalogueItemId))
                .ReturnsAsync(hosting);

            var actual = (await controller.OnPremise(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.HostingType));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_OnPremise_InvalidId_ReturnsBadRequestResult(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.OnPremise(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new OnPremiseModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_BrowserBased_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(new CatalogueItem());

            await controller.BrowserBased(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_BrowserBased_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.BrowserBased(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_BrowserBased_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.BrowserBased(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new BrowserBasedModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupportedBrowsers_GetsSolutionFromService(
           CatalogueItemId catalogueItemId,
           CatalogueItem catalogueItem,
           [Frozen] Mock<ISolutionsService> mockService,
           CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            await controller.SupportedBrowsers(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupportedBrowsers_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.SupportedBrowsers(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupportedBrowsers_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.SupportedBrowsers(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new SupportedBrowsersModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SupportedBrowsers_CallsSaveClientApplication(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            SupportedBrowsersModel model,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetClientApplication(catalogueItemId))
                .ReturnsAsync(clientApplication);

            await controller.SupportedBrowsers(catalogueItemId, model);

            mockService.Verify(s => s.SaveClientApplication(catalogueItemId, clientApplication));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SupportedBrowsers_RedirectsToBrowserBased(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            SupportedBrowsersModel model,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetClientApplication(catalogueItemId))
                .ReturnsAsync(clientApplication);

            var actual = (await controller.SupportedBrowsers(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.BrowserBased));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SupportedBrowsers_InvalidId_ReturnsBadRequestResult(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.SupportedBrowsers(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new SupportedBrowsersModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PlugInsOrExtensions_GetsSolutionFromService(
           CatalogueItemId catalogueItemId,
           CatalogueItem catalogueItem,
           [Frozen] Mock<ISolutionsService> mockService,
           CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            await controller.PlugInsOrExtensions(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PlugInsOrExtensions_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.PlugInsOrExtensions(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PlugInsOrExtensions_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.PlugInsOrExtensions(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new PlugInsOrExtensionsModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PlugInsOrExtensions_CallsSaveClientApplication(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            PlugInsOrExtensionsModel model,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetClientApplication(catalogueItemId))
                .ReturnsAsync(clientApplication);

            await controller.PlugInsOrExtensions(catalogueItemId, model);

            mockService.Verify(s => s.SaveClientApplication(catalogueItemId, clientApplication));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PlugInsOrExtensions_RedirectsToBrowserBased(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            PlugInsOrExtensionsModel model,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetClientApplication(catalogueItemId))
                .ReturnsAsync(clientApplication);

            var actual = (await controller.PlugInsOrExtensions(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.BrowserBased));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PlugInsOrExtensions_InvalidId_ReturnsBadRequestResult(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.PlugInsOrExtensions(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new PlugInsOrExtensionsModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConnectivityAndResolution_GetsSolutionFromService(
           CatalogueItemId catalogueItemId,
           CatalogueItem catalogueItem,
           [Frozen] Mock<ISolutionsService> mockService,
           CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            await controller.ConnectivityAndResolution(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConnectivityAndResolution_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.ConnectivityAndResolution(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConnectivityAndResolution_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.ConnectivityAndResolution(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new ConnectivityAndResolutionModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConnectivityAndResolution_CallsSaveClientApplication(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            ConnectivityAndResolutionModel model,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetClientApplication(catalogueItemId))
                .ReturnsAsync(clientApplication);

            await controller.ConnectivityAndResolution(catalogueItemId, model);

            mockService.Verify(s => s.SaveClientApplication(catalogueItemId, clientApplication));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConnectivityAndResolution_RedirectsToBrowserBased(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            ConnectivityAndResolutionModel model,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetClientApplication(catalogueItemId))
                .ReturnsAsync(clientApplication);

            var actual = (await controller.ConnectivityAndResolution(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.BrowserBased));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConnectivityAndResolution_InvalidId_ReturnsBadRequestResult(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.ConnectivityAndResolution(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new ConnectivityAndResolutionModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HardwareRequirements_GetsSolutionFromService(
           CatalogueItemId catalogueItemId,
           CatalogueItem catalogueItem,
           [Frozen] Mock<ISolutionsService> mockService,
           CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            await controller.HardwareRequirements(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HardwareRequirements_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.HardwareRequirements(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HardwareRequirements_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.HardwareRequirements(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new HardwareRequirementsModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HardwareRequirements_CallsSaveClientApplication(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            HardwareRequirementsModel model,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetClientApplication(catalogueItemId))
                .ReturnsAsync(clientApplication);

            await controller.HardwareRequirements(catalogueItemId, model);

            mockService.Verify(s => s.SaveClientApplication(catalogueItemId, clientApplication));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HardwareRequirements_RedirectsToBrowserBased(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            HardwareRequirementsModel model,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetClientApplication(catalogueItemId))
                .ReturnsAsync(clientApplication);

            var actual = (await controller.HardwareRequirements(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.BrowserBased));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HardwareRequirements_InvalidId_ReturnsBadRequestResult(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.HardwareRequirements(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new HardwareRequirementsModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalInformation_GetsSolutionFromService(
           CatalogueItemId catalogueItemId,
           CatalogueItem catalogueItem,
           [Frozen] Mock<ISolutionsService> mockService,
           CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            await controller.AdditionalInformation(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalInformation_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.AdditionalInformation(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalInformation_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.AdditionalInformation(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new AdditionalInformationModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalInformation_CallsSaveClientApplication(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            AdditionalInformationModel model,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetClientApplication(catalogueItemId))
                .ReturnsAsync(clientApplication);

            await controller.AdditionalInformation(catalogueItemId, model);

            mockService.Verify(s => s.SaveClientApplication(catalogueItemId, clientApplication));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalInformation_RedirectsToBrowserBased(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            AdditionalInformationModel model,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetClientApplication(catalogueItemId))
                .ReturnsAsync(clientApplication);

            var actual = (await controller.AdditionalInformation(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.BrowserBased));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalInformation_InvalidId_ReturnsBadRequestResult(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.AdditionalInformation(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new AdditionalInformationModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ClientApplicationType_GetsSolutionFromService(
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController catalogueSolutionsController)
        {
            mockService.Setup(s => s.GetSolution(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            await catalogueSolutionsController.ClientApplicationType(catalogueItem.Id);

            mockService.Verify(s => s.GetSolution(catalogueItem.Id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ClientApplicationType_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.ClientApplicationType(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new ClientApplicationTypeSectionModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ClientApplicationType_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.ClientApplicationType(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ClientApplicationType_InvalidId_ReturnsBadRequestResult(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.ClientApplicationType(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new ClientApplicationTypeSectionModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddApplicationType_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            await controller.AddApplicationType(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddApplicationType_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.AddApplicationType(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new ClientApplicationTypeSelectionModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddApplicationType_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            CatalogueSolutionsController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.AddApplicationType(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SetPublicationStatus_SamePublicationStatus_DoesNotCallSavePublicationStatus(
            CatalogueItem solution,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            CatalogueSolutionsController controller)
        {
            var manageCatalogueSolutionModel = new ManageCatalogueSolutionModel { SelectedPublicationStatus = solution.PublishedStatus };

            mockSolutionService.Setup(s => s.GetSolution(solution.Id))
                .ReturnsAsync(solution);

            await controller.SetPublicationStatus(solution.Id, manageCatalogueSolutionModel);

            mockSolutionService.Verify(s => s.SavePublicationStatus(solution.Id, manageCatalogueSolutionModel.SelectedPublicationStatus), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SetPublicationStatus_CallsSavePublicationStatus(
            CatalogueItem solution,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            CatalogueSolutionsController controller)
        {
            var newPublicationStatus = PublicationStatus.Published;
            solution.PublishedStatus = PublicationStatus.Draft;

            var manageCatalogueSolutionModel = new ManageCatalogueSolutionModel { SelectedPublicationStatus = newPublicationStatus };

            mockSolutionService.Setup(s => s.GetSolution(solution.Id))
                .ReturnsAsync(solution);

            await controller.SetPublicationStatus(solution.Id, manageCatalogueSolutionModel);

            mockSolutionService.Verify(s => s.SavePublicationStatus(solution.Id, manageCatalogueSolutionModel.SelectedPublicationStatus));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SetPublicationStatus_ClearsFilterCache(
            CatalogueItem solution,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            [Frozen] Mock<IFilterCache> mockFilterCache,
            CatalogueSolutionsController controller)
        {
            var newPublicationStatus = PublicationStatus.Published;
            solution.PublishedStatus = PublicationStatus.Draft;

            var manageCatalogueSolutionModel = new ManageCatalogueSolutionModel { SelectedPublicationStatus = newPublicationStatus };

            mockSolutionService.Setup(s => s.GetSolution(solution.Id))
                .ReturnsAsync(solution);

            await controller.SetPublicationStatus(solution.Id, manageCatalogueSolutionModel);

            mockFilterCache.Verify(f => f.Remove(It.IsAny<IEnumerable<string>>()));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SetPublicationStatus_ReturnsRedirectToActionResult(
            CatalogueItem solution,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            CatalogueSolutionsController controller)
        {
            var newPublicationStatus = PublicationStatus.Published;
            solution.PublishedStatus = PublicationStatus.Draft;

            var manageCatalogueSolutionModel = new ManageCatalogueSolutionModel { SelectedPublicationStatus = newPublicationStatus };

            mockSolutionService.Setup(s => s.GetSolution(solution.Id))
                .ReturnsAsync(solution);

            var actual = (await controller.SetPublicationStatus(solution.Id, manageCatalogueSolutionModel)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.Index));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SetPublicationStatus_InvalidModel_ReturnsViewWithModel(
            CatalogueItem solution,
            PublicationStatus publicationStatus,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            CatalogueSolutionsController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var manageCatalogueSolutionModel = new ManageCatalogueSolutionModel { SelectedPublicationStatus = publicationStatus };

            mockSolutionService.Setup(s => s.GetSolution(solution.Id))
                .ReturnsAsync(solution);

            var actual = (await controller.SetPublicationStatus(solution.Id, manageCatalogueSolutionModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.Model.Should().BeEquivalentTo(manageCatalogueSolutionModel);
        }
    }
}
