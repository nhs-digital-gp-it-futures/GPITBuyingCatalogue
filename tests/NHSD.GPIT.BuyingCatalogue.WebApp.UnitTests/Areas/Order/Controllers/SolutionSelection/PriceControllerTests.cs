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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Prices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.SolutionSelection
{
    public static class PriceControllerTests
    {
        private const int PriceId = 1;

        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(PricesController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(PricesController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Order");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(PricesController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectPrice_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<ISolutionListPriceService> mockListPriceService,
            PricesController controller)
        {
            var orderItem = order.OrderItems.First().CatalogueItem;

            orderItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(order);

            mockListPriceService
                .Setup(lps => lps.GetCatalogueItemWithPublishedListPrices(orderItem.Id))
                .ReturnsAsync(orderItem);

            var result = await controller.SelectPrice(internalOrgId, callOffId);

            mockOrderService.VerifyAll();
            mockListPriceService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectPriceModel(orderItem);

            actualResult.Model.Should().BeEquivalentTo(expected, m => m.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectPrice_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            PricesController controller)
        {
            var model = new SelectPriceModel
            {
                SelectedPriceId = PriceId,
            };

            var result = await controller.SelectPrice(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(PricesController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(PricesController.ConfirmPrice));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "priceId", PriceId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectPrice_ModelError_ReturnsModel(
            string internalOrgId,
            CallOffId callOffId,
            SelectPriceModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<ISolutionListPriceService> mockListPriceService,
            PricesController controller)
        {
            var orderItem = order.OrderItems.First().CatalogueItem;

            orderItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(order);

            mockListPriceService
                .Setup(lps => lps.GetCatalogueItemWithPublishedListPrices(orderItem.Id))
                .ReturnsAsync(orderItem);

            controller.ModelState.AddModelError("key", "message");

            var result = await controller.SelectPrice(internalOrgId, callOffId, model);

            mockOrderService.VerifyAll();
            mockListPriceService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectPriceModel(orderItem)
            {
                SelectedPriceId = model.SelectedPriceId,
                SolutionName = model.SolutionName,
                SolutionType = model.SolutionType,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, m => m.Excluding(o => o.BackLink).Excluding(o => o.BackLinkText));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConfirmPrice_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<ISolutionListPriceService> mockListPriceService,
            PricesController controller)
        {
            var orderItem = order.OrderItems.First().CatalogueItem;
            var priceId = orderItem.CataloguePrices.First().CataloguePriceId;

            orderItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(order);

            mockListPriceService
                .Setup(lps => lps.GetCatalogueItemWithPublishedListPrices(orderItem.Id))
                .ReturnsAsync(orderItem);

            var result = await controller.ConfirmPrice(internalOrgId, callOffId, priceId);

            mockOrderService.VerifyAll();
            mockListPriceService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new ConfirmPriceModel(orderItem, priceId);

            actualResult.Model.Should().BeEquivalentTo(expected, m => m.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConfirmPrice_ModelError_ReturnsModel(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmPriceModel model,
            PricesController controller)
        {
            controller.ModelState.AddModelError("key", "message");

            var result = await controller.ConfirmPrice(internalOrgId, callOffId, PriceId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConfirmPrice_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmPriceModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<ISolutionListPriceService> mockListPriceService,
            [Frozen] Mock<IOrderPriceService> mockOrderPriceService,
            PricesController controller)
        {
            var orderItem = order.OrderItems.First().CatalogueItem;
            var price = orderItem.CataloguePrices.First();

            orderItem.CatalogueItemType = CatalogueItemType.Solution;

            price.CataloguePriceTiers
                .Select((tier, i) => (tier, i))
                .ForEach(x =>
                {
                    model.Tiers[x.i].Id = x.tier.Id;
                    model.Tiers[x.i].AgreedPrice = $"{x.tier.Price:#,##0.00##}";
                });

            mockOrderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(order);

            mockListPriceService
                .Setup(lps => lps.GetCatalogueItemWithPublishedListPrices(orderItem.Id))
                .ReturnsAsync(orderItem);

            List<OrderPricingTierDto> actual = null;

            mockOrderPriceService
                .Setup(x => x.AddPrice(order.Id, price, It.IsAny<List<OrderPricingTierDto>>()))
                .Callback<int, CataloguePrice, List<OrderPricingTierDto>>((_, _, x) => actual = x)
                .Returns(Task.CompletedTask);

            var result = await controller.ConfirmPrice(internalOrgId, callOffId, price.CataloguePriceId, model);

            mockOrderService.VerifyAll();
            mockListPriceService.VerifyAll();
            mockOrderPriceService.VerifyAll();

            actual.ForEach(x =>
            {
                price.CataloguePriceTiers
                    .Single(t => t.LowerRange == x.LowerRange && t.UpperRange == x.UpperRange)
                    .Price.Should().Be(x.Price);
            });

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(OrderController.Order));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalServiceSelectPrice_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<ISolutionListPriceService> mockListPriceService,
            PricesController controller)
        {
            var orderItem = order.OrderItems.First().CatalogueItem;

            orderItem.CatalogueItemType = CatalogueItemType.AdditionalService;

            mockListPriceService
                .Setup(lps => lps.GetCatalogueItemWithPublishedListPrices(orderItem.Id))
                .ReturnsAsync(orderItem);

            var result = await controller.AdditionalServiceSelectPrice(internalOrgId, callOffId, orderItem.Id);

            mockListPriceService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectPriceModel(orderItem);

            actualResult.Model.Should().BeEquivalentTo(expected, m => m.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalServiceSelectPrice_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            PricesController controller)
        {
            var model = new SelectPriceModel
            {
                SelectedPriceId = PriceId,
            };

            var result = await controller.AdditionalServiceSelectPrice(internalOrgId, callOffId, catalogueItemId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(PricesController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(PricesController.AdditionalServiceConfirmPrice));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "catalogueItemId", catalogueItemId },
                { "priceId", PriceId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalServiceSelectPrice_ModelError_ReturnsModel(
        string internalOrgId,
        CallOffId callOffId,
        SelectPriceModel model,
        EntityFramework.Ordering.Models.Order order,
        [Frozen] Mock<ISolutionListPriceService> mockListPriceService,
        PricesController controller)
        {
            var orderItem = order.OrderItems.First().CatalogueItem;

            orderItem.CatalogueItemType = CatalogueItemType.Solution;

            mockListPriceService
                .Setup(lps => lps.GetCatalogueItemWithPublishedListPrices(orderItem.Id))
                .ReturnsAsync(orderItem);

            controller.ModelState.AddModelError("key", "message");

            var result = await controller.AdditionalServiceSelectPrice(internalOrgId, callOffId, orderItem.Id, model);

            mockListPriceService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectPriceModel(orderItem)
            {
                SelectedPriceId = model.SelectedPriceId,
                SolutionName = model.SolutionName,
                SolutionType = model.SolutionType,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, m => m.Excluding(o => o.BackLink).Excluding(o => o.BackLinkText));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalServiceConfirmPrice_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<ISolutionListPriceService> mockListPriceService,
            PricesController controller)
        {
            var orderItem = order.OrderItems.First().CatalogueItem;
            var priceId = orderItem.CataloguePrices.First().CataloguePriceId;

            orderItem.CatalogueItemType = CatalogueItemType.AdditionalService;

            mockListPriceService
                .Setup(lps => lps.GetCatalogueItemWithPublishedListPrices(orderItem.Id))
                .ReturnsAsync(orderItem);

            var result = await controller.AdditionalServiceConfirmPrice(internalOrgId, callOffId, orderItem.Id, priceId);

            mockListPriceService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new ConfirmPriceModel(orderItem, priceId);

            actualResult.Model.Should().BeEquivalentTo(expected, m => m.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalServiceConfirmPrice_ModelError_ReturnsModel(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            ConfirmPriceModel model,
            PricesController controller)
        {
            controller.ModelState.AddModelError("key", "message");

            var result = await controller.AdditionalServiceConfirmPrice(internalOrgId, callOffId, catalogueItemId, PriceId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalServiceConfirmPrice_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmPriceModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<ISolutionListPriceService> mockListPriceService,
            [Frozen] Mock<IOrderPriceService> mockOrderPriceService,
            PricesController controller)
        {
            var orderItem = order.OrderItems.First().CatalogueItem;
            var price = orderItem.CataloguePrices.First();

            orderItem.CatalogueItemType = CatalogueItemType.AdditionalService;

            price.CataloguePriceTiers
                .Select((tier, i) => (tier, i))
                .ForEach(x =>
                {
                    model.Tiers[x.i].Id = x.tier.Id;
                    model.Tiers[x.i].AgreedPrice = $"{x.tier.Price:#,##0.00##}";
                });

            mockOrderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(order);

            mockListPriceService
                .Setup(lps => lps.GetCatalogueItemWithPublishedListPrices(orderItem.Id))
                .ReturnsAsync(orderItem);

            List<OrderPricingTierDto> actual = null;

            mockOrderPriceService
                .Setup(x => x.AddPrice(order.Id, price, It.IsAny<List<OrderPricingTierDto>>()))
                .Callback<int, CataloguePrice, List<OrderPricingTierDto>>((_, _, x) => actual = x)
                .Returns(Task.CompletedTask);

            var result = await controller.AdditionalServiceConfirmPrice(internalOrgId, callOffId, orderItem.Id, price.CataloguePriceId, model);

            mockOrderService.VerifyAll();
            mockListPriceService.VerifyAll();
            mockOrderPriceService.VerifyAll();

            actual.ForEach(x =>
            {
                price.CataloguePriceTiers
                    .Single(t => t.LowerRange == x.LowerRange && t.UpperRange == x.UpperRange)
                    .Price.Should().Be(x.Price);
            });

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(OrderController.Order));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AssociatedServiceSelectPrice_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<ISolutionListPriceService> mockListPriceService,
            PricesController controller)
        {
            var orderItem = order.OrderItems.First().CatalogueItem;

            orderItem.CatalogueItemType = CatalogueItemType.AssociatedService;

            mockListPriceService
                .Setup(lps => lps.GetCatalogueItemWithPublishedListPrices(orderItem.Id))
                .ReturnsAsync(orderItem);

            var result = await controller.AssociatedServiceSelectPrice(internalOrgId, callOffId, orderItem.Id);

            mockListPriceService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var expected = new SelectPriceModel(orderItem);

            actualResult.Model.Should().BeEquivalentTo(expected, m => m.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AssociatedServiceSelectPrice_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            PricesController controller)
        {
            var model = new SelectPriceModel
            {
                SelectedPriceId = PriceId,
            };

            var result = await controller.AssociatedServiceSelectPrice(internalOrgId, callOffId, catalogueItemId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(PricesController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(PricesController.AssociatedServiceConfirmPrice));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "catalogueItemId", catalogueItemId },
                { "priceId", PriceId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AssociatedServiceSelectPrice_ModelError_ReturnsModel(
            string internalOrgId,
            CallOffId callOffId,
            SelectPriceModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<ISolutionListPriceService> mockListPriceService,
            PricesController controller)
        {
            var orderItem = order.OrderItems.First().CatalogueItem;

            orderItem.CatalogueItemType = CatalogueItemType.Solution;

            mockListPriceService
                .Setup(lps => lps.GetCatalogueItemWithPublishedListPrices(orderItem.Id))
                .ReturnsAsync(orderItem);

            controller.ModelState.AddModelError("key", "message");

            var result = await controller.AssociatedServiceSelectPrice(internalOrgId, callOffId, orderItem.Id, model);

            mockListPriceService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectPriceModel(orderItem)
            {
                SelectedPriceId = model.SelectedPriceId,
                SolutionName = model.SolutionName,
                SolutionType = model.SolutionType,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, m => m.Excluding(o => o.BackLink).Excluding(o => o.BackLinkText));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AssociatedServiceConfirmPrice_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<ISolutionListPriceService> mockListPriceService,
            PricesController controller)
        {
            var orderItem = order.OrderItems.First().CatalogueItem;
            var priceId = orderItem.CataloguePrices.First().CataloguePriceId;

            orderItem.CatalogueItemType = CatalogueItemType.AssociatedService;

            mockListPriceService
                .Setup(lps => lps.GetCatalogueItemWithPublishedListPrices(orderItem.Id))
                .ReturnsAsync(orderItem);

            var result = await controller.AssociatedServiceConfirmPrice(internalOrgId, callOffId, orderItem.Id, priceId);

            mockListPriceService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new ConfirmPriceModel(orderItem, priceId);

            actualResult.Model.Should().BeEquivalentTo(expected, m => m.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AssociatedServiceConfirmPrice_ModelError_ReturnsModel(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            ConfirmPriceModel model,
            PricesController controller)
        {
            controller.ModelState.AddModelError("key", "message");

            var result = await controller.AssociatedServiceConfirmPrice(internalOrgId, callOffId, catalogueItemId, PriceId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AssociatedServiceConfirmPrice_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmPriceModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<ISolutionListPriceService> mockListPriceService,
            [Frozen] Mock<IOrderPriceService> mockOrderPriceService,
            PricesController controller)
        {
            var orderItem = order.OrderItems.First().CatalogueItem;
            var price = orderItem.CataloguePrices.First();

            orderItem.CatalogueItemType = CatalogueItemType.AssociatedService;

            price.CataloguePriceTiers
                .Select((tier, i) => (tier, i))
                .ForEach(x =>
                {
                    model.Tiers[x.i].Id = x.tier.Id;
                    model.Tiers[x.i].AgreedPrice = $"{x.tier.Price:#,##0.00##}";
                });

            mockOrderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(order);

            mockListPriceService
                .Setup(lps => lps.GetCatalogueItemWithPublishedListPrices(orderItem.Id))
                .ReturnsAsync(orderItem);

            List<OrderPricingTierDto> actual = null;

            mockOrderPriceService
                .Setup(x => x.AddPrice(order.Id, price, It.IsAny<List<OrderPricingTierDto>>()))
                .Callback<int, CataloguePrice, List<OrderPricingTierDto>>((_, _, x) => actual = x)
                .Returns(Task.CompletedTask);

            var result = await controller.AssociatedServiceConfirmPrice(internalOrgId, callOffId, orderItem.Id, price.CataloguePriceId, model);

            mockOrderService.VerifyAll();
            mockListPriceService.VerifyAll();
            mockOrderPriceService.VerifyAll();

            actual.ForEach(x =>
            {
                price.CataloguePriceTiers
                    .Single(t => t.LowerRange == x.LowerRange && t.UpperRange == x.UpperRange)
                    .Price.Should().Be(x.Price);
            });

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(OrderController.Order));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }
    }
}
