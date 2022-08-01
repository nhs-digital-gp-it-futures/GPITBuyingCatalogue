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

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices.Base.Tiered
{
    public abstract class TieredPriceTiersBase : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private readonly IDictionary<string, string> parameters;

        protected TieredPriceTiersBase(
            LocalWebApplicationFactory factory,
            Type controller,
            IDictionary<string, string> parameters)
            : base(
                factory,
                controller,
                "TieredPriceTiers",
                parameters)
        {
            this.parameters = parameters;
        }

        protected abstract CatalogueItemId CatalogueItemId { get; }

        protected abstract int CataloguePriceId { get; }

        protected abstract Type Controller { get; }

        [Fact]
        public void AllSectionsDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.First(context => context.Id == CatalogueItemId);

            CommonActions.PageTitle().Should().Be($"Tiered list price information - {catalogueItem.Name}".FormatForComparison());
            CommonActions.LedeText().Should().Be($"Provide information about the pricing tiers available for your {catalogueItem.CatalogueItemType.Name()}.".FormatForComparison());

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
                Controller,
                "AddTieredListPrice").Should().BeTrue();
        }

        [Fact]
        public void DeletePrice_Redirects()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == CatalogueItemId);

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

            var updatedParameters = new Dictionary<string, string>(parameters)
            {
                [nameof(CataloguePriceId)] = price.CataloguePriceId.ToString(),
            };

            NavigateToUrl(
                Controller,
                "TieredPriceTiers",
                updatedParameters);

            CommonActions.ClickGoBackLink();

            CommonActions.ElementIsDisplayed(ListPriceObjects.DeletePriceLink).Should().BeTrue();

            CommonActions.ClickLinkElement(ListPriceObjects.DeletePriceLink);

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "DeleteListPrice").Should().BeTrue();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "Index").Should().BeTrue();
        }

        [Fact]
        public void DeletePrice_BackLink()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == CatalogueItemId);

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

            var updatedParameters = new Dictionary<string, string>(parameters)
            {
                [nameof(CataloguePriceId)] = price.CataloguePriceId.ToString(),
            };

            NavigateToUrl(
                Controller,
                "TieredPriceTiers",
                updatedParameters);

            CommonActions.ClickGoBackLink();

            CommonActions.ElementIsDisplayed(ListPriceObjects.DeletePriceLink).Should().BeTrue();

            CommonActions.ClickLinkElement(ListPriceObjects.DeletePriceLink);

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "DeleteListPrice").Should().BeTrue();

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "AddTieredListPrice").Should().BeTrue();

            catalogueItem.CataloguePrices.Remove(price);
            context.SaveChanges();
        }

        [Fact]
        public void DeleteTier_Redirects()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == CatalogueItemId);

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

            var updatedParameters = new Dictionary<string, string>(parameters)
            {
                [nameof(CataloguePriceId)] = price.CataloguePriceId.ToString(),
            };

            NavigateToUrl(
                Controller,
                "TieredPriceTiers",
                updatedParameters);

            CommonActions.ClickLinkElement(ListPriceObjects.EditTierPriceLink(1));

            CommonActions.ClickLinkElement(ListPriceObjects.DeleteTieredPriceTierLink);

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "DeleteTieredPriceTier").Should().BeTrue();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "TieredPriceTiers").Should().BeTrue();
        }

        [Fact]
        public void DeleteTier_BackLink()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == CatalogueItemId);

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

            var updatedParameters = new Dictionary<string, string>(parameters)
            {
                [nameof(CataloguePriceId)] = price.CataloguePriceId.ToString(),
            };

            NavigateToUrl(
                Controller,
                "TieredPriceTiers",
                updatedParameters);

            CommonActions.ClickLinkElement(ListPriceObjects.EditTierPriceLink(1));

            CommonActions.ClickLinkElement(ListPriceObjects.DeleteTieredPriceTierLink);

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "DeleteTieredPriceTier").Should().BeTrue();

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "EditTieredPriceTier").Should().BeTrue();

            catalogueItem.CataloguePrices.Remove(price);
            context.SaveChanges();
        }

        [Fact]
        public void ClickAddTierLink_NavigatesCorrectly()
        {
            CommonActions.ClickLinkElement(TieredPriceTiersObjects.AddTierLink);

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "AddTieredPriceTier").Should().BeTrue();
        }

        [Fact]
        public void ClickSubmit_Draft_NavigatesCorrectly()
        {
            CommonActions.ClickRadioButtonWithValue(PublicationStatus.Draft.ToString());

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "Index").Should().BeTrue();
        }

        [Fact]
        public void ClickSubmit_Publish_NavigatesCorrectly()
        {
            CommonActions.ClickRadioButtonWithValue(PublicationStatus.Published.ToString());

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "Index").Should().BeTrue();
        }

        [Fact]
        public void NoTiers_TableNotDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == CatalogueItemId);

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

            var updatedParameters = new Dictionary<string, string>(parameters)
            {
                [nameof(CataloguePriceId)] = price.CataloguePriceId.ToString(),
            };

            NavigateToUrl(
                Controller,
                "TieredPriceTiers",
                updatedParameters);

            CommonActions.ElementIsDisplayed(TieredPriceTiersObjects.TiersTable).Should().BeFalse();

            catalogueItem.CataloguePrices.Remove(price);
            context.SaveChanges();
        }
    }
}
