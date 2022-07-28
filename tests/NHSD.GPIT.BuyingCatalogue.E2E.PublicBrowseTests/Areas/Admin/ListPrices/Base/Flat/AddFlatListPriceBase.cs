using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ListPrices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices.Base.Flat
{
    public abstract class AddFlatListPriceBase : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string ActionName = "AddFlatListPrice";

        protected AddFlatListPriceBase(
            LocalWebApplicationFactory factory,
            Type controller,
            IDictionary<string, string> parameters)
            : base(
                factory,
                controller,
                ActionName,
                parameters)
        {
        }

        protected abstract CatalogueItemId CatalogueItemId { get; }

        protected abstract Type Controller { get; }

        [Fact]
        public void AllSectionsDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Single(c => c.Id == CatalogueItemId);

            CommonActions.PageTitle().Should().Be($"Add a flat list price - {catalogueItem.Name}".FormatForComparison());
            CommonActions.LedeText().Should().Be($"Provide the following information about the pricing model for your {catalogueItem.CatalogueItemType.Name()}.".FormatForComparison());

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
                Controller,
                "Index").Should().BeTrue();
        }

        [Fact]
        public void Submit_InvalidModelState_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                ActionName).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(ListPriceObjects.ProvisioningTypeInputError).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPriceObjects.CalculationTypeInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPriceObjects.PriceInputError).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPriceObjects.UnitDescriptionInputError).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPriceObjects.RangeDefinitionInputError).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPriceObjects.PublicationStatusInputError).Should().BeTrue();

            CommonActions.ClickRadioButtonWithText("Declarative");
            CommonActions.ClickSave();
            CommonActions.ElementIsDisplayed(ListPriceObjects.DeclarativeQuantityCalculationError).Should().BeTrue();

            CommonActions.ClickRadioButtonWithText("On demand");
            CommonActions.ClickSave();
            CommonActions.ElementIsDisplayed(ListPriceObjects.OnDemandQuantityCalculationError).Should().BeTrue();
        }

        [Fact]
        public void Submit_Input_NavigatesToCorrectPage()
        {
            CommonActions.ClickRadioButtonWithValue(ProvisioningType.Patient.ToString());
            CommonActions.ClickRadioButtonWithValue(PublicationStatus.Published.ToString());
            CommonActions.ClickRadioButtonWithValue(CataloguePriceCalculationType.SingleFixed.ToString());

            TextGenerators.TextInputAddText(ListPriceObjects.UnitDescriptionInput, 100);
            TextGenerators.TextInputAddText(ListPriceObjects.RangeDefinitionInput, 100);
            CommonActions.ElementAddValue(ListPriceObjects.PriceInput, "3.14");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "Index").Should().BeTrue();
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
