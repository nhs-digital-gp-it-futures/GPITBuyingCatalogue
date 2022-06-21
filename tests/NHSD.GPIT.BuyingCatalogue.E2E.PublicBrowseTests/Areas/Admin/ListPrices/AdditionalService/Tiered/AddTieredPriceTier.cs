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

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices.AdditionalService.Tiered
{
    public sealed class AddTieredPriceTier : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const int CataloguePriceId = 14;
        private static readonly CatalogueItemId SolutionId = new(99998, "001");
        private static readonly CatalogueItemId AdditionalServiceId = new(99998, "001A99");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
            { nameof(CataloguePriceId), CataloguePriceId.ToString() },
        };

        public AddTieredPriceTier(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AdditionalServiceListPriceController),
                  nameof(AdditionalServiceListPriceController.AddTieredPriceTier),
                  Parameters)
        {
        }

        [Fact]
        public void AllSectionsDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.First(c => c.Id == AdditionalServiceId);

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
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.TieredPriceTiers)).Should().BeTrue();
        }

        [Fact]
        public void ClickGoBackLink_Editing_NavigatesCorrectly()
        {
            NavigateToUrl(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.AddTieredPriceTier),
                Parameters,
                new Dictionary<string, string> { { "isEditing", true.ToString() } });

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.EditTieredListPrice)).Should().BeTrue();
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
        public void ClickSubmit_Input_RedirectsCorrectly()
        {
            CommonActions.ElementAddValue(AddTieredPriceTierObjects.PriceInput, "3.14");
            CommonActions.ElementAddValue(AddTieredPriceTierObjects.LowerRangeInput, "1");
            CommonActions.ClickRadioButtonWithText("Infinite upper range");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.TieredPriceTiers)).Should().BeTrue();
        }

        [Fact]
        public void ClickSubmit_Input_Editing_RedirectsCorrectly()
        {
            NavigateToUrl(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.AddTieredPriceTier),
                Parameters,
                new Dictionary<string, string> { { "isEditing", true.ToString() } });

            CommonActions.ElementAddValue(AddTieredPriceTierObjects.PriceInput, "3.10");
            CommonActions.ElementAddValue(AddTieredPriceTierObjects.LowerRangeInput, "15");
            CommonActions.ClickRadioButtonWithText("Infinite upper range");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.EditTieredListPrice)).Should().BeTrue();
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
    }
}
