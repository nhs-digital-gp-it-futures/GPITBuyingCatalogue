﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices
{
    public sealed class AddListPrice : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public AddListPrice(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(ListPriceController),
                  nameof(ListPriceController.AddListPrice),
                  Parameters)
        {
        }

        [Fact]
        public void AddListPrice_AllSectionsDisplayed()
        {
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions
                .ElementIsDisplayed(CommonSelectors.Header1)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsDisplayed(ListPricesObjects.ProvisioningTypeInput)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsNotDisplayed(ListPricesObjects.DeclarativeTimeInput)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsNotDisplayed(ListPricesObjects.OnDemandTimeInput)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsDisplayed(ListPricesObjects.PriceInput)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsDisplayed(ListPricesObjects.UnitInput)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsDisplayed(ListPricesObjects.UnitDefinitionInput)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsDisplayed(CommonSelectors.SubmitButton)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AddListPrice_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(ListPriceController),
                    nameof(ListPriceController.Index))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task AddListPrice_ClickGoBackLink_DoesNotSavePrice()
        {
            var expectedNumberOfListPrices = (await GetCataloguePrice()).Count;

            CommonActions.ClickGoBackLink();

            var actualNumberOfListPrices = (await GetCataloguePrice()).Count;

            actualNumberOfListPrices.Should().Be(expectedNumberOfListPrices);
        }

        [Fact]
        public void AddListPrice_DeclarativeProvisioningType_SelectListDisplayed()
        {
            const string provisioningType = "Declarative";

            CommonActions
                .ElementIsDisplayed(ListPricesObjects.DeclarativeTimeInput)
                .Should()
                .BeFalse();

            CommonActions
                .ClickRadioButtonWithText(provisioningType);

            CommonActions
                .ElementIsDisplayed(ListPricesObjects.DeclarativeTimeInput)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AddListPrice_OnDemandProvisioningType_SelectListDisplayed()
        {
            const string provisioningType = "On Demand";

            CommonActions
                .ElementIsDisplayed(ListPricesObjects.OnDemandTimeInput)
                .Should()
                .BeFalse();

            CommonActions
                .ClickRadioButtonWithText(provisioningType);

            CommonActions
                .ElementIsDisplayed(ListPricesObjects.OnDemandTimeInput)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AddListPrice_NoInput_ThrowsError()
        {
            const string expectedPriceSummaryErrorMessage = "A valid price must be entered.";
            const string expectedUnitSummaryErrorMessage = "The Unit field is required.";

            CommonActions.ClickSave();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(ListPriceController),
                    nameof(ListPriceController.AddListPrice))
                .Should()
                .BeTrue();

            CommonActions
                .ErrorSummaryDisplayed()
                .Should()
                .BeTrue();

            CommonActions
                .ErrorSummaryLinksExist()
                .Should()
                .BeTrue();

            CommonActions
                .ElementShowingCorrectErrorMessage(
                    ListPricesObjects.PriceSummaryError,
                    expectedPriceSummaryErrorMessage)
                .Should()
                .BeTrue();

            CommonActions
                .ElementShowingCorrectErrorMessage(
                    ListPricesObjects.UnitSummaryERror,
                    expectedUnitSummaryErrorMessage)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AddListPrice_ValidInput_ExpectedResult()
        {
            const string provisioningType = "Patient";

            CommonActions
                .ClickRadioButtonWithText(provisioningType);

            CommonActions
                .ElementAddValue(ListPricesObjects.PriceInput, "3.134");

            TextGenerators
                .TextInputAddText(ListPricesObjects.UnitInput, 255);

            CommonActions.ClickSave();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(ListPriceController),
                    nameof(ListPriceController.Index))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task AddListPrice_DuplicateListPrice_ThrowsError()
        {
            var catalogueItemPrice = (await GetCataloguePrice()).First();

            CommonActions
                .ClickRadioButtonWithText(catalogueItemPrice.ProvisioningType.ToString());

            CommonActions
                .ElementAddValue(ListPricesObjects.PriceInput, catalogueItemPrice.Price.ToString());

            CommonActions
                .ElementAddValue(ListPricesObjects.UnitInput, catalogueItemPrice.PricingUnit.Description);

            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed();
        }

        private async Task<IList<CataloguePrice>> GetCataloguePrice()
        {
            await using var context = GetEndToEndDbContext();
            var cataloguePrice = await context
                .CataloguePrices
                .Include(c => c.CatalogueItem)
                .Include(c => c.PricingUnit)
                .Where(c => c.CatalogueItemId == SolutionId)
                .ToListAsync();

            return cataloguePrice;
        }
    }
}
