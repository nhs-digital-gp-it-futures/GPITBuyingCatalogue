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

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices.Base
{
    [Collection(nameof(AdminCollection))]
    public abstract class ManageListPricesBase : AuthorityTestBase
    {
        protected ManageListPricesBase(
            LocalWebApplicationFactory factory,
            Type controller,
            Dictionary<string, string> parameters)
            : base(
                factory,
                controller,
                "Index",
                parameters)
        {
        }

        protected abstract CatalogueItemId CatalogueItemId { get; }

        protected abstract Type Controller { get; }

        protected abstract Type BacklinkController { get; }

        protected abstract string BacklinkAction { get; }

        [Fact]
        public void AllSectionsDisplayed()
        {
            var catalogueItem = GetCatalogueItemWithPrices(CatalogueItemId);

            CommonActions.PageTitle().Should().Be($"List price - {catalogueItem.Name}".FormatForComparison());
            CommonActions.LedeText().Should().Be($"Provide details of how much your {catalogueItem.CatalogueItemType.Name()} costs. You must add at least one price type.".FormatForComparison());

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
                BacklinkController,
                BacklinkAction).Should().BeTrue();
        }

        [Fact]
        public void ClickContinue_NavigatesToCorrectPage()
        {
            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
                BacklinkController,
                BacklinkAction).Should().BeTrue();
        }

        [Fact]
        public void ClickAddPrice_NavigatesToCorrectPage()
        {
            CommonActions.ClickLinkElement(ManageListPricesObjects.AddPriceLink);

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "AddFlatListPrice").Should().BeTrue();
        }

        [Fact]
        public void ClickEditPrice_Tiered_NavigatesToCorrectPage()
        {
            var catalogueItem = GetCatalogueItemWithPrices(CatalogueItemId);

            var tieredPrice = catalogueItem.CataloguePrices.First(p => p.CataloguePriceType == CataloguePriceType.Tiered);

            CommonActions.ClickLinkElement(ManageListPricesObjects.EditTieredPriceLink(tieredPrice.CataloguePriceId));

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "EditTieredListPrice").Should().BeTrue();
        }

        [Fact]
        public void ClickEditPrice_Flat_NavigatesToCorrectPage()
        {
            var catalogueItem = GetCatalogueItemWithPrices(CatalogueItemId);

            var tieredPrice = catalogueItem.CataloguePrices.First(p => p.CataloguePriceType == CataloguePriceType.Flat);

            CommonActions.ClickLinkElement(ManageListPricesObjects.EditFlatPriceLink(tieredPrice.CataloguePriceId));

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "EditFlatListPrice").Should().BeTrue();
        }

        private CatalogueItem GetCatalogueItemWithPrices(CatalogueItemId id)
            => GetEndToEndDbContext()
            .CatalogueItems
            .Include(ci => ci.CataloguePrices)
            .ThenInclude(p => p.PricingUnit)
            .Include(ci => ci.CataloguePrices)
            .ThenInclude(p => p.CataloguePriceTiers)
            .AsNoTracking()
            .First(ci => ci.Id == id);
    }
}
