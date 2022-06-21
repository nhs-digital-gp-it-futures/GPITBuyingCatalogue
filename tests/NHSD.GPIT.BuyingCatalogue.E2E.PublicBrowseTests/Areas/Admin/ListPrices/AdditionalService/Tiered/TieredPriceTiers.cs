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

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices.AdditionalService.Tiered
{
    public sealed class TieredPriceTiers : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
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

        public TieredPriceTiers(LocalWebApplicationFactory factory)
            : base(
                factory,
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.TieredPriceTiers),
                Parameters)
        {
        }

        [Fact]
        public void AllSectionsDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.First(c => c.Id == AdditionalServiceId);

            CommonActions.PageTitle().Should().Be($"Tiered list price information - {catalogueItem.Name}".FormatForComparison());
            CommonActions.LedeText().Should().Be("Provide information about the pricing tiers available for your Additional Service.".FormatForComparison());

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(TieredPriceTiersObjects.TiersTable).Should().BeTrue();
            CommonActions.ElementIsDisplayed(TieredPriceTiersObjects.PublicationStatusInput).Should().BeTrue();
        }

        [Fact]
        public void ClickGoBackLink_NavigatesCorrectly()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.AddTieredListPrice)).Should().BeTrue();
        }

        [Fact]
        public void DeletePrice_Redirects()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == AdditionalServiceId);

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
                { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
                { nameof(CataloguePriceId), price.CataloguePriceId.ToString() },
            };

            NavigateToUrl(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.TieredPriceTiers),
                parameters);

            CommonActions.ClickGoBackLink();

            CommonActions.ElementIsDisplayed(ListPriceObjects.DeletePriceLink).Should().BeTrue();

            CommonActions.ClickLinkElement(ListPriceObjects.DeletePriceLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.DeleteListPrice)).Should().BeTrue();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.Index)).Should().BeTrue();
        }

        [Fact]
        public void DeletePrice_BackLink()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == AdditionalServiceId);

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
                { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
                { nameof(CataloguePriceId), price.CataloguePriceId.ToString() },
            };

            NavigateToUrl(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.TieredPriceTiers),
                parameters);

            CommonActions.ClickGoBackLink();

            CommonActions.ElementIsDisplayed(ListPriceObjects.DeletePriceLink).Should().BeTrue();

            CommonActions.ClickLinkElement(ListPriceObjects.DeletePriceLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.DeleteListPrice)).Should().BeTrue();

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.AddTieredListPrice)).Should().BeTrue();

            catalogueItem.CataloguePrices.Remove(price);
            context.SaveChanges();
        }

        [Fact]
        public void DeleteTier_Redirects()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == AdditionalServiceId);

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
                { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
                { nameof(CataloguePriceId), price.CataloguePriceId.ToString() },
            };

            NavigateToUrl(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.TieredPriceTiers),
                parameters);

            CommonActions.ClickLinkElement(ListPriceObjects.EditTierPriceLink(1));

            CommonActions.ClickLinkElement(ListPriceObjects.DeleteTieredPriceTierLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.DeleteTieredPriceTier)).Should().BeTrue();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.TieredPriceTiers)).Should().BeTrue();
        }

        [Fact]
        public void DeleteTier_BackLink()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == AdditionalServiceId);

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
                { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
                { nameof(CataloguePriceId), price.CataloguePriceId.ToString() },
            };

            NavigateToUrl(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.TieredPriceTiers),
                parameters);

            CommonActions.ClickLinkElement(ListPriceObjects.EditTierPriceLink(1));

            CommonActions.ClickLinkElement(ListPriceObjects.DeleteTieredPriceTierLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.DeleteTieredPriceTier)).Should().BeTrue();

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.EditTieredPriceTier)).Should().BeTrue();

            catalogueItem.CataloguePrices.Remove(price);
            context.SaveChanges();
        }

        [Fact]
        public void ClickAddTierLink_NavigatesCorrectly()
        {
            CommonActions.ClickLinkElement(TieredPriceTiersObjects.AddTierLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.AddTieredPriceTier)).Should().BeTrue();
        }

        [Fact]
        public void ClickSubmit_Draft_NavigatesCorrectly()
        {
            CommonActions.ClickRadioButtonWithValue(PublicationStatus.Draft.ToString());

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.Index)).Should().BeTrue();
        }

        [Fact]
        public void ClickSubmit_Publish_NavigatesCorrectly()
        {
            CommonActions.ClickRadioButtonWithValue(PublicationStatus.Published.ToString());

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.Index)).Should().BeTrue();
        }

        [Fact]
        public void NoTiers_TableNotDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == AdditionalServiceId);

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
                { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
                { nameof(CataloguePriceId), price.CataloguePriceId.ToString() },
            };

            NavigateToUrl(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.TieredPriceTiers),
                parameters);

            CommonActions.ElementIsDisplayed(TieredPriceTiersObjects.TiersTable).Should().BeFalse();

            catalogueItem.CataloguePrices.Remove(price);
            context.SaveChanges();
        }

        [Fact]
        public void Publish_NoTiers_ThrowsError()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == AdditionalServiceId);

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
                { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
                { nameof(CataloguePriceId), price.CataloguePriceId.ToString() },
            };

            NavigateToUrl(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.TieredPriceTiers),
                parameters);

            CommonActions.ClickRadioButtonWithValue(PublicationStatus.Published.ToString());
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(TieredPriceTiersObjects.PublicationStatusInputError, "Error: Add at least 1 tier").Should().BeTrue();

            catalogueItem.CataloguePrices.Remove(price);
            context.SaveChanges();
        }

        [Fact]
        public void Publish_InvalidStartingRange_ThrowsError()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == AdditionalServiceId);

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
                { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
                { nameof(CataloguePriceId), price.CataloguePriceId.ToString() },
            };

            NavigateToUrl(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.TieredPriceTiers),
                parameters);

            CommonActions.ClickRadioButtonWithValue(PublicationStatus.Published.ToString());
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(TieredPriceTiersObjects.PublicationStatusInputError, "Error: Lowest tier must have a low range of 1").Should().BeTrue();

            catalogueItem.CataloguePrices.Remove(price);
            context.SaveChanges();
        }

        [Fact]
        public void Publish_NoInfiniteRange_ThrowsError()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == AdditionalServiceId);

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
                { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
                { nameof(CataloguePriceId), price.CataloguePriceId.ToString() },
            };

            NavigateToUrl(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.TieredPriceTiers),
                parameters);

            CommonActions.ClickRadioButtonWithValue(PublicationStatus.Published.ToString());
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(TieredPriceTiersObjects.PublicationStatusInputError, "Error: Highest tier must have an infinite upper range").Should().BeTrue();

            catalogueItem.CataloguePrices.Remove(price);
            context.SaveChanges();
        }

        [Fact]
        public void Publish_OverlappingTiers_ThrowsError()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == AdditionalServiceId);

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
                { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
                { nameof(CataloguePriceId), price.CataloguePriceId.ToString() },
            };

            NavigateToUrl(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.TieredPriceTiers),
                parameters);

            CommonActions.ClickRadioButtonWithValue(PublicationStatus.Published.ToString());
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(ListPriceObjects.PublicationStatusInputError, "Error: Tier 1's upper range overlaps with Tier 2's lower range").Should().BeTrue();

            catalogueItem.CataloguePrices.Remove(price);
            context.SaveChanges();
        }

        [Fact]
        public void Publish_GapsBetweenTiers_ThrowsError()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == AdditionalServiceId);

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
                { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
                { nameof(CataloguePriceId), price.CataloguePriceId.ToString() },
            };

            NavigateToUrl(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.TieredPriceTiers),
                parameters);

            CommonActions.ClickRadioButtonWithValue(PublicationStatus.Published.ToString());
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(ListPriceObjects.PublicationStatusInputError, "Error: There's a gap between Tier 1's upper range and Tier 2's lower range").Should().BeTrue();

            catalogueItem.CataloguePrices.Remove(price);
            context.SaveChanges();
        }
    }
}
