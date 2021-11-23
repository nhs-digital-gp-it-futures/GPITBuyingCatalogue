using System.Collections.Generic;
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
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class AssociatedServicesControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(AssociatedServicesController).Should()
                .BeDecoratedWith<AuthorizeAttribute>(a => a.Policy == "AdminOnly");
            typeof(AssociatedServicesController).Should()
                .BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Admin");
            typeof(AssociatedServicesController).Should()
                .BeDecoratedWith<RouteAttribute>(r => r.Template == "admin/catalogue-solutions/manage/{solutionId}/associated-services");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(AssociatedServicesController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AssociatedServices_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            List<AssociatedService> associatedServices,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            [Frozen] Mock<IAssociatedServicesService> mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            mockSolutionService.Setup(s => s.GetSolution(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var catalogueItems = associatedServices.Select(a => a.CatalogueItem).ToList();
            mockAssociatedServicesService.Setup(s => s.GetAssociatedServicesForSupplier(catalogueItem.Supplier.Id))
                .ReturnsAsync(catalogueItems);

            var actual = await controller.AssociatedServices(catalogueItem.Id);

            actual.Should().BeOfType<ViewResult>();

            mockSolutionService.Verify(s => s.GetSolution(catalogueItem.Id));
            actual.As<ViewResult>().ViewName.Should().BeNull();
            actual.As<ViewResult>().Model.Should().BeEquivalentTo(new AssociatedServicesModel(catalogueItem, catalogueItems), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AssociatedServices_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            AssociatedServicesController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = await controller.AssociatedServices(catalogueItemId);

            actual.Should().BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AssociatedServices_Saves_And_RedirectsToDesktop(
            CatalogueItemId catalogueItemId,
            AssociatedServicesModel model,
            [Frozen] Mock<IAssociatedServicesService> mockService,
            AssociatedServicesController controller)
        {
            var actual = await controller.AssociatedServices(catalogueItemId, model);

            mockService.Verify(s => s.RelateAssociatedServicesToSolution(catalogueItemId, It.Is<IEnumerable<CatalogueItemId>>(
                l => l.SequenceEqual(model.SelectableAssociatedServices
                    .Where(a => a.Selected)
                    .Select(a => a.CatalogueItemId)))));

            actual.Should().BeOfType<RedirectToActionResult>();

            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
            actual.As<RedirectToActionResult>().RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddAssociatedService_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            AssociatedServicesController controller)
        {
            mockSolutionService.Setup(s => s.GetSolution(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var actual = await controller.AddAssociatedService(catalogueItem.Id);

            mockSolutionService.Verify(s => s.GetSolution(catalogueItem.Id));

            actual.Should().BeOfType<ViewResult>();

            actual.As<ViewResult>().ViewName.Should().BeNull();
            actual.As<ViewResult>().Model.Should().BeEquivalentTo(new AddAssociatedServiceModel(catalogueItem), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddAssociatedService_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            AssociatedServicesController controller)
        {
            mockService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = await controller.AddAssociatedService(catalogueItemId);

            actual.Should().BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditAssociatedService_ValidIds_ReturnsViewWithExpectedModel(
            Solution solution,
            AssociatedService associatedService,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            [Frozen] Mock<IAssociatedServicesService> mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            associatedService.CatalogueItem.PublishedStatus = PublicationStatus.Draft;

            mockSolutionService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            mockAssociatedServicesService.Setup(s => s.GetAssociatedService(associatedService.CatalogueItemId))
                .ReturnsAsync(associatedService.CatalogueItem);

            var expectedModel = new EditAssociatedServiceModel(solution.CatalogueItem, associatedService.CatalogueItem);

            var actual = await controller.EditAssociatedService(solution.CatalogueItemId, associatedService.CatalogueItemId);

            mockSolutionService.Verify(s => s.GetSolution(solution.CatalogueItemId));
            mockAssociatedServicesService.Verify(a => a.GetAssociatedService(associatedService.CatalogueItemId));

            actual.Should().BeOfType<ViewResult>();

            actual.As<ViewResult>().ViewName.Should().BeNull();
            actual.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditAssociatedService_InvalidSolutionId_ReturnsBadRequestResult(
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            AssociatedServicesController controller)
        {
            mockSolutionService.Setup(s => s.GetSolution(solutionId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = await controller.EditAssociatedService(solutionId, associatedServiceId);

            actual.Should().BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditAssociatedService_InvalidAssociatedServiceId_ReturnsBadRequestResult(
            CatalogueItem solution,
            CatalogueItemId associatedServiceId,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            [Frozen] Mock<IAssociatedServicesService> mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            mockSolutionService.Setup(s => s.GetSolution(solution.Id))
                .ReturnsAsync(solution);

            mockAssociatedServicesService.Setup(s => s.GetAssociatedService(associatedServiceId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = await controller.EditAssociatedService(solution.Id, associatedServiceId);

            actual.Should().BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Associated Service found for Id: {associatedServiceId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditAssociatedServiceDetails_ValidIds_ReturnsViewWithExpectedModel(
            CatalogueItem solution,
            AssociatedService associatedService,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            [Frozen] Mock<IAssociatedServicesService> mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            mockSolutionService.Setup(s => s.GetSolution(solution.Id))
                .ReturnsAsync(solution);

            var catalogueItem = associatedService.CatalogueItem;
            mockAssociatedServicesService.Setup(s => s.GetAssociatedService(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var actual = await controller.EditAssociatedServiceDetails(solution.Id, catalogueItem.Id);

            mockSolutionService.Verify(s => s.GetSolution(solution.Id));
            mockAssociatedServicesService.Verify(a => a.GetAssociatedService(catalogueItem.Id));

            actual.Should().BeOfType<ViewResult>();

            actual.As<ViewResult>().ViewName.Should().BeNull();
            actual.As<ViewResult>().Model.Should().BeEquivalentTo(new EditAssociatedServiceDetailsModel(solution, catalogueItem), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditAssociatedServiceDetails_InvalidSolutionId_ReturnsBadRequestResult(
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            AssociatedServicesController controller)
        {
            mockSolutionService.Setup(s => s.GetSolution(solutionId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = await controller.EditAssociatedServiceDetails(solutionId, associatedServiceId);

            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditAssociatedServicesDetails_InvalidAssociatedServiceId_ReturnsBadRequestResult(
            [Frozen] Mock<IAssociatedServicesService> mockAssociatedService,
            AssociatedServicesController controller,
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId)
        {
            mockAssociatedService.Setup(s => s.GetAssociatedService(associatedServiceId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = await controller.EditAssociatedServiceDetails(solutionId, associatedServiceId);

            actual.Should().BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Associated Service found for Id: {associatedServiceId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditAssociatedServiceDetails_Valid_ReturnsViewWithExpectedRouteValues(
            [Frozen] Mock<IAssociatedServicesService> mockAssociatedServicesService,
            AssociatedServicesController controller,
            CatalogueItem solution,
            AssociatedService associatedService)
        {
            var catalogueItem = associatedService.CatalogueItem;
            var model = new EditAssociatedServiceDetailsModel(solution, catalogueItem);

            mockAssociatedServicesService.Setup(s => s.EditDetails(
                catalogueItem.Id,
                new AssociatedServicesDetailsModel
                {
                    Name = model.Name,
                    Description = model.Description,
                    OrderGuidance = model.OrderGuidance,
                    UserId = It.IsAny<int>(),
                }));

            var actual = await controller.EditAssociatedServiceDetails(solution.Id, catalogueItem.Id, model);

            actual.Should().BeOfType<RedirectToActionResult>();

            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(AssociatedServicesController.EditAssociatedService));
            actual.As<RedirectToActionResult>().RouteValues["solutionId"].Should().Be(solution.Id);
            actual.As<RedirectToActionResult>().RouteValues["associatedServiceId"].Should().Be(catalogueItem.Id);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditAssociatedServiceDetails_InvalidSolutionId_ReturnsBadRequestResult(
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            AssociatedServicesController controller,
            EditAssociatedServiceDetailsModel model,
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId)
        {
            mockSolutionService.Setup(s => s.GetSolution(solutionId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = await controller.EditAssociatedServiceDetails(solutionId, associatedServiceId, model);

            actual.Should().BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditAssociatedServiceDetails_InvalidAssociatedServiceId_ReturnsBadRequestResult(
            [Frozen] Mock<IAssociatedServicesService> mockService,
            AssociatedServicesController controller,
            EditAssociatedServiceDetailsModel model,
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId)
        {
            mockService.Setup(s => s.GetAssociatedService(associatedServiceId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = await controller.EditAssociatedServiceDetails(solutionId, associatedServiceId, model);

            actual.Should().BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Associated Service found for Id: {associatedServiceId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditAssociatedServiceDetails_MatchingName_ReturnsModelError(
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            [Frozen] Mock<IAssociatedServicesService> mockAssociatedServicesService,
            AssociatedServicesController controller,
            CatalogueItem item,
            CatalogueItem solution,
            AssociatedService associatedService,
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId)
        {
            mockSolutionService.Setup(s => s.GetSolution(solutionId))
                    .ReturnsAsync(solution);

            var catalogueItem = associatedService.CatalogueItem;

            mockAssociatedServicesService.Setup(s => s.GetAssociatedService(associatedServiceId))
                .ReturnsAsync(catalogueItem);

            mockAssociatedServicesService.Setup(s =>
                s.AssociatedServiceExistsWithNameForSupplier(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CatalogueItemId>()))
                .ReturnsAsync(true);

            var model = new EditAssociatedServiceDetailsModel(solution, catalogueItem);

            item.Id = solutionId;
            item.Name = model.Name;

            var actual = await controller.EditAssociatedServiceDetails(solutionId, associatedServiceId, model);

            actual.Should().BeOfType<ViewResult>();

            actual.As<ViewResult>().Model.Should().NotBeNull();
            actual.As<ViewResult>().Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ManageListPrices_ValidId_ReturnsCataloguePrice(
            CatalogueItemId catalogueItemId,
            CatalogueItemId associatedServiceId,
            CatalogueItem catalogueItem,
            CatalogueItem associatedService,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            [Frozen] Mock<IAssociatedServicesService> mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            mockSolutionsService
                .Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockAssociatedServicesService
                .Setup(s => s.GetAssociatedService(associatedServiceId))
                .ReturnsAsync(associatedService);

            var actual = (await controller.ManageListPrices(catalogueItemId, associatedServiceId)).As<ViewResult>();

            actual.ViewName.Should().BeNull();

            var manageListPricesModel = actual.Model.As<ManageListPricesModel>();
            manageListPricesModel.CataloguePrices.Should().BeEquivalentTo(associatedService.CataloguePrices);
            manageListPricesModel.CatalogueItemId.Should().Be(associatedService.Id);
            manageListPricesModel.CatalogueName.Should().Be(associatedService.Name);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ManageListPrices_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId associatedServiceId,
            [Frozen] Mock<IAssociatedServicesService> mockAssociatedServicesService,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            AssociatedServicesController controller,
            CatalogueItem catalogueItem)
        {
            mockSolutionService
                .Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockAssociatedServicesService
                .Setup(s => s.GetAssociatedService(associatedServiceId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.ManageListPrices(catalogueItemId, associatedServiceId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Associated Service found for Id: {associatedServiceId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddListPrice_ValidId_ReturnsModel(
            CatalogueItemId catalogueItemId,
            CatalogueItemId associatedServiceId,
            [Frozen] Mock<IAssociatedServicesService> mockAssociatedServicesService,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            AssociatedServicesController controller,
            CatalogueItem catalogueItem,
            CatalogueItem associatedService)
        {
            mockSolutionsService
                .Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockAssociatedServicesService
                .Setup(s => s.GetAssociatedService(associatedServiceId))
                .ReturnsAsync(associatedService);

            var actual = await controller.AddListPrice(catalogueItemId, associatedServiceId);

            mockSolutionsService.Verify(s => s.GetSolution(catalogueItemId));
            actual.Should().NotBeNull();
            actual.Should().BeOfType<ViewResult>();

            var model = actual.As<ViewResult>().Model.As<EditListPriceModel>();
            model.ItemId.Should().Be(associatedService.Id);
            model.ItemName.Should().Be(associatedService.Name);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddListPrice_InvalidSolutionId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId associatedServiceId,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            AssociatedServicesController controller)
        {
            mockSolutionsService
                .Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.AddListPrice(catalogueItemId, associatedServiceId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddListPrice_InvalidAssociatedServiceId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId associatedServiceId,
            [Frozen] Mock<IAssociatedServicesService> mockAssociatedServicesService,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            AssociatedServicesController controller,
            CatalogueItem catalogueItem)
        {
            mockSolutionsService
                .Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockAssociatedServicesService
                .Setup(s => s.GetAssociatedService(associatedServiceId))
                .ReturnsAsync((CatalogueItem)null);

            var actual = (await controller.AddListPrice(catalogueItemId, associatedServiceId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Associated Service found for Id: {associatedServiceId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddListPrice_ModelStateValid_RedirectsToManageListPrices(
            CatalogueItemId catalogueItemId,
            CatalogueItemId associatedServiceId,
            [Frozen] Mock<IAssociatedServicesService> mockAssociatedServicesService,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            AssociatedServicesController controller,
            CatalogueItem catalogueItem)
        {
            const decimal price = 3.21M;
            var solutionId = associatedServiceId;
            var editListPriceModel = new EditListPriceModel(catalogueItem)
            {
                Price = price,
                SelectedProvisioningType = ProvisioningType.Patient,
                Unit = "per patient",
            };

            mockSolutionsService
                .Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockAssociatedServicesService
                .Setup(s => s.GetAssociatedService(associatedServiceId))
                .ReturnsAsync(catalogueItem);

            var actual = await controller.AddListPrice(solutionId, associatedServiceId, editListPriceModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ControllerName.Should().BeNull();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(AssociatedServicesController.ManageListPrices));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditListPrice_ValidId_ReturnsModel(
            CatalogueItemId catalogueItemId,
            CatalogueItemId associatedServiceId,
            [Frozen] Mock<IListPricesService> mockListPricesService,
            AssociatedServicesController controller,
            CatalogueItem catalogueItem)
        {
            var cataloguePriceId = catalogueItem
                .CataloguePrices
                .First()
                .CataloguePriceId;

            mockListPricesService
                .Setup(s => s.GetCatalogueItemWithPrices(associatedServiceId))
                .ReturnsAsync(catalogueItem);

            var actual = await controller.EditListPrice(catalogueItemId, associatedServiceId, cataloguePriceId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<ViewResult>();
            var model = actual.As<ViewResult>().Model;
            model.Should().BeOfType<EditListPriceModel>();

            model.As<EditListPriceModel>().ItemName.Should().Be(catalogueItem.Name);
            model.As<EditListPriceModel>().CataloguePriceId.Should().Be(cataloguePriceId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditListPrice_InvalidListPriceId_RedirectsToManageListPrices(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] Mock<IListPricesService> mockListPricesService,
            AssociatedServicesController controller)
        {
            catalogueItem.CataloguePrices.Clear();

            const int cataloguePriceId = int.MaxValue;

            mockListPricesService
                .Setup(s => s.GetCatalogueItemWithPrices(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = await controller.EditListPrice(catalogueItemId, catalogueItemId, cataloguePriceId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ControllerName.Should().BeNull();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(AssociatedServicesController.ManageListPrices));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditListPrice_ModelStateValid_RedirectsToManageListPrices(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] Mock<IListPricesService> mockListPricesService,
            AssociatedServicesController controller)
        {
            const decimal price = 3.21M;
            var cataloguePriceId = catalogueItem
                .CataloguePrices
                .First()
                .CataloguePriceId;

            var editListPriceModel = new EditListPriceModel(catalogueItem)
            {
                CataloguePriceId = cataloguePriceId,
                Price = price,
                SelectedProvisioningType = ProvisioningType.Patient,
                Unit = "per patient",
            };

            var actual = await controller.EditListPrice(catalogueItemId, catalogueItem.Id, cataloguePriceId, editListPriceModel);

            mockListPricesService.Verify(s => s.UpdateListPrice(catalogueItem.Id, It.IsAny<SaveListPriceModel>()));

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ControllerName.Should().BeNull();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(AssociatedServicesController.ManageListPrices));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SetPublicationStatus_SamePublicationStatus_DoesNotCallSavePublicationStatus(
            CatalogueItem catalogueItem,
            EditAssociatedServiceModel model,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            [Frozen] Mock<IAssociatedServicesService> mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            model.SelectedPublicationStatus = catalogueItem.PublishedStatus;

            mockSolutionService.Setup(s => s.GetSolution(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            mockAssociatedServicesService.Setup(s => s.GetAssociatedService(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            await controller.SetPublicationStatus(catalogueItem.Id, catalogueItem.Id, model);

            mockSolutionService.Verify(s => s.SavePublicationStatus(catalogueItem.Id, model.SelectedPublicationStatus), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SetPublicationStatus_CallsSavePublicationStatus(
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            [Frozen] Mock<IAssociatedServicesService> mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            catalogueItem.PublishedStatus = PublicationStatus.Draft;

            var model = new EditAssociatedServiceModel { SelectedPublicationStatus = PublicationStatus.Published };

            mockSolutionService.Setup(s => s.GetSolution(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            mockAssociatedServicesService.Setup(s => s.GetAssociatedService(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            await controller.SetPublicationStatus(catalogueItem.Id, catalogueItem.Id, model);

            mockAssociatedServicesService.Verify(s => s.SavePublicationStatus(catalogueItem.Id, model.SelectedPublicationStatus));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SetPublicationStatus_ReturnsRedirectToActionResult(
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            [Frozen] Mock<IAssociatedServicesService> mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            catalogueItem.PublishedStatus = PublicationStatus.Draft;

            var model = new EditAssociatedServiceModel { SelectedPublicationStatus = PublicationStatus.Published };

            mockSolutionService.Setup(s => s.GetSolution(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            mockAssociatedServicesService.Setup(s => s.GetAssociatedService(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.SetPublicationStatus(catalogueItem.Id, catalogueItem.Id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(AssociatedServicesController.AssociatedServices));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SetPublicationStatus_InvalidModel_ReturnsViewWithModel(
            Solution solution,
            AssociatedService associatedService,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            [Frozen] Mock<IAssociatedServicesService> mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var model = new EditAssociatedServiceModel(solution.CatalogueItem, associatedService.CatalogueItem);

            mockSolutionService.Setup(s => s.GetSolution(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            mockAssociatedServicesService.Setup(s => s.GetAssociatedService(associatedService.CatalogueItemId))
                .ReturnsAsync(associatedService.CatalogueItem);

            var actual = (await controller.SetPublicationStatus(solution.CatalogueItemId, associatedService.CatalogueItemId, model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().Be(nameof(AssociatedServicesController.EditAssociatedService));
            actual.Model.Should().BeEquivalentTo(model);
        }
    }
}
