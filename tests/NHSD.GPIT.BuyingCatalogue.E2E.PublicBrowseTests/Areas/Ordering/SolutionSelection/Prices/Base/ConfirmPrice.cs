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
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.Prices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.Prices.Base
{
    [Collection(nameof(OrderingCollection))]
    public abstract class ConfirmPrice : BuyerTestBase, IDisposable
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
            CommonActions.ElementIsDisplayed(PriceObjects.AgreedPriceInput(0)).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.PriceDetailsHelp).Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void ConfirmPrice_PriceIsBlank_Error()
        {
            CommonActions.ClearInputElement(PriceObjects.AgreedPriceInput(0));
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(PricesController),
                nameof(PricesController.ConfirmPrice)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                PriceObjects.AgreedPriceInputError(0),
                PricingTierModelValidator.PriceNotEnteredErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void ConfirmPrice_PriceNotANumber_Error()
        {
            CommonActions.ElementAddValue(PriceObjects.AgreedPriceInput(0), "abc");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(PricesController),
                nameof(PricesController.ConfirmPrice)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                PriceObjects.AgreedPriceInputError(0),
                PricingTierModelValidator.PriceNotNumericErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void ConfirmPrice_PriceNegative_Error()
        {
            CommonActions.ElementAddValue(PriceObjects.AgreedPriceInput(0), "-1");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(PricesController),
                nameof(PricesController.ConfirmPrice)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                PriceObjects.AgreedPriceInputError(0),
                PricingTierModelValidator.PriceNegativeErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void ConfirmPrice_PriceHasMoreThanFourDecimalPlaces_Error()
        {
            CommonActions.ElementAddValue(PriceObjects.AgreedPriceInput(0), "1.00001");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(PricesController),
                nameof(PricesController.ConfirmPrice)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                PriceObjects.AgreedPriceInputError(0),
                PricingTierModelValidator.PriceNotWithinFourDecimalPlacesErrorMessage).Should().BeTrue();
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(100000)]
        [InlineData(99999.9999)]
        [InlineData(int.MaxValue)]
        public void ConfirmPrice_PriceHigherThanListPrice_Error(decimal value)
        {
            CommonActions.ElementAddValue(PriceObjects.AgreedPriceInput(0), $"{value:#,#0.00##}");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(PricesController),
                nameof(PricesController.ConfirmPrice)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextContains(
                PriceObjects.AgreedPriceInputError(0),
                PricingTierModelValidator.PriceHigherThanListPriceErrorMessage).Should().BeTrue();
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
            CommonActions.ElementAddValue(PriceObjects.AgreedPriceInput(0), $"{value:#,##0.00##}");
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
