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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
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
            [Frozen] Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(new CatalogueItem());

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
            HostingTypeSectionModel model,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            await controller.HostingType(catalogueItemId, model);

            mockService.Verify(s => s.SaveHosting(catalogueItemId, It.IsAny<Hosting>()));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HostingType_RedirectsToManageCatalogueSolution(
            CatalogueItemId catalogueItemId,
            HostingTypeSectionModel model,
            [Frozen] Mock<ISolutionsService> mockService)
        {
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

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
    }
}
