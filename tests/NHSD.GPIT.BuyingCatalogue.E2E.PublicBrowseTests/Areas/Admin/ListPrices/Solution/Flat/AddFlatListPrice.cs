using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ListPrices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices.Solution.Flat
{
    public sealed class AddFlatListPrice : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new CatalogueItemId(99998, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public AddFlatListPrice(LocalWebApplicationFactory factory)
            : base(
                factory,
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.AddFlatListPrice),
                Parameters)
        {
        }

        [Fact]
        public void AllSectionsDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Single(c => c.Id == SolutionId);

            CommonActions.PageTitle().Should().Be($"Add a flat list price - {catalogueItem.Name}".FormatForComparison());
            CommonActions.LedeText().Should().Be("Provide the following information about the pricing model for your Catalogue Solution.".FormatForComparison());

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(ListPriceObjects.ProvisioningTypeInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPriceObjects.UnitDescriptionInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPriceObjects.UnitDefinitionInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPriceObjects.PriceInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPriceObjects.PublicationStatusInput).Should().BeTrue();

            CommonActions.ElementIsDisplayed(ListPriceObjects.DeletePriceLink).Should().BeFalse();

            CommonActions.ElementIsDisplayed(ListPriceObjects.OnDemandBillingPeriodInput).Should().BeFalse();
            CommonActions.ElementIsDisplayed(ListPriceObjects.DeclarativeBillingPeriodInput).Should().BeFalse();

            CommonActions.ElementIsDisplayed(ListPriceObjects.OnDemandQuantityCalculationRadioButtons).Should().BeFalse();
            CommonActions.ElementIsDisplayed(ListPriceObjects.DeclarativeQuantityCalculationRadioButtons).Should().BeFalse();

            CommonActions.ClickRadioButtonWithValue(ListPriceObjects.ProvisioningTypeRadioButtons, ProvisioningType.PerServiceRecipient.ToString());
            CommonActions.ElementIsDisplayed(ListPriceObjects.PerServiceRecipientBillingPeriodInput).Should().BeTrue();

            CommonActions.ClickRadioButtonWithValue(ListPriceObjects.ProvisioningTypeRadioButtons, ProvisioningType.OnDemand.ToString());
            CommonActions.ElementIsDisplayed(ListPriceObjects.OnDemandQuantityCalculationRadioButtons).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPriceObjects.OnDemandBillingPeriodInput).Should().BeTrue();

            CommonActions.ClickRadioButtonWithValue(ListPriceObjects.ProvisioningTypeRadioButtons, ProvisioningType.Declarative.ToString());
            CommonActions.ElementIsDisplayed(ListPriceObjects.DeclarativeQuantityCalculationRadioButtons).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPriceObjects.DeclarativeBillingPeriodInput).Should().BeTrue();
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

            CommonActions.ElementShowingCorrectErrorMessage(ListPriceObjects.ProvisioningTypeInputError, "Error: Select a provisioning type").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(ListPriceObjects.PublicationStatusInputError, "Error: Select a publication status").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(ListPriceObjects.UnitDescriptionInputError, "Enter a unit").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(ListPriceObjects.PriceInputError, "Enter a price").Should().BeTrue();
        }

        [Fact]
        public void Submit_Duplicate_ThrowsError()
        {
            var catalogueItem = GetCatalogueItemWithPrices(SolutionId);
            var price = catalogueItem.CataloguePrices.First(p => p.CataloguePriceType == CataloguePriceType.Flat);

            CommonActions.ClickRadioButtonWithValue(price.ProvisioningType.ToString());
            CommonActions.ClickRadioButtonWithValue(price.PublishedStatus.ToString());

            CommonActions.ElementAddValue(ListPriceObjects.UnitDescriptionInput, price.PricingUnit.Description);
            CommonActions.ElementAddValue(ListPriceObjects.PriceInput, price.CataloguePriceTiers.First().Price.ToString());

            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(ListPriceObjects.ProvisioningTypeInputError, "Error: A list price with these details already exists").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(ListPriceObjects.UnitDescriptionInputError, "A list price with these details already exists").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(ListPriceObjects.PriceInputError, "A list price with these details already exists").Should().BeTrue();
        }

        [Fact]
        public void Submit_Input_NavigatesToCorrectPage()
        {
            CommonActions.ClickRadioButtonWithValue(ProvisioningType.Patient.ToString());
            CommonActions.ClickRadioButtonWithValue(PublicationStatus.Published.ToString());

            TextGenerators.TextInputAddText(ListPriceObjects.UnitDescriptionInput, 100);
            CommonActions.ElementAddValue(ListPriceObjects.PriceInput, "3.14");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.Index)).Should().BeTrue();
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
