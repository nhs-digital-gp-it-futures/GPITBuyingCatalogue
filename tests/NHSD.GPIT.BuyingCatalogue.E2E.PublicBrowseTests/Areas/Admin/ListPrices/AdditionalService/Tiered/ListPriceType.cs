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
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices.AdditionalService.Tiered
{
    public sealed class ListPriceType : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new(99998, "001");
        private static readonly CatalogueItemId AdditionalServiceId = new(99998, "001A99");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
        };

        public ListPriceType(LocalWebApplicationFactory factory)
            : base(
                factory,
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.ListPriceType),
                Parameters)
        {
        }

        public static IEnumerable<object[]> PriceTypeTestData => new[]
        {
            new object[]
            {
                CataloguePriceType.Tiered,
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.AddTieredListPrice),
            },
            new object[]
            {
                CataloguePriceType.Flat,
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.AddFlatListPrice),
            },
        };

        [Fact]
        public void AllSectionsDisplayed()
        {
            var catalogueItem = GetCatalogueItemWithPrices(AdditionalServiceId);

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
