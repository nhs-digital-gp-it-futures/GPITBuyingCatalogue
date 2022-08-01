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
    public abstract class EditTieredListPriceBase : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private readonly IDictionary<string, string> parameters;

        protected EditTieredListPriceBase(
            LocalWebApplicationFactory factory,
            Type controller,
            IDictionary<string, string> parameters)
            : base(
                factory,
                controller,
                "EditTieredListPrice",
                parameters)
        {
            this.parameters = parameters;
        }

        protected abstract CatalogueItemId CatalogueItemId { get; }

        protected abstract Type Controller { get; }

        protected abstract int CataloguePriceId { get; }

        protected abstract int MaximumTiersPriceId { get; }

        [Fact]
        public void AllSectionsDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(p => p.CataloguePrices).Single(c => c.Id == CatalogueItemId);
            var price = catalogueItem.CataloguePrices.First(p => p.CataloguePriceId == CataloguePriceId);
            var publishStatus = price.PublishedStatus;

            price.PublishedStatus = PublicationStatus.Published;
            context.SaveChanges();
            Driver.Navigate().Refresh();

            CommonActions.PageTitle().Should().Be($"Edit a tiered list price - {catalogueItem.Name}".FormatForComparison());
            CommonActions.LedeText().Should().Be($"Provide the following information about the pricing model for your {catalogueItem.CatalogueItemType.Name()}.".FormatForComparison());

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(ListPriceObjects.ProvisioningTypeInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPriceObjects.CalculationTypeInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPriceObjects.UnitDescriptionInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPriceObjects.UnitDefinitionInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPriceObjects.RangeDefinitionInput).Should().BeTrue();
            CommonActions.ElementExists(ListPriceObjects.MaximumTiersInset).Should().BeFalse();

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

            CommonActions.ElementIsDisplayed(ListPriceObjects.TieredPriceTable).Should().BeTrue();

            CommonActions.ElementIsDisplayed(ListPriceObjects.PublicationStatusInput).Should().BeTrue();

            CommonActions.ElementIsDisplayed(ListPriceObjects.PublishedInsetSection).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPriceObjects.AddTierLink).Should().BeFalse();
            CommonActions.ElementIsDisplayed(ListPriceObjects.DeletePriceLink).Should().BeFalse();

            price.PublishedStatus = PublicationStatus.Unpublished;
            context.SaveChanges();
            Driver.Navigate().Refresh();

            CommonActions.ElementIsDisplayed(ListPriceObjects.PublishedInsetSection).Should().BeFalse();
            CommonActions.ElementIsDisplayed(ListPriceObjects.AddTierLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPriceObjects.DeletePriceLink).Should().BeTrue();

            price.PublishedStatus = publishStatus;
            context.SaveChanges();
        }

        [Fact]
        public void MaximumTiersReached_CorrectInsetDisplayed()
        {
            var maximumTiersParameters = new Dictionary<string, string>(parameters)
            {
                [nameof(CataloguePriceId)] = MaximumTiersPriceId.ToString(),
            };

            NavigateToUrl(Controller, "EditTieredListPrice", maximumTiersParameters);

            CommonActions.ElementIsDisplayed(ListPriceObjects.AddTierLink).Should().BeFalse();
            CommonActions.ElementIsDisplayed(ListPriceObjects.MaximumTiersInset).Should().BeTrue();
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
        public void ClickAddPriceTier_NavigatesToCorrectPage()
        {
            using var context = GetEndToEndDbContext();
            var price = context.CataloguePrices.Single(p => p.CatalogueItemId == CatalogueItemId && p.CataloguePriceId == CataloguePriceId);
            var publishStatus = price.PublishedStatus;

            price.PublishedStatus = PublicationStatus.Unpublished;
            context.SaveChanges();

            Driver.Navigate().Refresh();

            CommonActions.ClickLinkElement(ListPriceObjects.AddTierLink);

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "AddTieredPriceTier");

            price.PublishedStatus = publishStatus;
            context.SaveChanges();
        }

        [Fact]
        public void Submit_Input_NavigatesToCorrectPage()
        {
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
                "EditTieredListPrice",
                updatedParameters);

            CommonActions.ElementIsDisplayed(ListPriceObjects.TieredPriceTable).Should().BeFalse();

            catalogueItem.CataloguePrices.Remove(price);
            context.SaveChanges();
        }

        [Fact]
        public void Publish_InvalidModelState_ThrowsError()
        {
            CommonActions.ClickRadioButtonWithValue(PublicationStatus.Published.ToString());
            CommonActions.ClearInputElement(ListPriceObjects.UnitDescriptionInput);
            CommonActions.ClearInputElement(ListPriceObjects.RangeDefinitionInput);

            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(ListPriceObjects.UnitDescriptionInputError).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPriceObjects.RangeDefinitionInputError).Should().BeTrue();
        }

        [Fact]
        public void Published_EditPrice_NavigatesToCorrectPage()
        {
            using var context = GetEndToEndDbContext();
            var price = context.CataloguePrices.Single(p => p.CatalogueItemId == CatalogueItemId && p.CataloguePriceId == CataloguePriceId);
            var publishStatus = price.PublishedStatus;

            price.PublishedStatus = PublicationStatus.Published;
            context.SaveChanges();

            Driver.Navigate().Refresh();

            CommonActions.ClickLinkElement(ListPriceObjects.EditTierPriceLink(1));

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "EditTierPrice").Should().BeTrue();

            price.PublishedStatus = publishStatus;
            context.SaveChanges();
        }

        [Fact]
        public void Unpublished_EditPrice_NavigatesToCorrectPage()
        {
            using var context = GetEndToEndDbContext();
            var price = context.CataloguePrices.Single(p => p.CatalogueItemId == CatalogueItemId && p.CataloguePriceId == CataloguePriceId);
            var publishStatus = price.PublishedStatus;

            price.PublishedStatus = PublicationStatus.Unpublished;
            context.SaveChanges();

            Driver.Navigate().Refresh();

            CommonActions.ClickLinkElement(ListPriceObjects.EditTierPriceLink(1));

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "EditTieredPriceTier").Should().BeTrue();

            price.PublishedStatus = publishStatus;
            context.SaveChanges();
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
                "EditTieredListPrice",
                updatedParameters);

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
                "EditTieredListPrice",
                updatedParameters);

            CommonActions.ElementIsDisplayed(ListPriceObjects.DeletePriceLink).Should().BeTrue();

            CommonActions.ClickLinkElement(ListPriceObjects.DeletePriceLink);

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "DeleteListPrice").Should().BeTrue();

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "EditTieredListPrice").Should().BeTrue();

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
                "EditTieredListPrice",
                updatedParameters);

            CommonActions.ClickLinkElement(ListPriceObjects.EditTierPriceLink(1));

            CommonActions.ClickLinkElement(ListPriceObjects.DeleteTieredPriceTierLink);

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "DeleteTieredPriceTier").Should().BeTrue();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "EditTieredListPrice").Should().BeTrue();
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
                "EditTieredListPrice",
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
