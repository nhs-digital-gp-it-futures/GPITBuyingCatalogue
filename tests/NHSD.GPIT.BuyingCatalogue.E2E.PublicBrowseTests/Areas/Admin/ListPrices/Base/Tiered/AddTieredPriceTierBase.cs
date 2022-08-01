using System;
using System.Collections.Generic;
using System.Linq;
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
    public abstract class AddTieredPriceTierBase : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
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
        public void ClickSubmit_InvalidModelState_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(AddTieredPriceTierObjects.PriceInputError).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddTieredPriceTierObjects.LowerRangeInputError).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddTieredPriceTierObjects.RangeTypeInputError).Should().BeTrue();

            CommonActions.ClickRadioButtonWithText("Set upper range");

            CommonActions.ClickSave();

            CommonActions.ElementIsDisplayed(AddTieredPriceTierObjects.UpperRangeInputError).Should().BeTrue();
        }

        [Fact]
        public void ClickSubmit_Input_RedirectsCorrectly()
        {
            CommonActions.ElementAddValue(AddTieredPriceTierObjects.PriceInput, "3.14");
            CommonActions.ElementAddValue(AddTieredPriceTierObjects.LowerRangeInput, "1");
            CommonActions.ClickRadioButtonWithText("Infinite upper range");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "TieredPriceTiers").Should().BeTrue();
        }

        [Fact]
        public void ClickSubmit_Input_Editing_RedirectsCorrectly()
        {
            NavigateToUrl(
                Controller,
                "AddTieredPriceTier",
                parameters,
                new Dictionary<string, string> { { "isEditing", true.ToString() } });

            CommonActions.ElementAddValue(AddTieredPriceTierObjects.PriceInput, "3.10");
            CommonActions.ElementAddValue(AddTieredPriceTierObjects.LowerRangeInput, "15");
            CommonActions.ClickRadioButtonWithText("Infinite upper range");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "EditTieredListPrice").Should().BeTrue();
        }
    }
}
