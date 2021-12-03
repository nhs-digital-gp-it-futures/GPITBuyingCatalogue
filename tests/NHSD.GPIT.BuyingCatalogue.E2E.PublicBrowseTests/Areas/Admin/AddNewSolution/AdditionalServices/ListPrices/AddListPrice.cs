using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.AdditionalServices.ListPrices
{
    public sealed class AddListPrice : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new(99998, "001");
        private static readonly CatalogueItemId AdditionalServiceId = new(99998, "001A99");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
        };

        public AddListPrice(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AdditionalServicesController),
                  nameof(AdditionalServicesController.AddListPrice),
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
                    typeof(AdditionalServicesController),
                    nameof(AdditionalServicesController.ManageListPrices))
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
        public void AddListPrice_OnDemandProvisioningType_SelectListDisplayed()
        {
            var provisioningType = ProvisioningType.OnDemand.Name();

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
            const string expectedPriceSummaryErrorMessage = "Enter a price";
            const string expectedUnitSummaryErrorMessage = "Enter a unit";
            const string expectedProvisioningTypeSummaryErrorMessage = "Error: Select a provisioning type";

            CommonActions.ClickSave();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(AdditionalServicesController),
                    nameof(AdditionalServicesController.AddListPrice))
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
                    ListPricesObjects.UnitSummaryError,
                    expectedUnitSummaryErrorMessage)
                .Should()
                .BeTrue();

            CommonActions
                .ElementShowingCorrectErrorMessage(
                    ListPricesObjects.ProvisioningTypeSummaryError,
                    expectedProvisioningTypeSummaryErrorMessage)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AddListPrice_ValidInput_ExpectedResult()
        {
            var provisioningType = ProvisioningType.Patient.Name();

            CommonActions
                .ClickRadioButtonWithText(provisioningType);

            CommonActions
                .ElementAddValue(ListPricesObjects.PriceInput, "3.134");

            TextGenerators
                .TextInputAddText(ListPricesObjects.UnitInput, 255);

            CommonActions.ClickSave();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(AdditionalServicesController),
                    nameof(AdditionalServicesController.PublishListPrice))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task AddListPrice_DuplicateListPrice_ThrowsError()
        {
            var catalogueItemPrice = (await GetCataloguePrice()).First();

            CommonActions
                .ClickRadioButtonWithText(catalogueItemPrice.ProvisioningType.Name());

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
                .Where(c => c.CatalogueItemId == AdditionalServiceId)
                .ToListAsync();

            return cataloguePrice;
        }
    }
}
