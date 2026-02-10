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
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Pricing;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.SolutionSelection
{
    public static class PricesControllerTests
    {
        private const int PriceId = 1;

        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(PricesController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(PricesController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(PricesController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SelectPrice_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            RoutingResult routingResult,
            [Frozen] IRoutingService routingService,
            [Frozen] IListPriceService mockListPriceService,
            PricesController controller)
        {
            var orderItem = order.OrderItems.First().CatalogueItem;

            orderItem.CatalogueItemType = CatalogueItemType.Solution;

            mockListPriceService.GetCatalogueItemWithPublishedListPrices(orderItem.Id).Returns(orderItem);
            routingService.GetRoute(RoutingPoint.SelectPriceBackLink, Arg.Any<OrderWrapper>(), Arg.Any<RouteValues>()).Returns(routingResult);

            var result = await controller.SelectPrice(internalOrgId, callOffId, orderItem.Id);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectPriceModel(orderItem);

            actualResult.Model.Should().BeEquivalentTo(expected, m => m.Excluding(o => o.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SelectPrice_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            PricesController controller)
        {
            var model = new SelectPriceModel
            {
                SelectedPriceId = PriceId,
            };

            var result = await controller.SelectPrice(internalOrgId, callOffId, catalogueItemId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(PricesController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(PricesController.ConfirmPrice));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "catalogueItemId", catalogueItemId },
                { "priceId", PriceId },
                { "source", model.Source },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SelectPrice_ModelError_ReturnsModel(
            string internalOrgId,
            CallOffId callOffId,
            SelectPriceModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IListPriceService mockListPriceService,
            PricesController controller)
        {
            var orderItem = order.OrderItems.First().CatalogueItem;

            orderItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockListPriceService.GetCatalogueItemWithPublishedListPrices(orderItem.Id).Returns(orderItem);

            controller.ModelState.AddModelError("key", "message");

            var result = await controller.SelectPrice(internalOrgId, callOffId, orderItem.Id, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectPriceModel(orderItem)
            {
                Title = model.Title,
                Caption = model.Caption,
                Advice = model.Advice,
                AdditionalAdvice = model.AdditionalAdvice,
                SelectedPriceId = model.SelectedPriceId,
                SolutionName = model.SolutionName,
                SolutionType = model.SolutionType,
                Source = model.Source,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, m => m.Excluding(o => o.BackLink).Excluding(o => o.BackLinkText));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ConfirmPrice_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            RoutingResult routingResult,
            [Frozen] IRoutingService routingService,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IListPriceService mockListPriceService,
            PricesController controller)
        {
            var orderItem = order.OrderItems.First().CatalogueItem;
            var price = orderItem.CataloguePrices.First();

            orderItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));
            mockOrderService.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId).Returns(new OrderWrapper(order));
            mockListPriceService.GetCatalogueItemWithPublishedListPrices(orderItem.Id).Returns(orderItem);
            routingService.GetRoute(RoutingPoint.ConfirmPriceBackLink, Arg.Any<OrderWrapper>(), Arg.Any<RouteValues>()).Returns(routingResult);

            var result = await controller.ConfirmPrice(internalOrgId, callOffId, orderItem.Id, price.CataloguePriceId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new ConfirmPriceModel(orderItem, price, null);

            actualResult.Model.Should().BeEquivalentTo(expected, m => m.Excluding(o => o.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_ConfirmPrice_ModelError_ReturnsModel(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            ConfirmPriceModel model,
            PricesController controller)
        {
            controller.ModelState.AddModelError("key", "message");

            var result = await controller.ConfirmPrice(internalOrgId, callOffId, catalogueItemId, PriceId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_ConfirmPrice_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IListPriceService mockListPriceService,
            [Frozen] IOrderPriceService mockOrderPriceService,
            [Frozen] IRoutingService mockRoutingService,
            PricesController controller)
        {
            var catalogueItem = order.OrderItems.First().CatalogueItem;
            var price = catalogueItem.CataloguePrices.First();

            catalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var orderWrapper = new OrderWrapper(order);
            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(orderWrapper);

            mockListPriceService.GetCatalogueItemWithPublishedListPrices(catalogueItem.Id).Returns(catalogueItem);

            mockRoutingService.GetRoute(RoutingPoint.ConfirmPrice, orderWrapper, Arg.Any<RouteValues>())
                .Returns(
                    new RoutingResult
                    {
                        ActionName = nameof(TaskListController.TaskList),
                        ControllerName = typeof(TaskListController).ControllerName(),
                        RouteValues = new { internalOrgId, callOffId },
                    });

            List<PricingTierDto> actual = null;

            mockOrderPriceService
                .When(x => x.UpsertPrice(order.Id, price, Arg.Any<List<PricingTierDto>>()))
                .Do(x => actual = x.Arg<List<PricingTierDto>>());

            var model = new ConfirmPriceModel(catalogueItem, price, null);

            var result = await controller.ConfirmPrice(internalOrgId, callOffId, catalogueItem.Id, price.CataloguePriceId, model);

            actual.ForEach(x =>
            {
                price.CataloguePriceTiers
                    .First(t => t.LowerRange == x.LowerRange && t.UpperRange == x.UpperRange)
                    .Price.Should().Be(x.Price);
            });

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(TaskListController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(TaskListController.TaskList));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditPrice_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            RoutingResult routingResult,
            List<CataloguePrice> prices,
            [Frozen] IRoutingService routingService,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IListPriceService listPriceService,
            PricesController controller)
        {
            var orderItem = order.OrderItems.First();
            var price = prices.First();

            orderItem.OrderItemPrice.CataloguePriceId = price.CataloguePriceId;
            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            orderItem.CatalogueItem.CataloguePrices = prices;

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));
            listPriceService.GetCatalogueItemWithPublishedListPrices(orderItem.CatalogueItemId).Returns(orderItem.CatalogueItem);
            routingService.GetRoute(RoutingPoint.EditPriceBackLink, Arg.Any<OrderWrapper>(), Arg.Any<RouteValues>()).Returns(routingResult);

            var result = await controller.EditPrice(internalOrgId, callOffId, orderItem.CatalogueItemId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var expected = new ConfirmPriceModel(orderItem.CatalogueItem, price, orderItem.OrderItemPrice);

            actualResult.Model.Should().BeEquivalentTo(expected, m => m.Excluding(o => o.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditPrice_MultiplePricesAvailable_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            CatalogueItem catalogueItem,
            List<CataloguePrice> availablePrices,
            RoutingResult routingResult,
            [Frozen] IRoutingService routingService,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IListPriceService mockListPriceService,
            PricesController controller)
        {
            var orderItem = order.OrderItems.First();
            var price = availablePrices.First();

            orderItem.CatalogueItem = catalogueItem;
            orderItem.CatalogueItemId = catalogueItem.Id;
            orderItem.OrderItemPrice.CataloguePriceId = price.CataloguePriceId;

            catalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            catalogueItem.CataloguePrices = availablePrices;

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));
            mockListPriceService.GetCatalogueItemWithPublishedListPrices(orderItem.CatalogueItemId).Returns(catalogueItem);
            routingService.GetRoute(RoutingPoint.EditPriceBackLink, Arg.Any<OrderWrapper>(), Arg.Any<RouteValues>()).Returns(routingResult);

            var result = await controller.EditPrice(internalOrgId, callOffId, orderItem.CatalogueItemId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var expected = new ConfirmPriceModel(orderItem.CatalogueItem, price, orderItem.OrderItemPrice);

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditPrice_MultiplePricesAvailable_FromTaskList_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            CatalogueItem catalogueItem,
            List<CataloguePrice> availablePrices,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IListPriceService mockListPriceService,
            PricesController controller)
        {
            var orderItem = order.OrderItems.First();

            catalogueItem.CataloguePrices = availablePrices;

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockListPriceService.GetCatalogueItemWithPublishedListPrices(orderItem.CatalogueItemId).Returns(catalogueItem);

            var result = await controller.EditPrice(internalOrgId, callOffId, orderItem.CatalogueItemId, RoutingSource.TaskList);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(PricesController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(PricesController.SelectPrice));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "catalogueItemId", orderItem.CatalogueItemId },
                { "selectedPriceId", orderItem.OrderItemPrice.CataloguePriceId },
                { "source", RoutingSource.TaskList },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditPrice_ModelError_ReturnsModel(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            ConfirmPriceModel model,
            PricesController controller)
        {
            controller.ModelState.AddModelError("key", "message");

            var result = await controller.EditPrice(internalOrgId, callOffId, catalogueItemId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditPrice_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IOrderPriceService mockOrderPriceService,
            [Frozen] IRoutingService mockRoutingService,
            PricesController controller)
        {
            var orderItem = order.OrderItems.First();

            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var orderWrapper = new OrderWrapper(order);
            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(orderWrapper);

            List<PricingTierDto> actual = null;

            mockOrderPriceService
                .When(x => x.UpdatePrice(order.Id, orderItem.CatalogueItemId, Arg.Any<List<PricingTierDto>>()))
                .Do(x => actual = x.Arg<List<PricingTierDto>>());

            var model = new ConfirmPriceModel(orderItem.OrderItemPrice, orderItem.CatalogueItem);

            model.Tiers.ForEach(x =>
            {
                var newPrice = x.ListPrice - 0.0001M;
                x.AgreedPrice = $"{newPrice:#,##0.00##}";
            });

            mockRoutingService
                .GetRoute(RoutingPoint.EditPrice, Arg.Any<OrderWrapper>(), Arg.Any<RouteValues>())
                .Returns(new RoutingResult
                {
                    ActionName = nameof(TaskListController.TaskList),
                    ControllerName = typeof(TaskListController).ControllerName(),
                    RouteValues = new { internalOrgId, callOffId },
                });

            var result = await controller.EditPrice(internalOrgId, callOffId, orderItem.CatalogueItemId, model);

            actual.ForEach(x =>
            {
                model.Tiers
                    .First(t => t.LowerRange == x.LowerRange && t.UpperRange == x.UpperRange)
                    .AgreedPrice.Should().Be($"{x.Price:#,##0.00##}");
            });

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(TaskListController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(TaskListController.TaskList));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ViewPrice_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order original,
            EntityFramework.Ordering.Models.Order amendment,
            [Frozen] IOrderService mockOrderService,
            PricesController controller)
        {
            original.Revision = 1;
            amendment.OrderNumber = original.OrderNumber;
            amendment.Revision = 2;

            var orderItem = original.Clone().OrderItems.First();

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(amendment, [original]));

            var result = await controller.ViewPrice(internalOrgId, callOffId, orderItem.CatalogueItemId);

            var expected = new ViewPriceModel(orderItem.OrderItemPrice, orderItem.CatalogueItem)
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            var actual = result.Should().BeOfType<ViewResult>().Subject;

            var actualModel = actual.Model.As<ViewPriceModel>();

            actualModel.Should().NotBeNull();
            actual.Model.Should()
                .BeEquivalentTo(
                    expected,
                    x => x.Excluding(m => m.BackLink)
                        .Excluding(m => m.OnwardLink)
                        .Excluding(m => m.Tiers));

            var actualTiers = actualModel.Tiers;

            actualTiers.Should().BeEquivalentTo(expected.Tiers, opt => opt.Excluding(m => m.Id));
        }
    }
}
