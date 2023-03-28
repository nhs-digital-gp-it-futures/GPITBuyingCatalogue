using System;
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
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices.Base.Tiered
{
    [Collection(nameof(AdminCollection))]
    public abstract class ListPriceTypeBase : AuthorityTestBase
    {
        protected ListPriceTypeBase(
            LocalWebApplicationFactory factory,
            Type controller,
            IDictionary<string, string> parameters)
            : base(
                factory,
                controller,
                "ListPriceType",
                parameters)
        {
        }

        public static IEnumerable<object[]> PriceTypeTestData => new object[][]
        {
            new object[]
            {
                CataloguePriceType.Tiered,
                "AddTieredListPrice",
            },
            new object[]
            {
                CataloguePriceType.Flat,
                "AddFlatListPrice",
            },
        };

        protected abstract CatalogueItemId CatalogueItemId { get; }

        protected abstract Type Controller { get; }

        [Fact]
        public void AllSectionsDisplayed()
        {
            var catalogueItem = GetCatalogueItemWithPrices(CatalogueItemId);

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
            string methodName)
        {
            CommonActions.ClickRadioButtonWithValue(priceType.ToString());

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                methodName).Should().BeTrue();
        }

        [Fact]
        public void ClickGoBackLink_NavigatesToCorrectPage()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "Index").Should().BeTrue();
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
