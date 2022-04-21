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

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices.Solution.Flat
{
    public sealed class EditFlatListPrice : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new CatalogueItemId(99998, "001");
        private static readonly int CataloguePriceId = 1;

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(CataloguePriceId), CataloguePriceId.ToString() },
        };

        public EditFlatListPrice(LocalWebApplicationFactory factory)
            : base(
                factory,
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.EditFlatListPrice),
                Parameters)
        {
        }

        [Fact]
        public void AllSectionsDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Single(c => c.Id == SolutionId);

            CommonActions.PageTitle().Should().Be($"Edit a flat list price - {catalogueItem.Name}".FormatForComparison());
            CommonActions.LedeText().Should().Be("Provide the following information about the pricing model for your Catalogue Solution.".FormatForComparison());

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(AddEditFlatListPriceObjects.ProvisioningTypeInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddEditFlatListPriceObjects.CalculationTypeInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddEditFlatListPriceObjects.UnitDescriptionInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddEditFlatListPriceObjects.UnitDefinitionInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddEditFlatListPriceObjects.PriceInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddEditFlatListPriceObjects.PublicationStatusInput).Should().BeTrue();

            CommonActions.ElementIsDisplayed(AddEditFlatListPriceObjects.OnDemandBillingPeriodInput).Should().BeFalse();
            CommonActions.ElementIsDisplayed(AddEditFlatListPriceObjects.DeclarativeBillingPeriodInput).Should().BeFalse();

            CommonActions.ElementIsDisplayed(AddEditFlatListPriceObjects.DeletePriceLink).Should().BeTrue();

            CommonActions.ClickRadioButtonWithValue(ProvisioningType.OnDemand.ToString());
            CommonActions.ElementIsDisplayed(AddEditFlatListPriceObjects.OnDemandBillingPeriodInput).Should().BeTrue();

            CommonActions.ClickRadioButtonWithValue(ProvisioningType.Declarative.ToString());
            CommonActions.ElementIsDisplayed(AddEditFlatListPriceObjects.DeclarativeBillingPeriodInput).Should().BeTrue();
        }

        [Fact]
        public void ClickGoBackLink_NavigatesToCorrectPage()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.Index)).Should().BeTrue();
        }

        [Fact]
        public void ClickDelete_NavigatesToCorrectPage()
        {
            CommonActions.ClickLinkElement(AddEditFlatListPriceObjects.DeletePriceLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.DeleteListPrice)).Should().BeTrue();
        }

        [Fact]
        public void Submit_Duplicate_ThrowsError()
        {
            var catalogueItem = GetCatalogueItemWithPrices(SolutionId);
            var price = catalogueItem.CataloguePrices.First(p => p.CataloguePriceType == CataloguePriceType.Flat && p.CataloguePriceId != CataloguePriceId);

            CommonActions.ClickRadioButtonWithValue(price.ProvisioningType.ToString());
            CommonActions.ClickRadioButtonWithValue(price.CataloguePriceCalculationType.ToString());
            CommonActions.ClickRadioButtonWithValue(price.PublishedStatus.ToString());

            CommonActions.ElementAddValue(AddEditFlatListPriceObjects.UnitDescriptionInput, price.PricingUnit.Description);
            CommonActions.ElementAddValue(AddEditFlatListPriceObjects.PriceInput, price.CataloguePriceTiers.First().Price.ToString());

            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(AddEditFlatListPriceObjects.ProvisioningTypeInputError, "Error: A list price with these details already exists").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddEditFlatListPriceObjects.CalculationTypeInputError, "Error: A list price with these details already exists").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddEditFlatListPriceObjects.UnitDescriptionInputError, "A list price with these details already exists").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddEditFlatListPriceObjects.PriceInputError, "A list price with these details already exists").Should().BeTrue();
        }

        [Fact]
        public void Submit_Input_NavigatesToCorrectPage()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.Index)).Should().BeTrue();
        }

        [Fact]
        public void DeletePrice_Redirects()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == SolutionId);

            var price = new CataloguePrice
            {
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Flat,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                PublishedStatus = PublicationStatus.Draft,
                PricingUnit = new PricingUnit
                {
                    RangeDescription = "patients test",
                    Description = "per flat single fixed test patient",
                },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                CataloguePriceTiers = new HashSet<CataloguePriceTier>
                {
                    new()
                    {
                        Price = 3.14M,
                        LowerRange = 1,
                        UpperRange = null,
                    },
                },
            };

            catalogueItem.CataloguePrices.Add(price);
            context.SaveChanges();

            var parameters = new Dictionary<string, string>
            {
                { nameof(SolutionId), SolutionId.ToString() },
                { nameof(CataloguePriceId), price.CataloguePriceId.ToString() },
            };

            NavigateToUrl(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.EditFlatListPrice),
                parameters);

            CommonActions.ElementIsDisplayed(AddEditFlatListPriceObjects.DeletePriceLink).Should().BeTrue();

            CommonActions.ClickLinkElement(AddEditFlatListPriceObjects.DeletePriceLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.DeleteListPrice)).Should().BeTrue();

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
