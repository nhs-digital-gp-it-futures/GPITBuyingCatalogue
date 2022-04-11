using System;
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
    public sealed class ListPriceType : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new CatalogueItemId(99998, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public ListPriceType(LocalWebApplicationFactory factory)
            : base(
                factory,
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.ListPriceType),
                Parameters)
        {
        }

        public static IEnumerable<object[]> PriceTypeTestData => new object[][]
        {
            new object[]
            {
                CataloguePriceType.Tiered,
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.AddTieredListPrice),
            },
            new object[]
            {
                CataloguePriceType.Flat,
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.ListPriceType),
            },
        };

        [Fact]
        public void AllSectionsDisplayed()
        {
            var catalogueItem = GetCatalogueItemWithPrices(SolutionId);

            CommonActions.PageTitle().Should().Be($"List price type - {catalogueItem.Name}".FormatForComparison());
            CommonActions.LedeText().Should().Be("Select either a flat or tiered list price type.".FormatForComparison());

            CommonActions.GetNumberOfRadioButtonsDisplayed().Should().Be(2);
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void ClickContinue_NoSelection_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                ListPriceTypeObjects.SelectedCataloguePriceTypeError,
                "Error: Select a price type").Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(PriceTypeTestData))]
        public void ClickContinue_WithSelection_NavigatesToCorrectPage(
            CataloguePriceType priceType,
            Type controllerType,
            string methodName)
        {
            CommonActions.ClickRadioButtonWithValue(priceType.ToString());

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                controllerType,
                methodName).Should().BeTrue();
        }

        [Fact]
        public void ClickGoBackLink_NavigatesToCorrectPage()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.Index)).Should().BeTrue();
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
