using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Ordering.Prices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.SolutionSelection.Prices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.Prices
{
    public class ConfirmPrice : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const string InternalOrgId = "CG-03F";
        private const int OrderId = 90012;
        private const int PriceId = 1;
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
            { nameof(PriceId), $"{PriceId}" },
        };

        public ConfirmPrice(LocalWebApplicationFactory factory)
            : base(factory, typeof(PricesController), nameof(PricesController.ConfirmPrice), Parameters)
        {
        }

        [Fact]
        public void ConfirmPrice_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo("Price of Catalogue Solution - E2E With Contact Multiple Prices".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(ConfirmPriceObjects.AgreedPriceInput(0)).Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void ConfirmPrice_PriceIsBlank_Error()
        {
            CommonActions.ClearInputElement(ConfirmPriceObjects.AgreedPriceInput(0));
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(PricesController),
                nameof(PricesController.ConfirmPrice)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                ConfirmPriceObjects.AgreedPriceInputError(0),
                PricingTierModelValidator.PriceNotEnteredErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void ConfirmPrice_PriceNotANumber_Error()
        {
            CommonActions.ElementAddValue(ConfirmPriceObjects.AgreedPriceInput(0), "abc");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(PricesController),
                nameof(PricesController.ConfirmPrice)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                ConfirmPriceObjects.AgreedPriceInputError(0),
                PricingTierModelValidator.PriceNotNumericErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void ConfirmPrice_PriceNegative_Error()
        {
            CommonActions.ElementAddValue(ConfirmPriceObjects.AgreedPriceInput(0), "-1");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(PricesController),
                nameof(PricesController.ConfirmPrice)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                ConfirmPriceObjects.AgreedPriceInputError(0),
                PricingTierModelValidator.PriceNegativeErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void ConfirmPrice_PriceHasMoreThanFourDecimalPlaces_Error()
        {
            CommonActions.ElementAddValue(ConfirmPriceObjects.AgreedPriceInput(0), "1.00001");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(PricesController),
                nameof(PricesController.ConfirmPrice)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                ConfirmPriceObjects.AgreedPriceInputError(0),
                PricingTierModelValidator.PriceNotWithinFourDecimalPlacesErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void ConfirmPrice_PriceHigherThanListPrice_Error()
        {
            CommonActions.ElementAddValue(ConfirmPriceObjects.AgreedPriceInput(0), $"{int.MaxValue}");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(PricesController),
                nameof(PricesController.ConfirmPrice)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextContains(
                ConfirmPriceObjects.AgreedPriceInputError(0),
                PricingTierModelValidator.PriceHigherThanListPriceErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void ConfirmPrice_NoChangesMade_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();

            var prices = GetPrices();

            prices.Count.Should().Be(1);
            prices[0].OrderItemPriceTiers.Count.Should().Be(1);

            var tier = prices[0].OrderItemPriceTiers.First();

            tier.ListPrice.Should().Be(999.9999M);
            tier.Price.Should().Be(999.9999M);
        }

        [Fact]
        public void ConfirmPrice_PriceLowerThanListPrice_ExpectedResult()
        {
            CommonActions.ElementAddValue(ConfirmPriceObjects.AgreedPriceInput(0), "0.0001");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();

            var prices = GetPrices();

            prices.Count.Should().Be(1);
            prices[0].OrderItemPriceTiers.Count.Should().Be(1);

            var tier = prices[0].OrderItemPriceTiers.First();

            tier.ListPrice.Should().Be(999.9999M);
            tier.Price.Should().Be(0.0001M);
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
                .Where(x => x.OrderId == OrderId)
                .ToList();
        }
    }
}
