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

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices.AdditionalService
{
    public sealed class ManageListPrices : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new(99998, "001");
        private static readonly CatalogueItemId AdditionalServiceId = new(99998, "001A99");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
        };

        public ManageListPrices(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AdditionalServiceListPriceController),
                  nameof(AdditionalServiceListPriceController.Index),
                  Parameters)
        {
        }

        [Fact]
        public void AllSectionsDisplayed()
        {
            var catalogueItem = GetCatalogueItemWithPrices(AdditionalServiceId);

            CommonActions.PageTitle().Should().Be($"List price - {catalogueItem.Name}".FormatForComparison());
            CommonActions.LedeText().Should().Be("Provide details of how much your Additional Service costs. You must add at least one price type.".FormatForComparison());

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ContinueButtonDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(ManageListPricesObjects.AddPriceLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ManageListPricesObjects.TieredPrices).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ManageListPricesObjects.FlatPrices).Should().BeTrue();

            var tieredPrices = catalogueItem.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Tiered).ToList();
            var flatPrices = catalogueItem.CataloguePrices.Except(tieredPrices).ToList();

            tieredPrices.ForEach(p => CommonActions.ElementIsDisplayed(ManageListPricesObjects.TieredPrice(p.CataloguePriceId)).Should().BeTrue());
            flatPrices.ForEach(p => CommonActions.ElementIsDisplayed(ManageListPricesObjects.FlatPrice(p.CataloguePriceId)).Should().BeTrue());
            catalogueItem.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Tiered).ToList().ForEach(p => CommonActions.ElementIsDisplayed(ManageListPricesObjects.EditTieredPriceLink(p.CataloguePriceId)).Should().BeTrue());
            catalogueItem.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat).ToList().ForEach(p => CommonActions.ElementIsDisplayed(ManageListPricesObjects.EditFlatPriceLink(p.CataloguePriceId)).Should().BeTrue());
        }

        [Fact]
        public void ClickGoBackLink_NavigatesToCorrectPage()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.EditAdditionalService)).Should().BeTrue();
        }

        [Fact]
        public void ClickContinue_NavigatesToCorrectPage()
        {
            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.EditAdditionalService)).Should().BeTrue();
        }

        [Fact]
        public void ClickAddPrice_NavigatesToCorrectPage()
        {
            CommonActions.ClickLinkElement(ManageListPricesObjects.AddPriceLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.ListPriceType)).Should().BeTrue();
        }

        [Fact]
        public void ClickEditPrice_Tiered_NavigatesToCorrectPage()
        {
            var catalogueItem = GetCatalogueItemWithPrices(AdditionalServiceId);

            var tieredPrice = catalogueItem.CataloguePrices.First(p => p.CataloguePriceType == CataloguePriceType.Tiered);

            CommonActions.ClickLinkElement(ManageListPricesObjects.EditTieredPriceLink(tieredPrice.CataloguePriceId));

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.EditTieredListPrice)).Should().BeTrue();
        }

        [Fact]
        public void ClickEditPrice_Flat_NavigatesToCorrectPage()
        {
            var catalogueItem = GetCatalogueItemWithPrices(AdditionalServiceId);

            var tieredPrice = catalogueItem.CataloguePrices.First(p => p.CataloguePriceType == CataloguePriceType.Flat);

            CommonActions.ClickLinkElement(ManageListPricesObjects.EditFlatPriceLink(tieredPrice.CataloguePriceId));

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.EditFlatListPrice)).Should().BeTrue();
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
