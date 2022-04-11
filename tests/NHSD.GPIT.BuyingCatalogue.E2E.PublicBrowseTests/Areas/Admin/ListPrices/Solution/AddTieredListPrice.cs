using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.ListPrices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices.Solution
{
    public sealed class AddTieredListPrice : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new CatalogueItemId(99998, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public AddTieredListPrice(LocalWebApplicationFactory factory)
            : base(
                factory,
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.AddTieredListPrice),
                Parameters)
        {
        }

        [Fact]
        public void AllSectionsDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Single(c => c.Id == SolutionId);

            CommonActions.PageTitle().Should().Be($"Add a tiered list price - {catalogueItem.Name}".FormatForComparison());
            CommonActions.LedeText().Should().Be("Provide the following information about the pricing model for your Catalogue Solution.".FormatForComparison());

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(AddTieredListPriceObjects.ProvisioningTypeInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddTieredListPriceObjects.CalculationTypeInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddTieredListPriceObjects.UnitDescriptionInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddTieredListPriceObjects.UnitDefinitionInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddTieredListPriceObjects.RangeDefinitionInput).Should().BeTrue();

            CommonActions.ElementIsDisplayed(AddTieredListPriceObjects.OnDemandBillingPeriodInput).Should().BeFalse();
            CommonActions.ElementIsDisplayed(AddTieredListPriceObjects.DeclarativeBillingPeriodInput).Should().BeFalse();

            CommonActions.ClickRadioButtonWithValue(ProvisioningType.OnDemand.ToString());
            CommonActions.ElementIsDisplayed(AddTieredListPriceObjects.OnDemandBillingPeriodInput).Should().BeTrue();

            CommonActions.ClickRadioButtonWithValue(ProvisioningType.Declarative.ToString());
            CommonActions.ElementIsDisplayed(AddTieredListPriceObjects.DeclarativeBillingPeriodInput).Should().BeTrue();
        }

        [Fact]
        public void ClickGoBackLink_NavigatesToCorrectPage()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.ListPriceType)).Should().BeTrue();
        }

        [Fact]
        public void Submit_NoInput_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(AddTieredListPriceObjects.ProvisioningTypeInputError, "Error: Select a provisioning type").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddTieredListPriceObjects.CalculationTypeInputError, "Error: Select a calculation type").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddTieredListPriceObjects.UnitDescriptionInputError, "Enter a unit").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddTieredListPriceObjects.RangeDefinitionInputError, "Enter a range definition").Should().BeTrue();
        }

        [Fact]
        public void Submit_Duplicate_ThrowsError()
        {
            var catalogueItem = GetCatalogueItemWithPrices(SolutionId);
            var price = catalogueItem.CataloguePrices.First();

            CommonActions.ClickRadioButtonWithValue(price.ProvisioningType.ToString());
            CommonActions.ClickRadioButtonWithValue(price.CataloguePriceCalculationType.ToString());

            CommonActions.ElementAddValue(AddTieredListPriceObjects.UnitDescriptionInput, price.PricingUnit.Description);
            CommonActions.ElementAddValue(AddTieredListPriceObjects.RangeDefinitionInput, price.PricingUnit.RangeDescription);

            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(AddTieredListPriceObjects.ProvisioningTypeInputError, "Error: A list price with these details already exists").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddTieredListPriceObjects.CalculationTypeInputError, "Error: A list price with these details already exists").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddTieredListPriceObjects.UnitDescriptionInputError, "A list price with these details already exists").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddTieredListPriceObjects.RangeDefinitionInputError, "A list price with these details already exists").Should().BeTrue();
        }

        [Fact]
        public void Submit_Input_NavigatesToCorrectPage()
        {
            CommonActions.ClickRadioButtonWithValue(ProvisioningType.Patient.ToString());
            CommonActions.ClickRadioButtonWithValue(CataloguePriceCalculationType.Volume.ToString());

            TextGenerators.TextInputAddText(AddTieredListPriceObjects.UnitDescriptionInput, 100);
            TextGenerators.TextInputAddText(AddTieredListPriceObjects.RangeDefinitionInput, 100);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.TieredPriceTiers)).Should().BeTrue();
        }

        private CatalogueItem GetCatalogueItemWithPrices(CatalogueItemId id)
            => GetEndToEndDbContext()
            .CatalogueItems
            .Include(ci => ci.CataloguePrices)
            .ThenInclude(p => p.PricingUnit)
            .Include(ci => ci.CataloguePrices)
            .ThenInclude(p => p.CataloguePriceTiers)
            .AsNoTracking()
            .Single(ci => ci.Id == id);
    }
}
