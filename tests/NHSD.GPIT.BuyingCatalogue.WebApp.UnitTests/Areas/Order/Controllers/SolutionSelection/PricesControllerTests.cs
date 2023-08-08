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
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Prices;
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
            [Frozen] Mock<IListPriceService> mockListPriceService,
            PricesController controller)
        {
            var orderItem = order.OrderItems.First().CatalogueItem;

            orderItem.CatalogueItemType = CatalogueItemType.Solution;

            mockListPriceService
                .Setup(lps => lps.GetCatalogueItemWithPublishedListPrices(orderItem.Id))
                .ReturnsAsync(orderItem);

            var result = await controller.SelectPrice(internalOrgId, callOffId, orderItem.Id);

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
        [CommonAutoData]
        public static async Task Post_SelectPrice_ModelError_ReturnsModel(
            string internalOrgId,
            CallOffId callOffId,
            SelectPriceModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IListPriceService> mockListPriceService,
            PricesController controller)
        {
            var orderItem = order.OrderItems.First().CatalogueItem;

            orderItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockListPriceService
                .Setup(lps => lps.GetCatalogueItemWithPublishedListPrices(orderItem.Id))
                .ReturnsAsync(orderItem);

            controller.ModelState.AddModelError("key", "message");

            var result = await controller.SelectPrice(internalOrgId, callOffId, orderItem.Id, model);

            mockOrderService.VerifyAll();
            mockListPriceService.VerifyAll();

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
        [CommonAutoData]
        public static async Task Get_ConfirmPrice_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IListPriceService> mockListPriceService,
            PricesController controller)
        {
            var orderItem = order.OrderItems.First().CatalogueItem;
            var priceId = orderItem.CataloguePrices.First().CataloguePriceId;

            orderItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockOrderService
                .Setup(x => x.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockListPriceService
                .Setup(lps => lps.GetCatalogueItemWithPublishedListPrices(orderItem.Id))
                .ReturnsAsync(orderItem);

            var result = await controller.ConfirmPrice(internalOrgId, callOffId, orderItem.Id, priceId);

            mockOrderService.VerifyAll();
            mockListPriceService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new ConfirmPriceModel(orderItem, priceId, null);

            actualResult.Model.Should().BeEquivalentTo(expected, m => m.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
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
        [CommonAutoData]
        public static async Task Post_ConfirmPrice_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IListPriceService> mockListPriceService,
            [Frozen] Mock<IOrderPriceService> mockOrderPriceService,
            [Frozen] Mock<IRoutingService> mockRoutingService,
            PricesController controller)
        {
            var catalogueItem = order.OrderItems.First().CatalogueItem;
            var price = catalogueItem.CataloguePrices.First();

            catalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockListPriceService
                .Setup(lps => lps.GetCatalogueItemWithPublishedListPrices(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            mockRoutingService
                .Setup(x => x.GetRoute(RoutingPoint.ConfirmPrice, order, It.IsAny<RouteValues>()))
                .Returns(new RoutingResult { ActionName = Constants.Actions.SelectQuantity, ControllerName = Constants.Controllers.Quantity });

            List<PricingTierDto> actual = null;

            mockOrderPriceService
                .Setup(x => x.UpsertPrice(order.Id, price, It.IsAny<List<PricingTierDto>>()))
                .Callback<int, CataloguePrice, List<PricingTierDto>>((_, _, x) => actual = x)
                .Returns(Task.CompletedTask);

            var model = new ConfirmPriceModel(catalogueItem, price.CataloguePriceId, null);

            var result = await controller.ConfirmPrice(internalOrgId, callOffId, catalogueItem.Id, price.CataloguePriceId, model);

            mockOrderService.VerifyAll();
            mockListPriceService.VerifyAll();
            mockOrderPriceService.VerifyAll();
            mockRoutingService.VerifyAll();

            actual.ForEach(x =>
            {
                price.CataloguePriceTiers
                    .First(t => t.LowerRange == x.LowerRange && t.UpperRange == x.UpperRange)
                    .Price.Should().Be(x.Price);
            });

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(QuantityController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(QuantityController.SelectQuantity));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditPrice_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            PricesController controller)
        {
            var orderItem = order.OrderItems.First();

            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.EditPrice(internalOrgId, callOffId, orderItem.CatalogueItemId);

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var expected = new ConfirmPriceModel(orderItem);

            actualResult.Model.Should().BeEquivalentTo(expected, m => m.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditPrice_MultiplePricesAvailable_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            CatalogueItem catalogueItem,
            List<CataloguePrice> availablePrices,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IListPriceService> mockListPriceService,
            PricesController controller)
        {
            var orderItem = order.OrderItems.First();

            catalogueItem.CataloguePrices = availablePrices;

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockListPriceService
                .Setup(x => x.GetCatalogueItemWithPublishedListPrices(orderItem.CatalogueItemId))
                .ReturnsAsync(catalogueItem);

            var result = await controller.EditPrice(internalOrgId, callOffId, orderItem.CatalogueItemId);

            mockOrderService.VerifyAll();
            mockListPriceService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var expected = new ConfirmPriceModel(orderItem);

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditPrice_MultiplePricesAvailable_FromTaskList_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            CatalogueItem catalogueItem,
            List<CataloguePrice> availablePrices,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IListPriceService> mockListPriceService,
            PricesController controller)
        {
            var orderItem = order.OrderItems.First();

            catalogueItem.CataloguePrices = availablePrices;

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockListPriceService
                .Setup(x => x.GetCatalogueItemWithPublishedListPrices(orderItem.CatalogueItemId))
                .ReturnsAsync(catalogueItem);

            var result = await controller.EditPrice(internalOrgId, callOffId, orderItem.CatalogueItemId, RoutingSource.TaskList);

            mockOrderService.VerifyAll();
            mockListPriceService.VerifyAll();

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
        [CommonAutoData]
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
        [CommonAutoData]
        public static async Task Post_EditPrice_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IOrderPriceService> mockOrderPriceService,
            [Frozen] Mock<IRoutingService> mockRoutingService,
            PricesController controller)
        {
            var orderItem = order.OrderItems.First();

            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            List<PricingTierDto> actual = null;

            mockOrderPriceService
                .Setup(x => x.UpdatePrice(order.Id, orderItem.CatalogueItemId, It.IsAny<List<PricingTierDto>>()))
                .Callback<int, CatalogueItemId, List<PricingTierDto>>((_, _, x) => actual = x)
                .Returns(Task.CompletedTask);

            var model = new ConfirmPriceModel(orderItem);

            model.Tiers.ForEach(x =>
            {
                var newPrice = x.ListPrice - 0.0001M;
                x.AgreedPrice = $"{newPrice:#,##0.00##}";
            });

            mockRoutingService
                .Setup(x => x.GetRoute(RoutingPoint.EditPrice, order, It.IsAny<RouteValues>()))
                .Returns(new RoutingResult
                {
                    ActionName = nameof(QuantityController.SelectQuantity),
                    ControllerName = typeof(QuantityController).ControllerName(),
                    RouteValues = new { internalOrgId, callOffId, orderItem.CatalogueItemId },
                });

            var result = await controller.EditPrice(internalOrgId, callOffId, orderItem.CatalogueItemId, model);

            mockOrderService.VerifyAll();
            mockOrderPriceService.VerifyAll();
            mockRoutingService.VerifyAll();

            actual.ForEach(x =>
            {
                model.Tiers
                    .First(t => t.LowerRange == x.LowerRange && t.UpperRange == x.UpperRange)
                    .AgreedPrice.Should().Be($"{x.Price:#,##0.00##}");
            });

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(QuantityController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(QuantityController.SelectQuantity));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "catalogueItemId", orderItem.CatalogueItemId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ViewPrice_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order original,
            EntityFramework.Ordering.Models.Order amendment,
            [Frozen] Mock<IOrderService> mockOrderService,
            PricesController controller)
        {
            original.Revision = 1;
            amendment.OrderNumber = original.OrderNumber;
            amendment.Revision = 2;

            var orders = new[] { original, amendment };

            var orderItem = original.Clone().OrderItems.First();

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(orders));

            var result = await controller.ViewPrice(internalOrgId, callOffId, orderItem.CatalogueItemId);

            mockOrderService.VerifyAll();

            var expected = new ViewPriceModel(orderItem)
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            var actual = result.Should().BeOfType<ViewResult>().Subject;

            actual.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink).Excluding(m => m.OnwardLink));
        }
    }
}
