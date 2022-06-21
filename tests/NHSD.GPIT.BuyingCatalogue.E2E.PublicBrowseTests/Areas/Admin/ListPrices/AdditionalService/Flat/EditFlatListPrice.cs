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
using OpenQA.Selenium;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices.AdditionalService.Flat
{
    public sealed class EditFlatListPrice : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const int CataloguePriceId = 10;
        private static readonly CatalogueItemId SolutionId = new(99998, "001");
        private static readonly CatalogueItemId AdditionalServiceId = new(99998, "001A99");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
            { nameof(CataloguePriceId), CataloguePriceId.ToString() },
        };

        public EditFlatListPrice(LocalWebApplicationFactory factory)
            : base(
                factory,
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.EditFlatListPrice),
                Parameters)
        {
        }

        [Fact]
        public void AllSectionsDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Single(c => c.Id == AdditionalServiceId);

            CommonActions.PageTitle().Should().Be($"Edit a flat list price - {catalogueItem.Name}".FormatForComparison());
            CommonActions.LedeText().Should().Be("Provide the following information about the pricing model for your Additional Service.".FormatForComparison());

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(ListPriceObjects.ProvisioningTypeInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPriceObjects.UnitDescriptionInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPriceObjects.UnitDefinitionInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPriceObjects.PriceInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPriceObjects.PublicationStatusInput).Should().BeTrue();

            CommonActions.ElementIsDisplayed(ListPriceObjects.OnDemandBillingPeriodInput).Should().BeFalse();
            CommonActions.ElementIsDisplayed(ListPriceObjects.DeclarativeBillingPeriodInput).Should().BeFalse();

            CommonActions.ElementIsDisplayed(ListPriceObjects.DeletePriceLink).Should().BeFalse();

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
        public void Unpublished_DeleteLinkVisible()
        {
            using var context = GetEndToEndDbContext();
            var cataloguePrice = context.CataloguePrices.Single(p => p.CataloguePriceId == CataloguePriceId);
            var originalPublishStatus = cataloguePrice.PublishedStatus;

            cataloguePrice.PublishedStatus = PublicationStatus.Unpublished;
            context.SaveChanges();

            Driver.Navigate().Refresh();

            CommonActions.ElementIsDisplayed(ListPriceObjects.DeletePriceLink).Should().BeTrue();

            cataloguePrice.PublishedStatus = originalPublishStatus;
            context.SaveChanges();
        }

        [Fact]
        public void ClickGoBackLink_NavigatesToCorrectPage()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.Index)).Should().BeTrue();
        }

        [Fact]
        public void ClickDelete_NavigatesToCorrectPage()
        {
            using var context = GetEndToEndDbContext();
            var cataloguePrice = context.CataloguePrices.Single(p => p.CataloguePriceId == CataloguePriceId);
            var originalPublishStatus = cataloguePrice.PublishedStatus;

            cataloguePrice.PublishedStatus = PublicationStatus.Unpublished;
            context.SaveChanges();

            Driver.Navigate().Refresh();

            CommonActions.ClickLinkElement(ListPriceObjects.DeletePriceLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.DeleteListPrice)).Should().BeTrue();

            cataloguePrice.PublishedStatus = originalPublishStatus;
            context.SaveChanges();
        }

        [Fact]
        public void Submit_Duplicate_ThrowsError()
        {
            var catalogueItem = GetCatalogueItemWithPrices(AdditionalServiceId);
            var price = catalogueItem.CataloguePrices.First(p => p.CataloguePriceType == CataloguePriceType.Flat && p.CataloguePriceId != CataloguePriceId);

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
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.Index)).Should().BeTrue();
        }

        [Fact]
        public void DeletePrice_Redirects()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Include(c => c.CataloguePrices).First(c => c.Id == AdditionalServiceId);

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
                { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
                { nameof(CataloguePriceId), price.CataloguePriceId.ToString() },
            };

            NavigateToUrl(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.EditFlatListPrice),
                parameters);

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
