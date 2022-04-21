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

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices.Solution
{
    public sealed class ManageListPrices : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new CatalogueItemId(99998, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public ManageListPrices(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CatalogueSolutionListPriceController),
                  nameof(CatalogueSolutionListPriceController.Index),
                  Parameters)
        {
        }

        [Fact]
        public void AllSectionsDisplayed()
        {
            var catalogueItem = GetCatalogueItemWithPrices(SolutionId);

            CommonActions.PageTitle().Should().Be($"List price - {catalogueItem.Name}".FormatForComparison());
            CommonActions.LedeText().Should().Be("Provide details of how much your Catalogue Solution costs. You must add at least one price type.".FormatForComparison());

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ContinueButtonDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(ManageListPricesObjects.AddPriceLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ManageListPricesObjects.TieredPrices).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ManageListPricesObjects.FlatPrices).Should().BeTrue();

            var tieredPrices = catalogueItem.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Tiered).ToList();
            var flatPrices = catalogueItem.CataloguePrices.Except(tieredPrices).ToList();

            tieredPrices.ForEach(p => CommonActions.ElementIsDisplayed(ManageListPricesObjects.TieredPrice(p.CataloguePriceId)).Should().BeTrue());
            flatPrices.ForEach(p => CommonActions.ElementIsDisplayed(ManageListPricesObjects.FlatPrice(p.CataloguePriceId)).Should().BeTrue());
            catalogueItem.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Tiered).ToList().ForEach(p => CommonActions.ElementIsDisplayed(ManageListPricesObjects.EditPriceLink(p.CataloguePriceId)).Should().BeTrue());
        }

        [Fact]
        public void ClickGoBackLink_NavigatesToCorrectPage()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ManageCatalogueSolution)).Should().BeTrue();
        }

        [Fact]
        public void ClickContinue_NavigatesToCorrectPage()
        {
            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ManageCatalogueSolution)).Should().BeTrue();
        }

        [Fact]
        public void ClickAddPrice_NavigatesToCorrectPage()
        {
            CommonActions.ClickLinkElement(ManageListPricesObjects.AddPriceLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.ListPriceType)).Should().BeTrue();
        }

        [Fact]
        public void ClickEditPrice_Tiered_NavigatesToCorrectPage()
        {
            var catalogueItem = GetCatalogueItemWithPrices(SolutionId);

            var tieredPrice = catalogueItem.CataloguePrices.First(p => p.CataloguePriceType == CataloguePriceType.Tiered);

            CommonActions.ClickLinkElement(ManageListPricesObjects.EditPriceLink(tieredPrice.CataloguePriceId));

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.EditTieredListPrice)).Should().BeTrue();
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
