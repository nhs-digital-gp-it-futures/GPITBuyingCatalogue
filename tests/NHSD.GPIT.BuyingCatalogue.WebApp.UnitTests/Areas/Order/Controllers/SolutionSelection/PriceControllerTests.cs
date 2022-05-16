using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
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
            [Frozen] Mock<ISolutionListPriceService> mockListPriceService,
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

            var result = await controller.SelectPrice(internalOrgId, callOffId, orderItem.Id, model);

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
            [Frozen] Mock<ISolutionListPriceService> mockListPriceService,
            PricesController controller)
        {
            var orderItem = order.OrderItems.First().CatalogueItem;
            var priceId = orderItem.CataloguePrices.First().CataloguePriceId;

            orderItem.CatalogueItemType = CatalogueItemType.Solution;

            mockListPriceService
                .Setup(lps => lps.GetCatalogueItemWithPublishedListPrices(orderItem.Id))
                .ReturnsAsync(orderItem);

            var result = await controller.ConfirmPrice(internalOrgId, callOffId, orderItem.Id, priceId);

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
            [Frozen] Mock<ISolutionListPriceService> mockListPriceService,
            [Frozen] Mock<IOrderPriceService> mockOrderPriceService,
            PricesController controller)
        {
            var catalogueItem = order.OrderItems.First().CatalogueItem;
            var price = catalogueItem.CataloguePrices.First();

            catalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(order);

            mockListPriceService
                .Setup(lps => lps.GetCatalogueItemWithPublishedListPrices(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            List<OrderPricingTierDto> actual = null;

            mockOrderPriceService
                .Setup(x => x.AddPrice(order.Id, price, It.IsAny<List<OrderPricingTierDto>>()))
                .Callback<int, CataloguePrice, List<OrderPricingTierDto>>((_, _, x) => actual = x)
                .Returns(Task.CompletedTask);

            var model = new ConfirmPriceModel(catalogueItem, price.CataloguePriceId);

            var result = await controller.ConfirmPrice(internalOrgId, callOffId, catalogueItem.Id, price.CataloguePriceId, model);

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

            actualResult.ControllerName.Should().Be(typeof(QuantityController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(QuantityController.SelectQuantity));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "catalogueItemId", catalogueItem.Id },
            });
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
                .ReturnsAsync(order);

            var result = await controller.EditPrice(internalOrgId, callOffId, orderItem.CatalogueItemId);

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var expected = new ConfirmPriceModel(orderItem);

            actualResult.Model.Should().BeEquivalentTo(expected, m => m.Excluding(o => o.BackLink));
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
            PricesController controller)
        {
            var orderItem = order.OrderItems.First();

            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(order);

            List<OrderPricingTierDto> actual = null;

            mockOrderPriceService
                .Setup(x => x.UpdatePrice(order.Id, orderItem.CatalogueItemId, It.IsAny<List<OrderPricingTierDto>>()))
                .Callback<int, CatalogueItemId, List<OrderPricingTierDto>>((_, _, x) => actual = x)
                .Returns(Task.CompletedTask);

            var model = new ConfirmPriceModel(orderItem);

            model.Tiers.ForEach(x =>
            {
                var newPrice = x.ListPrice - 0.0001M;
                x.AgreedPrice = $"{newPrice:#,##0.00##}";
            });

            var result = await controller.EditPrice(internalOrgId, callOffId, orderItem.CatalogueItemId, model);

            mockOrderService.VerifyAll();
            mockOrderPriceService.VerifyAll();

            actual.ForEach(x =>
            {
                model.Tiers
                    .Single(t => t.LowerRange == x.LowerRange && t.UpperRange == x.UpperRange)
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
    }
}
