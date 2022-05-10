using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Prices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.SolutionSelection.Prices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.Prices
{
    public class EditPrice : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const string InternalOrgId = "CG-03F";
        private const int OrderId = 90005;
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
        };

        public EditPrice(LocalWebApplicationFactory factory)
            : base(factory, typeof(PricesController), nameof(PricesController.EditPrice), Parameters)
        {
        }

        [Fact]
        public void EditPrice_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo("Price of Catalogue Solution - DFOCVC Solution Full".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(ConfirmPriceObjects.AgreedPriceInput(0)).Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void EditPrice_PriceIsBlank_Error()
        {
            CommonActions.ClearInputElement(ConfirmPriceObjects.AgreedPriceInput(0));
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(PricesController),
                nameof(PricesController.EditPrice)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                ConfirmPriceObjects.AgreedPriceInputError(0),
                PricingTierModelValidator.PriceNotEnteredErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void EditPrice_PriceNotANumber_Error()
        {
            CommonActions.ElementAddValue(ConfirmPriceObjects.AgreedPriceInput(0), "abc");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(PricesController),
                nameof(PricesController.EditPrice)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                ConfirmPriceObjects.AgreedPriceInputError(0),
                PricingTierModelValidator.PriceNotNumericErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void EditPrice_PriceNegative_Error()
        {
            CommonActions.ElementAddValue(ConfirmPriceObjects.AgreedPriceInput(0), "-1");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(PricesController),
                nameof(PricesController.EditPrice)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                ConfirmPriceObjects.AgreedPriceInputError(0),
                PricingTierModelValidator.PriceNegativeErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void EditPrice_PriceHasMoreThanFourDecimalPlaces_Error()
        {
            CommonActions.ElementAddValue(ConfirmPriceObjects.AgreedPriceInput(0), "1.00001");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(PricesController),
                nameof(PricesController.EditPrice)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                ConfirmPriceObjects.AgreedPriceInputError(0),
                PricingTierModelValidator.PriceNotWithinFourDecimalPlacesErrorMessage).Should().BeTrue();
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(10000000.0001)]
        [InlineData(9999999.9999)]
        [InlineData(int.MaxValue)]
        public void EditPrice_PriceHigherThanListPrice_Error(decimal value)
        {
            CommonActions.ElementAddValue(ConfirmPriceObjects.AgreedPriceInput(0), $"{value}");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(PricesController),
                nameof(PricesController.EditPrice)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextContains(
                ConfirmPriceObjects.AgreedPriceInputError(0),
                PricingTierModelValidator.PriceHigherThanListPriceErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void EditPrice_NoChangesMade_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(QuantityController),
                nameof(QuantityController.SelectQuantity)).Should().BeTrue();

            var prices = GetPrices();

            prices.Count.Should().Be(1);
            prices[0].OrderItemPriceTiers.Count.Should().Be(1);

            var tier = prices[0].OrderItemPriceTiers.First();

            tier.ListPrice.Should().Be(999.9999M);
            tier.Price.Should().Be(999.9999M);
        }

        [Theory]
        [InlineData(999.9998)]
        [InlineData(100)]
        [InlineData(1)]
        [InlineData(0.0001)]
        public void EditPrice_PriceLowerThanListPrice_ExpectedResult(decimal value)
        {
            CommonActions.ElementAddValue(ConfirmPriceObjects.AgreedPriceInput(0), $"{value}");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(QuantityController),
                nameof(QuantityController.SelectQuantity)).Should().BeTrue();

            var prices = GetPrices();

            prices.Count.Should().Be(1);
            prices[0].OrderItemPriceTiers.Count.Should().Be(1);

            var tier = prices[0].OrderItemPriceTiers.First();

            tier.ListPrice.Should().Be(999.9999M);
            tier.Price.Should().Be(value);
        }

        [Fact]
        public void EditPrice_PriceIsZero_ExpectedResult()
        {
            CommonActions.ElementAddValue(ConfirmPriceObjects.AgreedPriceInput(0), "0");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(QuantityController),
                nameof(QuantityController.SelectQuantity)).Should().BeTrue();

            var prices = GetPrices();

            prices.Count.Should().Be(1);
            prices[0].OrderItemPriceTiers.Count.Should().Be(1);

            var tier = prices[0].OrderItemPriceTiers.First();

            tier.ListPrice.Should().Be(999.9999M);
            tier.Price.Should().Be(0M);
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();

            foreach (var price in GetPrices())
            {
                price.OrderItemPriceTiers.First().Price = 999.9999M;

                context.OrderItemPrices.Update(price);
            }

            context.SaveChanges();
        }

        private List<OrderItemPrice> GetPrices()
        {
            return GetEndToEndDbContext().OrderItemPrices
                .Include(x => x.OrderItemPriceTiers)
                .Where(x => x.OrderId == OrderId)
                .ToList();
        }
    }
}
