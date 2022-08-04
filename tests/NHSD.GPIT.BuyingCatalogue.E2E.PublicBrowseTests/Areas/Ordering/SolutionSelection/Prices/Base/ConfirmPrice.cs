using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Prices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.SolutionSelection.Prices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.Prices.Base
{
    public abstract class ConfirmPrice : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private readonly int orderId;
        private readonly CatalogueItemId catalogueItemId;

        protected ConfirmPrice(LocalWebApplicationFactory factory, Dictionary<string, string> parameters)
            : base(factory, typeof(PricesController), nameof(PricesController.ConfirmPrice), parameters)
        {
            orderId = int.Parse(parameters["OrderId"]);
            catalogueItemId = CatalogueItemId.ParseExact(parameters["CatalogueItemId"]);
        }

        protected abstract decimal ListPrice { get; }

        protected abstract string PageTitle { get; }

        [Fact]
        public void ConfirmPrice_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo(PageTitle.FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(ConfirmPriceObjects.AgreedPriceInput(0)).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.PriceDetailsHelp).Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void ConfirmPrice_NoInputs_ThrowsError()
        {
            CommonActions.ClearInputElement(ConfirmPriceObjects.AgreedPriceInput(0));
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(PricesController),
                nameof(PricesController.ConfirmPrice)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(
                ConfirmPriceObjects.AgreedPriceInputError(0)).Should().BeTrue();
        }

        [Fact]
        public void ConfirmPrice_NoChangesMade_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(QuantityController),
                nameof(QuantityController.SelectServiceRecipientQuantity)).Should().BeTrue();

            var prices = GetPrices();

            prices.Count.Should().Be(1);
            prices[0].OrderItemPriceTiers.Count.Should().Be(1);

            var tier = prices[0].OrderItemPriceTiers.First();

            tier.ListPrice.Should().Be(ListPrice);
            tier.Price.Should().Be(ListPrice);
        }

        [Theory]
        [InlineData(999.9998)]
        [InlineData(100)]
        [InlineData(1)]
        [InlineData(0.0001)]
        public void ConfirmPrice_PriceLowerThanListPrice_ExpectedResult(decimal value)
        {
            CommonActions.ElementAddValue(ConfirmPriceObjects.AgreedPriceInput(0), $"{value:#,##0.00##}");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(QuantityController),
                nameof(QuantityController.SelectServiceRecipientQuantity)).Should().BeTrue();

            var prices = GetPrices();

            prices.Count.Should().Be(1);
            prices[0].OrderItemPriceTiers.Count.Should().Be(1);

            var tier = prices[0].OrderItemPriceTiers.First();

            tier.ListPrice.Should().Be(ListPrice);
            tier.Price.Should().Be(value);
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();

            foreach (var price in GetPrices())
            {
                context.OrderItemPrices.Remove(price);
            }

            context.SaveChanges();
        }

        private List<OrderItemPrice> GetPrices()
        {
            return GetEndToEndDbContext().OrderItemPrices
                .Include(x => x.OrderItemPriceTiers)
                .Where(x => x.OrderId == orderId
                    && x.CatalogueItemId == catalogueItemId)
                .ToList();
        }
    }
}
