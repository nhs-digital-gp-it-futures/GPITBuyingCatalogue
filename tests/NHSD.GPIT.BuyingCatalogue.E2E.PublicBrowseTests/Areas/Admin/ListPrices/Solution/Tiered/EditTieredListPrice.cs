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

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices.Solution.Tiered
{
    public sealed class EditTieredListPrice : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new CatalogueItemId(99998, "001");
        private static readonly int CataloguePriceId = 4;

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(CataloguePriceId), CataloguePriceId.ToString() },
        };

        public EditTieredListPrice(LocalWebApplicationFactory factory)
            : base(
                factory,
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.EditTieredListPrice),
                Parameters)
        {
        }

        [Fact]
        public void AllSectionsDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(p => p.CataloguePrices).Single(c => c.Id == SolutionId);
            var price = catalogueItem.CataloguePrices.First(p => p.CataloguePriceId == CataloguePriceId);
            var publishStatus = price.PublishedStatus;

            CommonActions.PageTitle().Should().Be($"Edit a tiered list price - {catalogueItem.Name}".FormatForComparison());
            CommonActions.LedeText().Should().Be("Provide the following information about the pricing model for your Catalogue Solution.".FormatForComparison());

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(AddEditTieredListPriceObjects.ProvisioningTypeInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddEditTieredListPriceObjects.CalculationTypeInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddEditTieredListPriceObjects.UnitDescriptionInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddEditTieredListPriceObjects.UnitDefinitionInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddEditTieredListPriceObjects.RangeDefinitionInput).Should().BeTrue();

            CommonActions.ElementIsDisplayed(AddEditTieredListPriceObjects.OnDemandBillingPeriodInput).Should().BeFalse();
            CommonActions.ElementIsDisplayed(AddEditTieredListPriceObjects.DeclarativeBillingPeriodInput).Should().BeFalse();

            CommonActions.ClickRadioButtonWithValue(ProvisioningType.OnDemand.ToString());
            CommonActions.ElementIsDisplayed(AddEditTieredListPriceObjects.OnDemandBillingPeriodInput).Should().BeTrue();

            CommonActions.ClickRadioButtonWithValue(ProvisioningType.Declarative.ToString());
            CommonActions.ElementIsDisplayed(AddEditTieredListPriceObjects.DeclarativeBillingPeriodInput).Should().BeTrue();

            CommonActions.ElementIsDisplayed(AddEditTieredListPriceObjects.TieredPriceTable).Should().BeTrue();

            CommonActions.ElementIsDisplayed(AddEditTieredListPriceObjects.PublicationStatusInput).Should().BeTrue();

            CommonActions.ElementIsDisplayed(AddEditTieredListPriceObjects.PublishedInsetSection).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddEditTieredListPriceObjects.AddTierLink).Should().BeFalse();

            price.PublishedStatus = PublicationStatus.Unpublished;
            context.SaveChanges();
            Driver.Navigate().Refresh();

            CommonActions.ElementIsDisplayed(AddEditTieredListPriceObjects.PublishedInsetSection).Should().BeFalse();
            CommonActions.ElementIsDisplayed(AddEditTieredListPriceObjects.AddTierLink).Should().BeTrue();

            price.PublishedStatus = publishStatus;
            context.SaveChanges();
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
        public void ClickAddPriceTier_NavigatesToCorrectPage()
        {
            using var context = GetEndToEndDbContext();
            var price = context.CataloguePrices.Single(p => p.CatalogueItemId == SolutionId && p.CataloguePriceId == CataloguePriceId);
            var publishStatus = price.PublishedStatus;

            price.PublishedStatus = PublicationStatus.Unpublished;
            context.SaveChanges();

            Driver.Navigate().Refresh();

            CommonActions.ClickLinkElement(AddEditTieredListPriceObjects.AddTierLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.AddTieredPriceTier));

            price.PublishedStatus = publishStatus;
            context.SaveChanges();
        }

        [Fact]
        public void Submit_Duplicate_ThrowsError()
        {
            var catalogueItem = GetCatalogueItemWithPrices(SolutionId);
            var price = catalogueItem.CataloguePrices.First(p => p.CataloguePriceId != CataloguePriceId && p.CataloguePriceType == CataloguePriceType.Tiered);

            CommonActions.ClickRadioButtonWithValue(price.ProvisioningType.ToString());
            CommonActions.ClickRadioButtonWithValue(price.CataloguePriceCalculationType.ToString());

            CommonActions.ElementAddValue(AddEditTieredListPriceObjects.UnitDescriptionInput, price.PricingUnit.Description);
            CommonActions.ElementAddValue(AddEditTieredListPriceObjects.RangeDefinitionInput, price.PricingUnit.RangeDescription);

            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(AddEditTieredListPriceObjects.ProvisioningTypeInputError, "Error: A list price with these details already exists").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddEditTieredListPriceObjects.CalculationTypeInputError, "Error: A list price with these details already exists").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddEditTieredListPriceObjects.UnitDescriptionInputError, "A list price with these details already exists").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddEditTieredListPriceObjects.RangeDefinitionInputError, "A list price with these details already exists").Should().BeTrue();
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
        public void NoTiers_TableNotDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == SolutionId);

            var price = new CataloguePrice
            {
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Tiered,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                PublishedStatus = PublicationStatus.Draft,
                PricingUnit = new PricingUnit
                {
                    RangeDescription = "patients test",
                    Description = "per tiered single fixed test patient",
                },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
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
                nameof(CatalogueSolutionListPriceController.EditTieredListPrice),
                parameters);

            CommonActions.ElementIsDisplayed(AddEditTieredListPriceObjects.TieredPriceTable).Should().BeFalse();

            catalogueItem.CataloguePrices.Remove(price);
            context.SaveChanges();
        }

        [Fact]
        public void Publish_NoTiers_ThrowsError()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == SolutionId);

            var price = new CataloguePrice
            {
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Tiered,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                PublishedStatus = PublicationStatus.Draft,
                PricingUnit = new PricingUnit
                {
                    RangeDescription = "patients test",
                    Description = "per tiered single fixed test patient",
                },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
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
                nameof(CatalogueSolutionListPriceController.EditTieredListPrice),
                parameters);

            CommonActions.ClickRadioButtonWithValue(PublicationStatus.Published.ToString());
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddEditTieredListPriceObjects.PublicationStatusInputError, "Error: Add at least 1 tier").Should().BeTrue();

            catalogueItem.CataloguePrices.Remove(price);
            context.SaveChanges();
        }

        [Fact]
        public void Publish_InvalidStartingRange_ThrowsError()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == SolutionId);

            var price = new CataloguePrice
            {
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Tiered,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                PublishedStatus = PublicationStatus.Draft,
                PricingUnit = new PricingUnit
                {
                    RangeDescription = "patients test",
                    Description = "per tiered single fixed test patient",
                },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                CataloguePriceTiers = new HashSet<CataloguePriceTier>
                {
                    new()
                    {
                        Price = 3.14M,
                        LowerRange = 2,
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
                nameof(CatalogueSolutionListPriceController.EditTieredListPrice),
                parameters);

            CommonActions.ClickRadioButtonWithValue(PublicationStatus.Published.ToString());
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddEditTieredListPriceObjects.PublicationStatusInputError, "Error: Lowest tier must have a low range of 1").Should().BeTrue();

            catalogueItem.CataloguePrices.Remove(price);
            context.SaveChanges();
        }

        [Fact]
        public void Publish_NoInfiniteRange_ThrowsError()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == SolutionId);

            var price = new CataloguePrice
            {
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Tiered,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                PublishedStatus = PublicationStatus.Draft,
                PricingUnit = new PricingUnit
                {
                    RangeDescription = "patients test",
                    Description = "per tiered single fixed test patient",
                },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                CataloguePriceTiers = new HashSet<CataloguePriceTier>
                {
                    new()
                    {
                        Price = 3.14M,
                        LowerRange = 1,
                        UpperRange = 9,
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
                nameof(CatalogueSolutionListPriceController.EditTieredListPrice),
                parameters);

            CommonActions.ClickRadioButtonWithValue(PublicationStatus.Published.ToString());
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddEditTieredListPriceObjects.PublicationStatusInputError, "Error: Highest tier must have an infinite upper range").Should().BeTrue();

            catalogueItem.CataloguePrices.Remove(price);
            context.SaveChanges();
        }

        [Fact]
        public void Publish_OverlappingTiers_ThrowsError()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == SolutionId);

            var price = new CataloguePrice
            {
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Tiered,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                PublishedStatus = PublicationStatus.Draft,
                PricingUnit = new PricingUnit
                {
                    RangeDescription = "patients test",
                    Description = "per tiered single fixed test patient",
                },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                CataloguePriceTiers = new HashSet<CataloguePriceTier>
                {
                    new()
                    {
                        Price = 3.14M,
                        LowerRange = 1,
                        UpperRange = 9,
                    },
                    new()
                    {
                        Price = 3M,
                        LowerRange = 9,
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
                nameof(CatalogueSolutionListPriceController.EditTieredListPrice),
                parameters);

            CommonActions.ClickRadioButtonWithValue(PublicationStatus.Published.ToString());
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddEditTieredListPriceObjects.PublicationStatusInputError, "Error: Tier 1's lower range overlaps with Tier 2's upper range").Should().BeTrue();

            catalogueItem.CataloguePrices.Remove(price);
            context.SaveChanges();
        }

        [Fact]
        public void Publish_GapsBetweenTiers_ThrowsError()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == SolutionId);

            var price = new CataloguePrice
            {
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Tiered,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                PublishedStatus = PublicationStatus.Draft,
                PricingUnit = new PricingUnit
                {
                    RangeDescription = "patients test",
                    Description = "per tiered single fixed test patient",
                },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                CataloguePriceTiers = new HashSet<CataloguePriceTier>
                {
                    new()
                    {
                        Price = 3.14M,
                        LowerRange = 1,
                        UpperRange = 9,
                    },
                    new()
                    {
                        Price = 3M,
                        LowerRange = 11,
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
                nameof(CatalogueSolutionListPriceController.EditTieredListPrice),
                parameters);

            CommonActions.ClickRadioButtonWithValue(PublicationStatus.Published.ToString());
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddEditTieredListPriceObjects.PublicationStatusInputError, "Error: There's a gap between Tier 1's upper range and Tier 2's lower range").Should().BeTrue();

            catalogueItem.CataloguePrices.Remove(price);
            context.SaveChanges();
        }

        [Fact]
        public void Published_EditPrice_NavigatesToCorrectPage()
        {
            using var context = GetEndToEndDbContext();
            var price = context.CataloguePrices.Single(p => p.CatalogueItemId == SolutionId && p.CataloguePriceId == CataloguePriceId);
            var publishStatus = price.PublishedStatus;

            price.PublishedStatus = PublicationStatus.Published;
            context.SaveChanges();

            Driver.Navigate().Refresh();

            CommonActions.ClickLinkElement(AddEditTieredListPriceObjects.EditTierPriceLink(1));

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.EditTierPrice)).Should().BeTrue();

            price.PublishedStatus = publishStatus;
            context.SaveChanges();
        }

        [Fact]
        public void Unpublished_EditPrice_NavigatesToCorrectPage()
        {
            using var context = GetEndToEndDbContext();
            var price = context.CataloguePrices.Single(p => p.CatalogueItemId == SolutionId && p.CataloguePriceId == CataloguePriceId);
            var publishStatus = price.PublishedStatus;

            price.PublishedStatus = PublicationStatus.Unpublished;
            context.SaveChanges();

            Driver.Navigate().Refresh();

            CommonActions.ClickLinkElement(AddEditTieredListPriceObjects.EditTierPriceLink(1));

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.EditTieredPriceTier)).Should().BeTrue();

            price.PublishedStatus = publishStatus;
            context.SaveChanges();
        }

        [Fact]
        public void DeletePrice_Redirects()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == SolutionId);

            var price = new CataloguePrice
            {
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Tiered,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                PublishedStatus = PublicationStatus.Draft,
                PricingUnit = new PricingUnit
                {
                    RangeDescription = "patients test",
                    Description = "per tiered single fixed test patient",
                },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                CataloguePriceTiers = new HashSet<CataloguePriceTier>
                {
                    new()
                    {
                        Price = 3.14M,
                        LowerRange = 1,
                        UpperRange = 9,
                    },
                    new()
                    {
                        Price = 3M,
                        LowerRange = 10,
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
                nameof(CatalogueSolutionListPriceController.EditTieredListPrice),
                parameters);

            CommonActions.ElementIsDisplayed(AddEditTieredListPriceObjects.DeletePriceLink).Should().BeTrue();

            CommonActions.ClickLinkElement(AddEditTieredListPriceObjects.DeletePriceLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.DeleteListPrice)).Should().BeTrue();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.Index)).Should().BeTrue();
        }

        [Fact]
        public void DeletePrice_BackLink()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == SolutionId);

            var price = new CataloguePrice
            {
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Tiered,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                PublishedStatus = PublicationStatus.Draft,
                PricingUnit = new PricingUnit
                {
                    RangeDescription = "patients test",
                    Description = "per tiered single fixed test patient",
                },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                CataloguePriceTiers = new HashSet<CataloguePriceTier>
                {
                    new()
                    {
                        Price = 3.14M,
                        LowerRange = 1,
                        UpperRange = 9,
                    },
                    new()
                    {
                        Price = 3M,
                        LowerRange = 10,
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
                nameof(CatalogueSolutionListPriceController.EditTieredListPrice),
                parameters);

            CommonActions.ElementIsDisplayed(AddEditTieredListPriceObjects.DeletePriceLink).Should().BeTrue();

            CommonActions.ClickLinkElement(AddEditTieredListPriceObjects.DeletePriceLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.DeleteListPrice)).Should().BeTrue();

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.EditTieredListPrice)).Should().BeTrue();

            catalogueItem.CataloguePrices.Remove(price);
            context.SaveChanges();
        }

        [Fact]
        public void DeleteTier_Redirects()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == SolutionId);

            var price = new CataloguePrice
            {
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Tiered,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                PublishedStatus = PublicationStatus.Draft,
                PricingUnit = new PricingUnit
                {
                    RangeDescription = "patients test",
                    Description = "per tiered single fixed test patient",
                },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                CataloguePriceTiers = new HashSet<CataloguePriceTier>
                {
                    new()
                    {
                        Price = 3.14M,
                        LowerRange = 1,
                        UpperRange = 9,
                    },
                    new()
                    {
                        Price = 3M,
                        LowerRange = 10,
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
                nameof(CatalogueSolutionListPriceController.EditTieredListPrice),
                parameters);

            CommonActions.ClickLinkElement(AddEditTieredListPriceObjects.EditTierPriceLink(1));

            CommonActions.ClickLinkElement(AddEditTieredListPriceObjects.DeleteTieredPriceTierLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.DeleteTieredPriceTier)).Should().BeTrue();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.EditTieredListPrice)).Should().BeTrue();
        }

        [Fact]
        public void DeleteTier_BackLink()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == SolutionId);

            var price = new CataloguePrice
            {
                ProvisioningType = ProvisioningType.Patient,
                CataloguePriceType = CataloguePriceType.Tiered,
                CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
                PublishedStatus = PublicationStatus.Draft,
                PricingUnit = new PricingUnit
                {
                    RangeDescription = "patients test",
                    Description = "per tiered single fixed test patient",
                },
                TimeUnit = TimeUnit.PerYear,
                CurrencyCode = "GBP",
                CataloguePriceTiers = new HashSet<CataloguePriceTier>
                {
                    new()
                    {
                        Price = 3.14M,
                        LowerRange = 1,
                        UpperRange = 9,
                    },
                    new()
                    {
                        Price = 3M,
                        LowerRange = 10,
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
                nameof(CatalogueSolutionListPriceController.EditTieredListPrice),
                parameters);

            CommonActions.ClickLinkElement(AddEditTieredListPriceObjects.EditTierPriceLink(1));

            CommonActions.ClickLinkElement(AddEditTieredListPriceObjects.DeleteTieredPriceTierLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.DeleteTieredPriceTier)).Should().BeTrue();

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.EditTieredPriceTier)).Should().BeTrue();

            catalogueItem.CataloguePrices.Remove(price);
            context.SaveChanges();
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
