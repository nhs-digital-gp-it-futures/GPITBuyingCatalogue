using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ListPrices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices.Base.Tiered
{
    [Collection(nameof(AdminCollection))]
    public abstract class AddTieredPriceTierBase : AuthorityTestBase
    {
        private readonly IDictionary<string, string> parameters;

        protected AddTieredPriceTierBase(
            LocalWebApplicationFactory factory,
            Type controller,
            IDictionary<string, string> parameters)
            : base(
                  factory,
                  controller,
                  "AddTieredPriceTier",
                  parameters)
        {
            this.parameters = parameters;
        }

        protected abstract int CataloguePriceId { get; }

        protected abstract CatalogueItemId CatalogueItemId { get; }

        protected abstract Type Controller { get; }

        [Fact]
        public void AllSectionsDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.First(c => c.Id == CatalogueItemId);

            CommonActions.PageTitle().Should().Be($"Add a pricing tier - {catalogueItem.Name}".FormatForComparison());
            CommonActions.LedeText().Should().Be("Provide information about this tier.".FormatForComparison());

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(AddTieredPriceTierObjects.PriceInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddTieredPriceTierObjects.LowerRangeInput).Should().BeTrue();

            CommonActions.GetNumberOfRadioButtonsDisplayed().Should().Be(2);
            CommonActions.ElementIsDisplayed(AddTieredPriceTierObjects.UpperRangeInput).Should().BeFalse();

            CommonActions.ClickRadioButtonWithText("Set upper range");
            CommonActions.ElementIsDisplayed(AddTieredPriceTierObjects.UpperRangeInput).Should().BeTrue();
        }

        [Fact]
        public void ClickGoBackLink_Adding_NavigatesCorrectly()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "TieredPriceTiers").Should().BeTrue();
        }

        [Fact]
        public void ClickGoBackLink_Editing_NavigatesCorrectly()
        {
            NavigateToUrl(
                Controller,
                "AddTieredPriceTier",
                parameters,
                new Dictionary<string, string> { { "isEditing", true.ToString() } });

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                nameof(CatalogueSolutionListPriceController.EditTieredListPrice)).Should().BeTrue();
        }

        [Fact]
        public void ClickSubmit_NoInput_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(AddTieredPriceTierObjects.PriceInputError, "Enter a price").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddTieredPriceTierObjects.LowerRangeInputError, "Enter a lower range").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddTieredPriceTierObjects.RangeTypeInputError, "Error: Select how you want to define the upper range").Should().BeTrue();
        }

        [Fact]
        public async Task ClickSubmit_Input_RedirectsCorrectly()
        {
            const decimal price = 3.14m;
            const int lowerRange = 1;

            CommonActions.ElementAddValue(AddTieredPriceTierObjects.PriceInput, price.ToString(CultureInfo.InvariantCulture));
            CommonActions.ElementAddValue(AddTieredPriceTierObjects.LowerRangeInput, lowerRange.ToString());
            CommonActions.ClickRadioButtonWithText("Infinite upper range");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "TieredPriceTiers").Should().BeTrue();

            await RemovePriceTier(price, lowerRange, null);
        }

        [Fact]
        public async Task ClickSubmit_Input_Editing_RedirectsCorrectly()
        {
            const decimal price = 3.10m;
            const int lowerRange = 15;

            NavigateToUrl(
                Controller,
                "AddTieredPriceTier",
                parameters,
                new Dictionary<string, string> { { "isEditing", true.ToString() } });

            CommonActions.ElementAddValue(AddTieredPriceTierObjects.PriceInput, price.ToString(CultureInfo.InvariantCulture));
            CommonActions.ElementAddValue(AddTieredPriceTierObjects.LowerRangeInput, lowerRange.ToString());
            CommonActions.ClickRadioButtonWithText("Infinite upper range");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "EditTieredListPrice").Should().BeTrue();

            await RemovePriceTier(price, lowerRange, null);
        }

        [Fact]
        public void ClickSubmit_NoUpperRange_ThrowsError()
        {
            CommonActions.ClickRadioButtonWithText("Set upper range");

            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(AddTieredPriceTierObjects.PriceInputError, "Enter a price").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddTieredPriceTierObjects.LowerRangeInputError, "Enter a lower range").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddTieredPriceTierObjects.UpperRangeInputError, "Enter an upper range").Should().BeTrue();
        }

        private async Task RemovePriceTier(decimal price, int lowerRange, int? upperRange)
        {
            await using var context = GetEndToEndDbContext();
            var tier = context.CataloguePriceTiers.First(
                x => x.CataloguePriceId == CataloguePriceId
                    && x.LowerRange == lowerRange
                    && x.UpperRange == null
                    && x.Price == price);

            context.CataloguePriceTiers.Remove(tier);
            await context.SaveChangesAsync();
        }
    }
}
