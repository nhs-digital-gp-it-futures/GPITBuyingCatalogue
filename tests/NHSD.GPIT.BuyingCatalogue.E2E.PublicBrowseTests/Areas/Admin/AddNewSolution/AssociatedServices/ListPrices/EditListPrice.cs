using System.Collections.Generic;
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

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.AssociatedServices.ListPrices
{
    public sealed class EditListPrice : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const int ListPriceId = 17;
        private static readonly CatalogueItemId SolutionId = new(99998, "001");

        private static readonly CatalogueItemId AssociatedServiceId = new(99998, "S-997");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(AssociatedServiceId), AssociatedServiceId.ToString() },
            { nameof(ListPriceId), ListPriceId.ToString() },
        };

        public EditListPrice(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AssociatedServicesController),
                  nameof(AssociatedServicesController.EditListPrice),
                  Parameters)
        {
        }

        [Fact]
        public void EditListPrice_AllSectionsDisplayed()
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
                .SaveButtonDisplayed()
                .Should()
                .BeTrue();
        }

        [Fact]
        public void EditListPrice_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(AssociatedServicesController),
                    nameof(AssociatedServicesController.ManageListPrices))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task EditListPrice_ClickGoBackLink_DoesNotUpdateListPrice()
        {
            var expectedCataloguePrice = await GetCataloguePrice();

            CommonActions
                .ElementAddValue(ListPricesObjects.PriceInput, "52.3");

            CommonActions
                .ElementAddValue(ListPricesObjects.UnitInput, "per Lorem ipsum");

            CommonActions.ClickGoBackLink();

            var actualCataloguePrice = await GetCataloguePrice();

            actualCataloguePrice.Price.Should().Be(expectedCataloguePrice.Price);
            actualCataloguePrice.PricingUnit.Description.Should().Be(expectedCataloguePrice.PricingUnit.Description);
        }

        [Fact]
        public async Task EditListPrice_Submit_UpdatesListPrice()
        {
            const string unitDescription = "per Lorem ipsum";
            const decimal price = 52.3M;

            CommonActions
                .ElementAddValue(ListPricesObjects.PriceInput, price.ToString());

            CommonActions
                .ElementAddValue(ListPricesObjects.UnitInput, unitDescription);

            CommonActions.ClickSave();

            var cataloguePrice = await GetCataloguePrice();

            cataloguePrice.Price.Should().Be(price);
            cataloguePrice.PricingUnit.Description.Should().Be(unitDescription);
        }

        [Fact]
        public void EditListPrice_DuplicateListPrice_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed();
        }

        private async Task<CataloguePrice> GetCataloguePrice()
        {
            await using var context = GetEndToEndDbContext();
            var cataloguePrice = await context
                .CataloguePrices
                .Include(c => c.CatalogueItem)
                .Include(c => c.PricingUnit)
                .SingleAsync(c => c.CatalogueItemId == AssociatedServiceId && c.CataloguePriceId == ListPriceId);

            return cataloguePrice;
        }
    }
}
