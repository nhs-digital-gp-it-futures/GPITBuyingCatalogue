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
using Microsoft.AspNetCore.Routing;
using Moq;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Quantity;
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
            typeof(QuantityController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Order");
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
        public static async Task Get_SelectQuantity_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            QuantityController controller)
        {
            var orderItem = order.OrderItems.First();

            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            orderItem.OrderItemPrice.ProvisioningType = ProvisioningType.Declarative;
            orderItem.OrderItemPrice.CataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerSolutionOrService;

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(order);

            var result = await controller.SelectQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId);

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = actualResult.Model.Should().BeOfType<SelectOrderItemQuantityModel>().Subject;
            var expected = new SelectOrderItemQuantityModel(orderItem);

            model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonInlineAutoData(ProvisioningType.Patient)]
        [CommonInlineAutoData(ProvisioningType.PerServiceRecipient)]
        public static async Task Get_SelectQuantity_ProvisioningType_ExpectedResult(
            ProvisioningType provisioningType,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            QuantityController controller)
        {
            var orderItem = order.OrderItems.First();

            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            orderItem.OrderItemPrice.ProvisioningType = provisioningType;
            orderItem.OrderItemPrice.CataloguePriceQuantityCalculationType = null;

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(order);

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
        public static async Task Get_SelectQuantity_QuantityCalculationPerServiceRecipient_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            QuantityController controller)
        {
            var orderItem = order.OrderItems.First();

            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            orderItem.OrderItemPrice.ProvisioningType = ProvisioningType.Declarative;
            orderItem.OrderItemPrice.CataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerServiceRecipient;

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(order);

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

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(order);

            mockOrderQuantityService
                .Setup(x => x.SetOrderItemQuantity(order.Id, orderItem.CatalogueItemId, quantity))
                .Returns(Task.CompletedTask);

            mockRoutingService
                .Setup(x => x.GetRoute(RoutingPoint.SelectQuantity, order, It.IsAny<RouteValues>()))
                .Returns(Route(internalOrgId, callOffId));

            model.Quantity = $"{quantity}";

            var result = await controller.SelectQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId, model);

            mockOrderService.VerifyAll();
            mockOrderQuantityService.VerifyAll();
            mockRoutingService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(AssociatedServicesController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(AssociatedServicesController.AddAssociatedServices));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectServiceRecipientQuantity_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IGpPracticeCacheService> mockCacheService,
            [Frozen] Mock<IOrderService> mockOrderService,
            QuantityController controller)
        {
            var orderItem = order.OrderItems.First();

            orderItem.OrderItemPrice.ProvisioningType = ProvisioningType.Patient;
            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            orderItem.OrderItemRecipients.ForEach(x => x.Quantity = null);

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(order);

            mockCacheService
                .Setup(x => x.GetNumberOfPatients(It.IsAny<string>()))
                .ReturnsAsync(NumberOfPatients);

            var result = await controller.SelectServiceRecipientQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId);

            mockOrderService.VerifyAll();
            mockCacheService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = actualResult.Model.Should().BeOfType<SelectServiceRecipientQuantityModel>().Subject;

            var expected = new SelectServiceRecipientQuantityModel(orderItem);
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
            QuantityController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            solution.OrderItemPrice.ProvisioningType = ProvisioningType.Patient;
            solution.OrderItemRecipients.ForEach(x => x.Quantity = NumberOfPatients);

            var orderItem = order.OrderItems.ElementAt(1);

            orderItem.OrderItemPrice.ProvisioningType = ProvisioningType.Patient;
            orderItem.OrderItemRecipients.ForEach(x => x.Quantity = null);

            for (var i = 0; i < solution.OrderItemRecipients.Count; i++)
            {
                orderItem.OrderItemRecipients.ElementAt(i).OdsCode = solution.OrderItemRecipients.ElementAt(i).OdsCode;
            }

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(order);

            var result = await controller.SelectServiceRecipientQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId);

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = actualResult.Model.Should().BeOfType<SelectServiceRecipientQuantityModel>().Subject;
            var expected = new SelectServiceRecipientQuantityModel(orderItem);

            expected.ServiceRecipients.ForEach(x => x.InputQuantity = $"{NumberOfPatients}");

            model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectServiceRecipientQuantity_PerServiceRecipient_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            QuantityController controller)
        {
            const int expectedQuantity = 1;
            var orderItem = order.OrderItems.First();

            orderItem.OrderItemPrice.ProvisioningType = ProvisioningType.PerServiceRecipient;
            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(order);

            var result = await controller.SelectServiceRecipientQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId);

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = actualResult.Model.Should().BeOfType<SelectServiceRecipientQuantityModel>().Subject;
            var expected = new SelectServiceRecipientQuantityModel(orderItem);

            expected.ServiceRecipients.ForEach(x => x.Quantity = expectedQuantity);

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

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(order);

            List<OrderItemRecipientQuantityDto> actual = null;

            mockOrderQuantityService
                .Setup(x => x.SetServiceRecipientQuantities(order.Id, orderItem.CatalogueItemId, It.IsAny<List<OrderItemRecipientQuantityDto>>()))
                .Callback<int, CatalogueItemId, List<OrderItemRecipientQuantityDto>>((_, _, x) => actual = x)
                .Returns(Task.CompletedTask);

            mockRoutingService
                .Setup(x => x.GetRoute(RoutingPoint.SelectQuantity, order, It.IsAny<RouteValues>()))
                .Returns(Route(internalOrgId, callOffId));

            model.ServiceRecipients.ForEach(x => x.InputQuantity = x.Quantity > 0 ? string.Empty : "1");

            var result = await controller.SelectServiceRecipientQuantity(internalOrgId, callOffId, orderItem.CatalogueItemId, model);

            mockOrderService.VerifyAll();
            mockOrderQuantityService.VerifyAll();

            foreach (var dto in actual)
            {
                model.ServiceRecipients
                    .Single(x => x.OdsCode == dto.OdsCode)
                    .Quantity.Should().Be(dto.Quantity == 0 ? 1 : dto.Quantity);
            }

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(AssociatedServicesController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(AssociatedServicesController.AddAssociatedServices));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        private static RoutingResult Route(string internalOrgId, CallOffId callOffId) => new()
        {
            ActionName = Constants.Actions.AddAssociatedServices,
            ControllerName = Constants.Controllers.AssociatedServices,
            RouteValues = new { internalOrgId, callOffId },
        };
    }
}
