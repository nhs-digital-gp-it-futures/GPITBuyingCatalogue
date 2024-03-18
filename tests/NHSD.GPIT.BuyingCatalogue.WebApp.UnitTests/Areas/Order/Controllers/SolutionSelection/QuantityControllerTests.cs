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
using Microsoft.AspNetCore.Routing;
using Moq;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Quantity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.SolutionSelection
{
    public static class QuantityControllerTests
    {
        private const int NumberOfPatients = 1234;

        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(QuantityController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(QuantityController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(QuantityController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectQuantity_BadRequest(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            QuantityController controller)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);

            var orderItem = order.OrderItems.First();

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.SelectQuantity(internalOrgId, callOffId, catalogueItemId);

            mockOrderService.VerifyAll();

            result.Should().BeOfType<BadRequestResult>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectQuantity_ViewResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            QuantityController controller)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);

            var orderItem = order.OrderItems.First();

            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            orderItem.OrderItemPrice.ProvisioningType = ProvisioningType.Declarative;
            orderItem.OrderItemPrice.CataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerSolutionOrService;

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.SelectQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId);

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = actualResult.Model.Should().BeOfType<SelectOrderItemQuantityModel>().Subject;
            var expected = new SelectOrderItemQuantityModel(orderItem.CatalogueItem, orderItem.OrderItemPrice, orderItem.Quantity);

            model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonInlineAutoData(ProvisioningType.Patient, null)]
        [CommonInlineAutoData(ProvisioningType.OnDemand, CataloguePriceQuantityCalculationType.PerServiceRecipient)]
        [CommonInlineAutoData(ProvisioningType.Declarative, CataloguePriceQuantityCalculationType.PerServiceRecipient)]
        public static async Task Get_SelectQuantity_ProvisioningType_CataloguePriceQuantityCalculationType_PerServiceRecipient_Combination_Redirects(
            ProvisioningType provisioningType,
            CataloguePriceQuantityCalculationType? cataloguePriceQuantityCalculationType,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            QuantityController controller)
        {
            var orderItem = order.OrderItems.First();

            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            orderItem.OrderItemPrice.ProvisioningType = provisioningType;
            orderItem.OrderItemPrice.CataloguePriceQuantityCalculationType = cataloguePriceQuantityCalculationType;

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.SelectQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId);

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(QuantityController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(QuantityController.SelectServiceRecipientQuantity));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "catalogueItemId", orderItem.CatalogueItemId },
                { "source", null },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectQuantity_ModelError_ReturnsModel(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            SelectOrderItemQuantityModel model,
            QuantityController controller)
        {
            controller.ModelState.AddModelError("key", "message");

            var result = await controller.SelectQuantity(internalOrgId, callOffId, catalogueItemId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectQuantity_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            SelectOrderItemQuantityModel model,
            int quantity,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IOrderQuantityService> mockOrderQuantityService,
            [Frozen] Mock<IRoutingService> mockRoutingService,
            QuantityController controller)
        {
            var orderItem = order.OrderItems.First();

            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var orderWrapper = new OrderWrapper(order);
            mockOrderService
                .Setup(x => x.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId))
                .ReturnsAsync(orderWrapper);

            mockOrderQuantityService
                .Setup(x => x.SetOrderItemQuantity(order.Id, orderItem.CatalogueItemId, quantity))
                .Returns(Task.CompletedTask);

            mockRoutingService
                .Setup(x => x.GetRoute(RoutingPoint.SelectQuantity, orderWrapper, It.IsAny<RouteValues>()))
                .Returns(Route(internalOrgId, callOffId));

            model.Quantity = $"{quantity}";

            var result = await controller.SelectQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId, model);

            mockOrderService.VerifyAll();
            mockOrderQuantityService.VerifyAll();
            mockRoutingService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(AssociatedServicesController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(AssociatedServicesController.SelectAssociatedServices));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [CommonInlineAutoData(ProvisioningType.OnDemand, CataloguePriceQuantityCalculationType.PerServiceRecipient)]
        [CommonInlineAutoData(ProvisioningType.Declarative, CataloguePriceQuantityCalculationType.PerServiceRecipient)]
        public static async Task Get_SelectServiceRecipientQuantity_ProvisioningType_Not_Patient(
            ProvisioningType provisioningType,
            CataloguePriceQuantityCalculationType? cataloguePriceQuantityCalculationType,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IOdsService> odsService,
            QuantityController controller,
            string location)
        {
            var orderItem = order.OrderItems.First();

            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            orderItem.OrderItemPrice.ProvisioningType = provisioningType;
            orderItem.OrderItemPrice.CataloguePriceQuantityCalculationType = cataloguePriceQuantityCalculationType;
            order.OrderRecipients.ForEach(r => r.OrderItemRecipients.ForEach(x => x.Quantity = null));

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            odsService.Setup(x => x.GetServiceRecipientsById(internalOrgId, It.IsAny<IEnumerable<string>>())).ReturnsAsync(
                 order.OrderRecipients.Select(
                            x => new ServiceRecipient { OrgId = x.OdsCode, Location = location })
                        .ToList());

            var result = await controller.SelectServiceRecipientQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId);

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = actualResult.Model.Should().BeOfType<SelectServiceRecipientQuantityModel>().Subject;

            var recipients = order.OrderRecipients.Select(
                x => new ServiceRecipientDto(x.OdsCode, x.OdsOrganisation?.Name, x.GetQuantityForItem(orderItem.CatalogueItemId), location));

            var expected = new SelectServiceRecipientQuantityModel(
                order.OrderType,
                order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient,
                orderItem.CatalogueItem,
                orderItem.OrderItemPrice,
                recipients,
                null);
            expected.ServiceRecipients.ForEach(x => x.InputQuantity = string.Empty);

            model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectServiceRecipientQuantity_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IGpPracticeService> mockGpPracticeService,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IOdsService> odsService,
            QuantityController controller,
            string location)
        {
            var orderItem = order.OrderItems.First();

            orderItem.OrderItemPrice.ProvisioningType = ProvisioningType.Patient;
            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderRecipients.ForEach(r => r.OrderItemRecipients.ForEach(x => x.Quantity = null));

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockGpPracticeService
                .Setup(x => x.GetNumberOfPatients(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(
                    order.OrderRecipients.Select(
                            x => new GpPracticeSize { OdsCode = x.OdsCode, NumberOfPatients = NumberOfPatients })
                        .ToList());

            odsService.Setup(x => x.GetServiceRecipientsById(internalOrgId, It.IsAny<IEnumerable<string>>())).ReturnsAsync(
                 order.OrderRecipients.Select(
                            x => new ServiceRecipient { OrgId = x.OdsCode, Location = location })
                        .ToList());

            var result = await controller.SelectServiceRecipientQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId);

            mockOrderService.VerifyAll();
            mockGpPracticeService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = actualResult.Model.Should().BeOfType<SelectServiceRecipientQuantityModel>().Subject;

            var recipients = order.OrderRecipients.Select(
                x => new ServiceRecipientDto(x.OdsCode, x.OdsOrganisation?.Name, x.GetQuantityForItem(orderItem.CatalogueItemId), location));

            var expected = new SelectServiceRecipientQuantityModel(
                order.OrderType,
                order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient,
                orderItem.CatalogueItem,
                orderItem.OrderItemPrice,
                recipients,
                null);
            expected.ServiceRecipients.ForEach(x => x.InputQuantity = $"{NumberOfPatients}");

            model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectServiceRecipientQuantity_WithPrePopulatedSolutionRecipients_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IOdsService> odsService,
            QuantityController controller,
            string location)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            solution.OrderItemPrice.ProvisioningType = ProvisioningType.Patient;
            order.OrderRecipients.ForEach(r => r.SetQuantityForItem(solution.CatalogueItemId, NumberOfPatients));

            var orderItem = order.OrderItems.ElementAt(1);

            orderItem.OrderItemPrice.ProvisioningType = ProvisioningType.Patient;
            order.OrderRecipients.ForEach(r => r.OrderItemRecipients.Where(i => i.CatalogueItemId == orderItem.CatalogueItemId).ForEach(x => x.Quantity = null));

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            odsService.Setup(x => x.GetServiceRecipientsById(internalOrgId, It.IsAny<IEnumerable<string>>())).ReturnsAsync(
                 order.OrderRecipients.Select(
                            x => new ServiceRecipient { OrgId = x.OdsCode, Location = location })
                        .ToList());

            var result = await controller.SelectServiceRecipientQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId);

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = actualResult.Model.Should().BeOfType<SelectServiceRecipientQuantityModel>().Subject;

            var recipients = order.OrderRecipients.Select(
                x => new ServiceRecipientDto(x.OdsCode, x.OdsOrganisation?.Name, x.GetQuantityForItem(orderItem.CatalogueItemId), location));

            var expected = new SelectServiceRecipientQuantityModel(
                order.OrderType,
                order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient,
                orderItem.CatalogueItem,
                orderItem.OrderItemPrice,
                recipients,
                null);

            expected.ServiceRecipients.ForEach(x => x.InputQuantity = $"{NumberOfPatients}");

            model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectServiceRecipientQuantity_ModelError_ReturnsModel(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            SelectServiceRecipientQuantityModel model,
            QuantityController controller)
        {
            controller.ModelState.AddModelError("key", "message");

            var result = await controller.SelectServiceRecipientQuantity(internalOrgId, callOffId, catalogueItemId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectServiceRecipientQuantity_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            SelectServiceRecipientQuantityModel model,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IOrderQuantityService> mockOrderQuantityService,
            [Frozen] Mock<IRoutingService> mockRoutingService,
            QuantityController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var orderItem = order.OrderItems.First();

            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var orderWrapper = new OrderWrapper(order);
            mockOrderService
                .Setup(x => x.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId))
                .ReturnsAsync(orderWrapper);

            List<OrderItemRecipientQuantityDto> actual = null;

            mockOrderQuantityService
                .Setup(x => x.SetServiceRecipientQuantities(order.Id, orderItem.CatalogueItemId, It.IsAny<List<OrderItemRecipientQuantityDto>>()))
                .Callback<int, CatalogueItemId, List<OrderItemRecipientQuantityDto>>((_, _, x) => actual = x)
                .Returns(Task.CompletedTask);

            mockRoutingService
                .Setup(x => x.GetRoute(RoutingPoint.SelectQuantity, orderWrapper, It.IsAny<RouteValues>()))
                .Returns(Route(internalOrgId, callOffId));

            model.ServiceRecipients.ForEach(x => x.InputQuantity = x.Quantity > 0 ? string.Empty : "1");

            var result = await controller.SelectServiceRecipientQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId, model);

            mockOrderService.VerifyAll();
            mockOrderQuantityService.VerifyAll();

            foreach (var dto in actual)
            {
                model.ServiceRecipients
                    .First(x => x.OdsCode == dto.OdsCode)
                    .Quantity.Should().Be(dto.Quantity == 0 ? 1 : dto.Quantity);
            }

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(AssociatedServicesController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(AssociatedServicesController.SelectAssociatedServices));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [CommonInlineAutoData(ProvisioningType.Patient)]
        public static async Task Get_ViewOrderItemQuantity_PerServiceRecipientPrice_ExpectedResult(
            ProvisioningType provisioningType,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            EntityFramework.Ordering.Models.Order amendment,
            [Frozen] Mock<IOrderService> orderService,
            QuantityController controller)
        {
            order.Revision = 1;
            amendment.OrderNumber = order.OrderNumber;
            amendment.Revision = 2;

            var orderItem = order.OrderItems.First();

            orderItem.OrderItemPrice.ProvisioningType = provisioningType;
            orderItem.OrderItemPrice.CataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerServiceRecipient;

            orderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(new[] { order, amendment }));

            var result = await controller.ViewOrderItemQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId);

            orderService.VerifyAll();

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ControllerName.Should().Be(typeof(QuantityController).ControllerName());
            actual.ActionName.Should().Be(nameof(QuantityController.ViewServiceRecipientQuantity));
            actual.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "catalogueItemId", orderItem.CatalogueItemId },
            });
        }

        [Theory]
        [CommonInlineAutoData(ProvisioningType.Declarative)]
        [CommonInlineAutoData(ProvisioningType.OnDemand)]
        public static async Task Get_ViewOrderItemQuantity_ExpectedResult(
            ProvisioningType provisioningType,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            EntityFramework.Ordering.Models.Order amendment,
            [Frozen] Mock<IOrderService> orderService,
            QuantityController controller)
        {
            order.Revision = 1;
            amendment.OrderNumber = order.OrderNumber;
            amendment.Revision = 2;

            var orderItem = order.OrderItems.First();

            orderItem.OrderItemPrice.ProvisioningType = provisioningType;
            orderItem.OrderItemPrice.CataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerSolutionOrService;

            orderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(new[] { order, amendment }));

            var result = await controller.ViewOrderItemQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId);

            orderService.VerifyAll();

            var actual = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new ViewOrderItemQuantityModel(orderItem)
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            actual.Should().NotBeNull();
            actual.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ViewServiceRecipientQuantity_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            EntityFramework.Ordering.Models.Order amendment,
            [Frozen] Mock<IOrderService> orderService,
            QuantityController controller)
        {
            order.Revision = 1;
            amendment.OrderNumber = order.OrderNumber;
            amendment.Revision = 2;

            var orderItem = order.OrderItems.First();

            orderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(new[] { order, amendment }));

            var result = await controller.ViewServiceRecipientQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId);

            orderService.VerifyAll();

            var actual = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new ViewServiceRecipientQuantityModel(orderItem, order.OrderRecipients)
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            actual.Should().NotBeNull();
            actual.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        private static RoutingResult Route(string internalOrgId, CallOffId callOffId) => new()
        {
            ActionName = Constants.Actions.SelectAssociatedServices,
            ControllerName = Constants.Controllers.AssociatedServices,
            RouteValues = new { internalOrgId, callOffId },
        };
    }
}
