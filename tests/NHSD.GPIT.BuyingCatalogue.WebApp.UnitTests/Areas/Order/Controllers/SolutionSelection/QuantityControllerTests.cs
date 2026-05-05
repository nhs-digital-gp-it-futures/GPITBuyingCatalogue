using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
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

        [Theory]
        [MockAutoData]
        public static async Task Get_SublocationHub_ReturnsViewWithModel(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            Solution solution,
            [Frozen] IOrderService orderService,
            QuantityController controller)
        {
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var wrapper = new OrderWrapper(order);

            var orderItem = order.OrderItems.First();
            orderItem.CatalogueItem = solution.CatalogueItem;

            orderService.GetOrderWithOrderItems(order.CallOffId, internalOrgId).Returns(wrapper);

            var orderRecipients = wrapper.DetermineOrderRecipients(orderItem.CatalogueItemId);
            var orderRecipientDtos = QuantityController.GetRecipientDtos(orderRecipients, orderItem);

            var expectedModel = new SublocationQuantityHubModel(
                order.OrderingParty,
                orderItem.CatalogueItem,
                orderItem.OrderItemPrice)
            {
                Caption = $"Order {order.CallOffId}",
                SubLocations = CreateSublocationHelper.CreateSubLocations(orderRecipientDtos)
                    .Select(sublocation => new SubLocationModel(sublocation)
                    {
                        ForwardingLink = "testUrl",
                    })
                    .ToArray(),
            };

            var result = await controller.SublocationHub(internalOrgId, order.CallOffId, solution.CatalogueItemId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = actualResult.Model.Should().BeOfType<SublocationQuantityHubModel>().Subject;

            model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SublocationHub_AllRecipientsCompleted_RedirectsToConfirmationPage(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            OrderItem solution,
            SublocationQuantityHubModel model,
            [Frozen] IOrderService orderService,
            QuantityController controller)
        {
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var wrapper = new OrderWrapper(order);

            var orderItem = order.OrderItems.First();
            orderItem.CatalogueItem = solution.CatalogueItem;

            orderService.GetOrderWithOrderItems(order.CallOffId, internalOrgId).Returns(wrapper);

            var orderRecipients = wrapper.DetermineOrderRecipients(orderItem.CatalogueItemId);

            orderRecipients.ForEach(x => x.SetQuantityForItem(solution, 5));

            var result = await controller.SublocationHub(
                internalOrgId,
                order.CallOffId,
                solution.CatalogueItemId,
                model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.Should().NotBeNull();
            actualResult.ActionName.Should().Be(nameof(controller.ConfirmQuantities));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SublocationHub_IncompleteRecipients_RedirectsToOrderTaskList(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            OrderItem solution,
            SublocationQuantityHubModel model,
            [Frozen] IOrderService orderService,
            QuantityController controller)
        {
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var wrapper = new OrderWrapper(order);

            var orderItem = order.OrderItems.First();
            orderItem.CatalogueItem = solution.CatalogueItem;
            orderItem.CatalogueItemId = solution.CatalogueItemId;

            orderService.GetOrderWithOrderItems(order.CallOffId, internalOrgId).Returns(wrapper);

            var orderRecipients = wrapper.DetermineOrderRecipients(orderItem.CatalogueItemId);

            orderRecipients.First().SetQuantityForItem(solution, null);

            var result = await controller.SublocationHub(
                internalOrgId,
                order.CallOffId,
                solution.CatalogueItemId,
                model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.Should().NotBeNull();
            actualResult.ActionName.Should().Be(nameof(OrderController.Order));
            actualResult.ControllerName.Should().Be(typeof(OrderController).ControllerName());
        }

        [Theory]
        [MockInlineAutoData(ProvisioningType.OnDemand, CataloguePriceQuantityCalculationType.PerServiceRecipient)]
        [MockInlineAutoData(ProvisioningType.Declarative, CataloguePriceQuantityCalculationType.PerServiceRecipient)]
        public static async Task Get_SelectServiceSublocationRecipientQuantity_ProvisioningType_Not_Patient(
            ProvisioningType provisioningType,
            CataloguePriceQuantityCalculationType? cataloguePriceQuantityCalculationType,
            string internalOrgId,
            CallOffId callOffId,
            string parentOdsCode,
            EntityFramework.Ordering.Models.Order order,
            RoutingResult routingResult,
            [Frozen] IRoutingService routingService,
            [Frozen] IOrderService mockOrderService,
            QuantityController controller)
        {
            var orderItem = order.OrderItems.First();

            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            orderItem.OrderItemPrice.ProvisioningType = provisioningType;
            orderItem.OrderItemPrice.CataloguePriceQuantityCalculationType = cataloguePriceQuantityCalculationType;
            order.FlattenedRecipients.ForEach(r => r.OrderItemSublocationRecipients.ForEach(x => x.Quantity = null));

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            routingService.GetRoute(
                    RoutingPoint.SelectQuantityBackLink,
                    Arg.Any<OrderWrapper>(),
                    Arg.Any<RouteValues>())
                .Returns(routingResult);

            var result = await controller.SelectServiceSublocationRecipientQuantity(
                internalOrgId,
                callOffId,
                orderItem.CatalogueItemId,
                parentOdsCode);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = actualResult.Model.Should().BeOfType<SelectServiceRecipientQuantityModel>().Subject;

            IEnumerable<ServiceRecipientQuantityDto> recipients = order.FlattenedRecipients
                .Where(recipient => recipient.ParentSublocationOdsCode == parentOdsCode)
                .Select(x =>
                    new ServiceRecipientQuantityDto(
                        x.ParentSublocationOdsCode,
                        x.RecipientOdsCode,
                        x.RecipientOdsOrganisation?.Name,
                        x.GetQuantityForItem(orderItem.CatalogueItemId),
                        x.ParentSublocation.SublocationOrganisation.Name));

            var expected = new SelectServiceRecipientQuantityModel(
                order.OrderType,
                order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient,
                orderItem.CatalogueItem,
                orderItem.OrderItemPrice,
                recipients,
                null);
            expected.SubLocations.ForEach(x => x.ServiceRecipients.ForEach(y => y.InputQuantity = string.Empty));

            model.Should()
                .BeEquivalentTo(
                    expected,
                    x => x
                        .Excluding(m => m.BackLink)
                        .Excluding(m => m.Title));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SelectServiceSublocationRecipientQuantity_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            string parentOdsCode,
            EntityFramework.Ordering.Models.Order order,
            RoutingResult routingResult,
            [Frozen] IRoutingService routingService,
            [Frozen] IGpPracticeService mockGpPracticeService,
            [Frozen] IOrderService mockOrderService,
            QuantityController controller)
        {
            var orderItem = order.OrderItems.First();

            orderItem.OrderItemPrice.ProvisioningType = ProvisioningType.Patient;
            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.FlattenedRecipients.ForEach(r => r.OrderItemSublocationRecipients.ForEach(x => x.Quantity = null));

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockGpPracticeService.GetNumberOfPatients(Arg.Any<IEnumerable<string>>())
                .Returns(
                    order.FlattenedRecipients.Select(x =>
                            new GpPracticeSize { OdsCode = x.RecipientOdsCode, NumberOfPatients = NumberOfPatients })
                        .ToList());

            routingService.GetRoute(
                    RoutingPoint.SelectQuantityBackLink,
                    Arg.Any<OrderWrapper>(),
                    Arg.Any<RouteValues>())
                .Returns(routingResult);

            var result = await controller.SelectServiceSublocationRecipientQuantity(
                internalOrgId,
                callOffId,
                orderItem.CatalogueItemId,
                parentOdsCode);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = actualResult.Model.Should().BeOfType<SelectServiceRecipientQuantityModel>().Subject;

            IEnumerable<ServiceRecipientQuantityDto> recipients = order.FlattenedRecipients
                .Where(x => x.ParentSublocationOdsCode == parentOdsCode)
                .Select(x =>
                    new ServiceRecipientQuantityDto(
                        x.ParentSublocationOdsCode,
                        x.RecipientOdsCode,
                        x.RecipientOdsOrganisation?.Name,
                        x.GetQuantityForItem(orderItem.CatalogueItemId),
                        x.ParentSublocation.SublocationOrganisation.Name));

            var expected = new SelectServiceRecipientQuantityModel(
                order.OrderType,
                order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient,
                orderItem.CatalogueItem,
                orderItem.OrderItemPrice,
                recipients,
                null);
            expected.SubLocations.ForEach(x =>
                x.ServiceRecipients.ForEach(y => y.InputQuantity = $"{NumberOfPatients}"));

            model.Should()
                .BeEquivalentTo(
                    expected,
                    x => x
                        .Excluding(m => m.BackLink)
                        .Excluding(m => m.Title));
        }

        [Theory]
        [MockAutoData]
        public static async Task
            Get_SelectServiceSublocationRecipientQuantity_WithPrePopulatedSolutionRecipients_ExpectedResult(
                string internalOrgId,
                CallOffId callOffId,
                string parentOdsCode,
                EntityFramework.Ordering.Models.Order order,
                RoutingResult routingResult,
                [Frozen] IRoutingService routingService,
                [Frozen] IOrderService mockOrderService,
                QuantityController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            solution.OrderItemPrice.ProvisioningType = ProvisioningType.Patient;
            order.FlattenedRecipients.ForEach(r => r.SetQuantityForItem(solution, NumberOfPatients));

            var orderItem = order.OrderItems.ElementAt(1);

            orderItem.OrderItemPrice.ProvisioningType = ProvisioningType.Patient;
            order.FlattenedRecipients.ForEach(r =>
                r.OrderItemSublocationRecipients.Where(i => i.OrderItem.Id == orderItem.Id)
                    .ForEach(x => x.Quantity = null));

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            routingService.GetRoute(
                    RoutingPoint.SelectQuantityBackLink,
                    Arg.Any<OrderWrapper>(),
                    Arg.Any<RouteValues>())
                .Returns(routingResult);

            var result = await controller.SelectServiceSublocationRecipientQuantity(
                internalOrgId,
                callOffId,
                orderItem.CatalogueItemId,
                parentOdsCode);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = actualResult.Model.Should().BeOfType<SelectServiceRecipientQuantityModel>().Subject;

            IEnumerable<ServiceRecipientQuantityDto> recipients = order.FlattenedRecipients
                .Where(recipient => recipient.ParentSublocationOdsCode == parentOdsCode)
                .Select(recipient =>
                    new ServiceRecipientQuantityDto(
                        recipient.ParentSublocationOdsCode,
                        recipient.RecipientOdsCode,
                        recipient.RecipientOdsOrganisation?.Name,
                        recipient.GetQuantityForItem(orderItem.CatalogueItemId),
                        recipient.ParentSublocation.SublocationOrganisation.Name));

            var expected = new SelectServiceRecipientQuantityModel(
                order.OrderType,
                order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient,
                orderItem.CatalogueItem,
                orderItem.OrderItemPrice,
                recipients,
                null);

            expected.SubLocations.ForEach(x =>
                x.ServiceRecipients.ForEach(y => y.InputQuantity = $"{NumberOfPatients}"));

            model.Should()
                .BeEquivalentTo(
                    expected,
                    x => x
                        .Excluding(m => m.BackLink)
                        .Excluding(m => m.Title));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SelectServiceSublocationRecipientQuantity_ModelError_ReturnsModel(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            string parentOdsCode,
            SelectServiceRecipientQuantityModel model,
            QuantityController controller)
        {
            controller.ModelState.AddModelError("key", "message");

            var result = await controller.SelectServiceSublocationRecipientQuantity(
                internalOrgId,
                callOffId,
                catalogueItemId,
                parentOdsCode,
                model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SelectServiceSublocationRecipientQuantity_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            string parentOdsCode,
            EntityFramework.Ordering.Models.Order order,
            SelectServiceRecipientQuantityModel model,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IOrderQuantityService mockOrderQuantityService,
            QuantityController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var orderItem = order.OrderItems.First();
            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var orderWrapper = new OrderWrapper(order);
            mockOrderService.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId).Returns(orderWrapper);

            List<OrderItemRecipientQuantityDto> actual = null;

            mockOrderQuantityService
                .When(x => x.SetServiceRecipientQuantities(
                    order.Id,
                    orderItem.CatalogueItemId,
                    Arg.Any<List<OrderItemRecipientQuantityDto>>()))
                .Do(x => actual = x.Arg<List<OrderItemRecipientQuantityDto>>());

            model.SubLocations.ForEach(x => x.ServiceRecipients.ForEach(y => y.InputQuantity = y.Quantity.ToString()));

            var result = await controller.SelectServiceSublocationRecipientQuantity(
                internalOrgId,
                callOffId,
                orderItem.CatalogueItemId,
                parentOdsCode,
                model);

            foreach (OrderItemRecipientQuantityDto dto in actual)
            {
                model.SubLocations.First()
                    .ServiceRecipients
                    .First(x => x.RecipientOdsCode == dto.RecipientOdsCode
                        && dto.ParentSublocationOdsCode == parentOdsCode)
                    .Quantity.Should()
                    .Be(dto.Quantity);
            }

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(QuantityController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(QuantityController.SublocationHub));
            actualResult.RouteValues.Should()
                .Contain(new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", callOffId }, });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ViewServiceRecipientQuantity_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            EntityFramework.Ordering.Models.Order amendment,
            OrderSublocation sublocation,
            [Frozen] IOrderService orderService,
            QuantityController controller)
        {
            order.Revision = 1;
            amendment.OrderNumber = order.OrderNumber;
            amendment.Revision = 2;

            var sublocationRecipient = new OrderSublocationRecipient("r1", sublocation.SublocationOdsCode);
            sublocation.SublocationRecipients = [sublocationRecipient];
            order.OrderSublocations = [sublocation];

            var amendSublocation = new OrderSublocation();
            amendSublocation.SublocationOdsCode = sublocation.SublocationOdsCode;
            var amendSublocationRecipient = new OrderSublocationRecipient("r2", sublocation.SublocationOdsCode);
            amendSublocation.SublocationRecipients = [sublocationRecipient, amendSublocationRecipient];
            amendment.OrderSublocations = [amendSublocation];

            var orderItem = order.OrderItems.First();
            amendment.OrderItems = [orderItem];

            orderService.GetOrderWithOrderItems(amendment.CallOffId, internalOrgId).Returns(new OrderWrapper(amendment, [order]));

            var result = await controller.ViewServiceRecipientQuantity(
                internalOrgId,
                callOffId,
                orderItem.CatalogueItemId,
                amendment.CallOffId);

            var actual = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new ViewServiceRecipientQuantityModel(orderItem, [amendSublocationRecipient])
            {
                InternalOrgId = internalOrgId, CallOffId = callOffId,
            };

            actual.Should().NotBeNull();
            actual.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ConfirmQuantities_ReturnsViewWithModel(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            Solution solution,
            [Frozen] IOrderService orderService,
            QuantityController controller)
        {
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var wrapper = new OrderWrapper(order);

            var orderItem = order.OrderItems.First();
            orderItem.CatalogueItem = solution.CatalogueItem;

            orderService.GetOrderWithOrderItems(order.CallOffId, internalOrgId).Returns(wrapper);

            var orderRecipients = wrapper.DetermineOrderRecipients(orderItem.CatalogueItemId);
            var orderRecipientDtos = QuantityController.GetRecipientDtos(orderRecipients, orderItem);

            var expectedModel = new ConfirmQuantitiesModel(
                orderItem.CatalogueItem,
                orderItem.OrderItemPrice,
                orderRecipientDtos);

            var result = await controller.ConfirmQuantities(internalOrgId, order.CallOffId, solution.CatalogueItemId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = actualResult.Model.Should().BeOfType<ConfirmQuantitiesModel>().Subject;

            model.Should()
                .BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.ContinueLink));
        }
    }
}
