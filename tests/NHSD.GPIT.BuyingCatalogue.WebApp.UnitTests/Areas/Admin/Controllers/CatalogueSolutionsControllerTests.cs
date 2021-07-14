using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EnumsNET;
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

            await new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>()).Features(
                catalogueItemId);

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
    }
}
