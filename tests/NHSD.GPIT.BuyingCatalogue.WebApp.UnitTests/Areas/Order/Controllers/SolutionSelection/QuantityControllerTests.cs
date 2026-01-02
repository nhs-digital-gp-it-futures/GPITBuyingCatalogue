using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
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
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(QuantityController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SelectQuantity_BadRequest(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            QuantityController controller)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.SelectQuantity(internalOrgId, callOffId, catalogueItemId);

            result.Should().BeOfType<BadRequestResult>();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SelectQuantity_ViewResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            RoutingResult routingResult,
            [Frozen] IRoutingService routingService,
            [Frozen] IOrderService mockOrderService,
            QuantityController controller)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);

            var orderItem = order.OrderItems.First();

            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            orderItem.OrderItemPrice.ProvisioningType = ProvisioningType.Declarative;
            orderItem.OrderItemPrice.CataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerSolutionOrService;

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));
            routingService.GetRoute(
                    RoutingPoint.SelectQuantityBackLink,
                    Arg.Any<OrderWrapper>(),
                    Arg.Any<RouteValues>())
                .Returns(routingResult);

            var result = await controller.SelectQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = actualResult.Model.Should().BeOfType<SelectOrderItemQuantityModel>().Subject;
            var expected = new SelectOrderItemQuantityModel(orderItem.CatalogueItem, orderItem.OrderItemPrice, orderItem.Quantity);

            model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockInlineAutoData(ProvisioningType.Patient, null)]
        [MockInlineAutoData(ProvisioningType.OnDemand, CataloguePriceQuantityCalculationType.PerServiceRecipient)]
        [MockInlineAutoData(ProvisioningType.Declarative, CataloguePriceQuantityCalculationType.PerServiceRecipient)]
        public static async Task Get_SelectQuantity_ProvisioningType_CataloguePriceQuantityCalculationType_PerServiceRecipient_Combination_Redirects(
            ProvisioningType provisioningType,
            CataloguePriceQuantityCalculationType? cataloguePriceQuantityCalculationType,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            QuantityController controller)
        {
            var orderItem = order.OrderItems.First();

            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            orderItem.OrderItemPrice.ProvisioningType = provisioningType;
            orderItem.OrderItemPrice.CataloguePriceQuantityCalculationType = cataloguePriceQuantityCalculationType;

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.SelectQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId);

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
        [MockAutoData]
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
        [MockAutoData]
        public static async Task Post_SelectQuantity_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            SelectOrderItemQuantityModel model,
            int quantity,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IOrderQuantityService mockOrderQuantityService,
            [Frozen] IRoutingService mockRoutingService,
            QuantityController controller)
        {
            var orderItem = order.OrderItems.First();

            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var orderWrapper = new OrderWrapper(order);
            mockOrderService.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId).Returns(orderWrapper);

            mockOrderQuantityService.SetOrderItemQuantity(order.Id, orderItem.CatalogueItemId, quantity).Returns(Task.CompletedTask);

            mockRoutingService.GetRoute(RoutingPoint.SelectQuantity, orderWrapper, Arg.Any<RouteValues>()).Returns(Route(internalOrgId, callOffId));

            model.Quantity = $"{quantity}";

            var result = await controller.SelectQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId, model);

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
        [MockAutoData]
        public static async Task Get_SelectServiceRecipientQuantity(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            RoutingResult routingResult,
            [Frozen] IRoutingService routingService,
            [Frozen] IOrderService mockOrderService,
            QuantityController controller)
        {
            var orderItem = order.OrderItems.ElementAt(1);

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            routingService.GetRoute(
                    RoutingPoint.SelectQuantityBackLink,
                    Arg.Any<OrderWrapper>(),
                    Arg.Any<RouteValues>())
                .Returns(routingResult);

            var result = await controller.SelectServiceRecipientQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = actualResult.Model.Should().BeOfType<SelectServiceRecipientQuantityModel>().Subject;

            IEnumerable<ServiceRecipientQuantityDto> recipients = order.FlattenedRecipients
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
                null)
            {
                Caption = $"Order {callOffId}",
                OrderingPartyName = order.OrderingParty.Name,
                RoutingFields = new RoutingFields()
                {
                    CatalogueItem = orderItem.CatalogueItemId,
                    InternalOrgId = internalOrgId,
                    CallOffId = callOffId,
                },
            };

            model.Should().BeEquivalentTo(expected, x => x
                .Excluding(m => m.BackLink));
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

            var result = await controller.SelectServiceSublocationRecipientQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId, parentOdsCode);

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

            model.Should().BeEquivalentTo(expected, x => x
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

            var result = await controller.SelectServiceSublocationRecipientQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId, parentOdsCode);

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
            expected.SubLocations.ForEach(x => x.ServiceRecipients.ForEach(y => y.InputQuantity = $"{NumberOfPatients}"));

            model.Should().BeEquivalentTo(expected, x => x
                .Excluding(m => m.BackLink)
                .Excluding(m => m.Title));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SelectServiceSublocationRecipientQuantity_WithPrePopulatedSolutionRecipients_ExpectedResult(
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
            order.FlattenedRecipients.ForEach(r => r.SetQuantityForItem(solution.CatalogueItemId, NumberOfPatients));

            var orderItem = order.OrderItems.ElementAt(1);

            orderItem.OrderItemPrice.ProvisioningType = ProvisioningType.Patient;
            order.FlattenedRecipients.ForEach(r =>
                r.OrderItemSublocationRecipients.Where(i => i.CatalogueItemId == orderItem.CatalogueItemId)
                    .ForEach(x => x.Quantity = null));

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            routingService.GetRoute(
                    RoutingPoint.SelectQuantityBackLink,
                    Arg.Any<OrderWrapper>(),
                    Arg.Any<RouteValues>())
                .Returns(routingResult);

            var result = await controller.SelectServiceSublocationRecipientQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId, parentOdsCode);

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

            expected.SubLocations.ForEach(x => x.ServiceRecipients.ForEach(y => y.InputQuantity = $"{NumberOfPatients}"));

            model.Should().BeEquivalentTo(expected, x => x
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

            var result = await controller.SelectServiceSublocationRecipientQuantity(internalOrgId, callOffId, catalogueItemId, parentOdsCode, model);

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
            [Frozen] IRoutingService mockRoutingService,
            QuantityController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var orderItem = order.OrderItems.First();

            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var orderWrapper = new OrderWrapper(order);
            mockOrderService.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId).Returns(orderWrapper);

            List<OrderItemRecipientQuantityDto> actual = null;

            mockOrderQuantityService
                .When(x => x.SetServiceRecipientQuantities(order.Id, orderItem.CatalogueItemId, Arg.Any<List<OrderItemRecipientQuantityDto>>()))
                .Do(x => actual = x.Arg<List<OrderItemRecipientQuantityDto>>());

            mockRoutingService.GetRoute(RoutingPoint.SelectQuantity, orderWrapper, Arg.Any<RouteValues>()).Returns(Route(internalOrgId, callOffId));

            model.SubLocations.ForEach(x => x.ServiceRecipients.ForEach(y => y.InputQuantity = y.Quantity > 0 ? string.Empty : "1"));

            var result = await controller.SelectServiceSublocationRecipientQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId, parentOdsCode, model);

            foreach (OrderItemRecipientQuantityDto dto in actual)
            {
                model.SubLocations.First().ServiceRecipients
                    .First(x => x.RecipientOdsCode == dto.RecipientOdsCode
                        && dto.ParentSublocationOdsCode == parentOdsCode)
                    .Quantity.Should()
                    .Be(dto.Quantity == 0 ? 1 : dto.Quantity);
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
        [MockAutoData]
        public static async Task Post_SelectServiceSublocationRecipientQuantity_RedirectToSelectServices(
            string internalOrgId,
            CallOffId callOffId,
            string parentOdsCode,
            EntityFramework.Ordering.Models.Order order,
            SelectServiceRecipientQuantityModel model,
            [Frozen] IOrderService mockOrderService,
            QuantityController controller)
        {
            var orderItem = order.OrderItems.First();

            var orderWrapper = new OrderWrapper(order);
            mockOrderService.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId).Returns(orderWrapper);

            model.SubLocations.First()
                .ServiceRecipients.ForEach(recipient =>
                    recipient.InputQuantity = recipient.Quantity > 0 ? string.Empty : "1");
            orderWrapper.Order.FlattenedRecipients.First().OrderItemSublocationRecipients.First().Quantity = 0;

            var result = await controller.SelectServiceSublocationRecipientQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId, parentOdsCode, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(QuantityController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(QuantityController.SelectServiceRecipientQuantity));
        }

        [Theory]
        [MockInlineAutoData(ProvisioningType.Patient)]
        public static async Task Get_ViewOrderItemQuantity_PerServiceRecipientPrice_ExpectedResult(
            ProvisioningType provisioningType,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            EntityFramework.Ordering.Models.Order amendment,
            [Frozen] IOrderService orderService,
            QuantityController controller)
        {
            order.Revision = 1;
            amendment.OrderNumber = order.OrderNumber;
            amendment.Revision = 2;

            var orderItem = order.OrderItems.First();

            orderItem.OrderItemPrice.ProvisioningType = provisioningType;
            orderItem.OrderItemPrice.CataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerServiceRecipient;

            orderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(amendment, [order]));

            var result = await controller.ViewOrderItemQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId);

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
        [MockInlineAutoData(ProvisioningType.Declarative)]
        [MockInlineAutoData(ProvisioningType.OnDemand)]
        public static async Task Get_ViewOrderItemQuantity_ExpectedResult(
            ProvisioningType provisioningType,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            EntityFramework.Ordering.Models.Order amendment,
            [Frozen] IOrderService orderService,
            QuantityController controller)
        {
            order.Revision = 1;
            amendment.OrderNumber = order.OrderNumber;
            amendment.Revision = 2;

            var orderItem = order.OrderItems.First();

            orderItem.OrderItemPrice.ProvisioningType = provisioningType;
            orderItem.OrderItemPrice.CataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerSolutionOrService;

            orderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(amendment, [order]));

            var result = await controller.ViewOrderItemQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId);

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
        [MockAutoData]
        public static async Task Get_ViewServiceRecipientQuantity_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            EntityFramework.Ordering.Models.Order amendment,
            [Frozen] IOrderService orderService,
            QuantityController controller)
        {
            order.Revision = 1;
            amendment.OrderNumber = order.OrderNumber;
            amendment.Revision = 2;

            var orderItem = order.OrderItems.First();

            orderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(amendment, [order]));

            var result = await controller.ViewServiceRecipientQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId);

            var actual = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new ViewServiceRecipientQuantityModel(orderItem, order.FlattenedRecipients)
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
