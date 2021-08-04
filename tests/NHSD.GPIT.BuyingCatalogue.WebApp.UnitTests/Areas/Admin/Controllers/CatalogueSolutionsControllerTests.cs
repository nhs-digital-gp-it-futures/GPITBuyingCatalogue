using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
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
        public static void Constructor_NullService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                    () =>
                        _ = new CatalogueSolutionsController(null, null))
                .ParamName.Should()
                .Be("solutionsService");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Features_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            [Frozen]Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(new CatalogueItem());
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            await controller.Features(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Features_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen]Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.Features(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new FeaturesModel().FromCatalogueItem(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Features_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen]Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.Features(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Features_CallsSavesSolutionFeatures(
            CatalogueItemId catalogueItemId,
            FeaturesModel model,
            [Frozen]Mock<ISolutionsService> mockService)
        {
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            await controller.Features(catalogueItemId, model);

            mockService.Verify(s => s.SaveSolutionFeatures(catalogueItemId, model.AllFeatures));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Features_RedirectsToManageCatalogueSolution(
            CatalogueItemId catalogueItemId,
            FeaturesModel model,
            [Frozen]Mock<ISolutionsService> mockService)
        {
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen]Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.Features(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new FeaturesModel().FromCatalogueItem(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_SolutionsInDatabase_ReturnsViewWithExpectedModel(
            List<CatalogueItem> solutions)
        {
            var expected = solutions.Select(s => new CatalogueModel(s)).ToList();
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetAllSolutions(null))
                .ReturnsAsync(solutions);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen]Mock<ISolutionsService> mockService,
            PublicationStatus status)
        {
            var model = new CatalogueSolutionsModel { SelectedPublicationStatus = status.ToString() };
            mockService.Setup(s => s.GetAllSolutions(status))
                .ReturnsAsync(solutions);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.Index(model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            model.SetSolutions(solutions);
            actual.Model.As<CatalogueSolutionsModel>().Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ManageSolution_ReturnsViewWithExpectedModel(
            CatalogueItem expected,
            AspNetUser aspNetUser,
            Guid userId)
        {
            expected.Solution.LastUpdatedBy = userId;
            aspNetUser.Id = userId.ToString();
            var mockSolutionService = new Mock<ISolutionsService>();
            mockSolutionService.Setup(s => s.GetSolution(expected.CatalogueItemId))
                .ReturnsAsync(expected);

            var mockUsersService = new Mock<IUsersService>();
            mockUsersService.Setup(u => u.GetUser(aspNetUser.Id))
                .ReturnsAsync(aspNetUser);
            var controller = new CatalogueSolutionsController(mockSolutionService.Object, mockUsersService.Object);

            var actual = (await controller.ManageCatalogueSolution(expected.CatalogueItemId)).As<ViewResult>();

            mockSolutionService.Verify(s => s.GetSolution(expected.CatalogueItemId));
            mockUsersService.Verify(u => u.GetUser(aspNetUser.Id));
            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            var model = actual.Model.As<ManageCatalogueSolutionModel>();
            model.Solution.Should().BeEquivalentTo(expected);
            model.LastUpdatedByName.Should().BeEquivalentTo($"{aspNetUser.FirstName} {aspNetUser.LastName}");
        }

        [Fact]
        public static async Task Get_Description_ReturnsViewWithExpectedModel()
        {
            var expected = new CatalogueItem
            {
                CatalogueItemId = new CatalogueItemId(999999, "999"),
                Solution = new Solution
                {
                    Summary = "XYZ Summary",
                    FullDescription = "XYZ description",
                    AboutUrl = "Fake url",
                },
                Name = "Fake Solution",
            };

            var mockSolutionService = new Mock<ISolutionsService>();
            var mockUserService = new Mock<IUsersService>();
            mockSolutionService.Setup(s => s.GetSolution(It.IsAny<CatalogueItemId>()))
                .ReturnsAsync(expected);

            var controller = new CatalogueSolutionsController(mockSolutionService.Object, mockUserService.Object);

            var actual = (await controller.Description(expected.CatalogueItemId)).As<ViewResult>();

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.Description(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SolutionDescription_InvalidModel_DoesNotCallService([Frozen] CatalogueItemId id)
        {
            var mockSolutionService = new Mock<ISolutionsService>();
            var mockUserService = new Mock<IUsersService>();
            var controller = new CatalogueSolutionsController(mockSolutionService.Object, mockUserService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.Description(id, new Mock<DescriptionModel>().Object);

            mockSolutionService.Verify(
                s => s.SaveSolutionDescription(It.IsAny<CatalogueItemId>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditDescription_InvalidModel_ReturnsViewWithModel([Frozen] CatalogueItemId id)
        {
            var mockEditDescriptionModel = new Mock<DescriptionModel>().Object;
            var controller = new CatalogueSolutionsController(Mock.Of<ISolutionsService>(), Mock.Of<IUsersService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.Description(id, mockEditDescriptionModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockEditDescriptionModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditDescription_ValidModel_CallsSaveSolutionDescriptionOnService(
            [Frozen] CatalogueItemId id,
            DescriptionModel model)
        {
            var mockSolutionService = new Mock<ISolutionsService>();
            var mockUserService = new Mock<IUsersService>();
            var controller = new CatalogueSolutionsController(mockSolutionService.Object, mockUserService.Object);

            await controller.Description(id, model);

            mockSolutionService.Verify(s => s.SaveSolutionDescription(id, model.Summary, model.Description, model.Link));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditDescription_ValidModel_RedirectsToExpectedAction(
            [Frozen] CatalogueItemId id,
            DescriptionModel model)
        {
            var controller = new CatalogueSolutionsController(Mock.Of<ISolutionsService>(), Mock.Of<IUsersService>());

            var actual = (await controller.Description(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.RouteValues["solutionId"].Should().Be(model.SolutionId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Implementation_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(new CatalogueItem());

            await new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>()).Implementation(
                catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Implementation_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.Implementation(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new ImplementationTimescaleModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Implementation_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.Implementation(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Implementation_CallsSaveImplementationDetail(
            CatalogueItemId catalogueItemId,
            ImplementationTimescaleModel model,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            await controller.Implementation(catalogueItemId, model);

            mockService.Verify(s => s.SaveImplementationDetail(catalogueItemId, It.IsAny<string>()));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Implementation_RedirectsToManageCatalogueSolution(
            CatalogueItemId catalogueItemId,
            ImplementationTimescaleModel model,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.Implementation(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new ImplementationTimescaleModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Roadmap_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            [Frozen]Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(new CatalogueItem());
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            await controller.Roadmap(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Roadmap_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen]Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.Roadmap(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new RoadmapModel().FromCatalogueItem(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Roadmap_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen]Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.Roadmap(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Roadmap_CallsSavesSolutionRoadmap(
            CatalogueItemId catalogueItemId,
            RoadmapModel model,
            [Frozen]Mock<ISolutionsService> mockService)
        {
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            await controller.Roadmap(catalogueItemId, model);

            mockService.Verify(s => s.SaveRoadMap(catalogueItemId, model.Link));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Roadmap_RedirectsToManageCatalogueSolution(
            CatalogueItemId catalogueItemId,
            RoadmapModel model,
            [Frozen]Mock<ISolutionsService> mockService)
        {
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen]Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            await new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>()).HostingType(
                catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HostingType_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.HostingType(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new HostingTypeSectionModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HostingType_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.HostingType(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HostingType_CallsSaveHosting(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            HostingTypeSectionModel model = new HostingTypeSectionModel(catalogueItem);

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            HostingTypeSectionModel model = new HostingTypeSectionModel(catalogueItem);

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            await controller.AddHostingType(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddHostingType_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.AddHostingType(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new HostingTypeSelectionModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddHostingType_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.AddHostingType(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddHostingType_RedirectsToPublicCloud(
            CatalogueItemId catalogueItemId,
            HostingTypeSelectionModel model,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            await controller.PublicCloud(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PublicCloud_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.PublicCloud(catalogueItemId)).As<ViewResult>();

            var hosting = catalogueItem.Solution?.GetHosting();
            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new PublicCloudModel(hosting?.PublicCloud));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PublicCloud_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.PublicCloud(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PublicCloud_CallsSaveHosting(
            CatalogueItemId catalogueItemId,
            PublicCloudModel model,
            Hosting hosting,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.PublicCloud(catalogueItemId)).As<ViewResult>();

            var hosting = catalogueItem.Solution?.GetHosting();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new PublicCloudModel(hosting.PublicCloud));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PrivateCloud_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            await controller.PrivateCloud(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PrivateCloud_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.PrivateCloud(catalogueItemId)).As<ViewResult>();

            var hosting = catalogueItem.Solution?.GetHosting();
            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new PrivateCloudModel(hosting?.PrivateCloud));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PrivateCloud_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.PrivateCloud(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PrivateCloud_CallsSaveHosting(
            CatalogueItemId catalogueItemId,
            PrivateCloudModel model,
            Hosting hosting,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.PrivateCloud(catalogueItemId)).As<ViewResult>();

            var hosting = catalogueItem.Solution?.GetHosting();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new PrivateCloudModel(hosting?.PrivateCloud));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HybridCloud_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            await controller.Hybrid(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HybridCloud_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.Hybrid(catalogueItemId)).As<ViewResult>();

            var hosting = catalogueItem.Solution?.GetHosting();
            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new HybridModel(hosting?.HybridHostingType));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HybridCloud_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.Hybrid(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HybridCloud_CallsSaveHosting(
            CatalogueItemId catalogueItemId,
            HybridModel model,
            Hosting hosting,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.Hybrid(catalogueItemId)).As<ViewResult>();

            var hosting = catalogueItem.Solution?.GetHosting();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new HybridModel(hosting?.HybridHostingType));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_OnPremiseCloud_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            await controller.OnPremise(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_OnPremise_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.OnPremise(catalogueItemId)).As<ViewResult>();

            var hosting = catalogueItem.Solution?.GetHosting();
            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new OnPremiseModel(hosting?.OnPremise));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_OnPremise_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.OnPremise(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_OnPremise_CallsSaveHosting(
            CatalogueItemId catalogueItemId,
            OnPremiseModel model,
            Hosting hosting,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.OnPremise(catalogueItemId)).As<ViewResult>();

            var hosting = catalogueItem.Solution?.GetHosting();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new OnPremiseModel(hosting?.OnPremise));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_BrowserBased_GetsSolutionFromService(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(new CatalogueItem());
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            await controller.BrowserBased(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_BrowserBased_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.BrowserBased(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_BrowserBased_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.BrowserBased(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new BrowserBasedModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_BrowserBased_CallsGetSolution(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            BrowserBasedModel model,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            await controller.BrowserBased(catalogueItemId, model);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_BrowserBased_RedirectsToManageCatalogueSolution(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            BrowserBasedModel model,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.BrowserBased(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupportedBrowsers_GetsSolutionFromService(
           CatalogueItemId catalogueItemId,
           CatalogueItem catalogueItem,
           [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            await controller.SupportedBrowsers(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupportedBrowsers_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.SupportedBrowsers(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupportedBrowsers_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetClientApplication(catalogueItemId))
                .ReturnsAsync(clientApplication);

            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetClientApplication(catalogueItemId))
                .ReturnsAsync(clientApplication);

            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
           [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            await controller.PlugInsOrExtensions(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PlugInsOrExtensions_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.PlugInsOrExtensions(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PlugInsOrExtensions_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetClientApplication(catalogueItemId))
                .ReturnsAsync(clientApplication);

            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetClientApplication(catalogueItemId))
                .ReturnsAsync(clientApplication);

            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
           [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            await controller.ConnectivityAndResolution(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConnectivityAndResolution_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.ConnectivityAndResolution(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConnectivityAndResolution_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetClientApplication(catalogueItemId))
                .ReturnsAsync(clientApplication);

            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetClientApplication(catalogueItemId))
                .ReturnsAsync(clientApplication);

            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
           [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            await controller.HardwareRequirements(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HardwareRequirements_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.HardwareRequirements(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HardwareRequirements_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetClientApplication(catalogueItemId))
                .ReturnsAsync(clientApplication);

            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetClientApplication(catalogueItemId))
                .ReturnsAsync(clientApplication);

            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
           [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            await controller.AdditionalInformation(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalInformation_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.AdditionalInformation(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalInformation_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetClientApplication(catalogueItemId))
                .ReturnsAsync(clientApplication);

            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetClientApplication(catalogueItemId))
                .ReturnsAsync(clientApplication);

            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            mockService.Setup(s => s.GetSolution(catalogueItem.CatalogueItemId))
                .ReturnsAsync(catalogueItem);

            await catalogueSolutionsController.ClientApplicationType(catalogueItem.CatalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItem.CatalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ClientApplicationType_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.ClientApplicationType(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new ClientApplicationTypeSectionModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ClientApplicationType_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.ClientApplicationType(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ClientApplicationType_CallsSaveClientApplication(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            ClientApplicationTypeSectionModel model = new ClientApplicationTypeSectionModel(catalogueItem);

            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            await controller.ClientApplicationType(catalogueItemId, model);

            mockService.Verify(s => s.SaveClientApplication(catalogueItemId, It.IsAny<ClientApplication>()));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ClientApplicationType_RedirectsToManageCatalogueSolution(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            ClientApplicationTypeSectionModel model = new ClientApplicationTypeSectionModel(catalogueItem);

            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.ClientApplicationType(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ClientApplicationType_InvalidId_ReturnsBadRequestResult(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            await controller.AddApplicationType(catalogueItemId);

            mockService.Verify(s => s.GetSolution(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddApplicationType_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.AddApplicationType(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new ClientApplicationTypeSelectionModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddApplicationType_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.AddApplicationType(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }
    }
}
