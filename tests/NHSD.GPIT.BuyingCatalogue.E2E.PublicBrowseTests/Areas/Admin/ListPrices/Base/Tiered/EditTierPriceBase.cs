using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ListPrices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices.Base.Tiered
{
    [Collection(nameof(AdminCollection))]
    public abstract class EditTierPriceBase : AuthorityTestBase
    {
        private const int TierIndex = 1;

        private static readonly Dictionary<string, string> QueryParameters = new()
        {
            { nameof(TierIndex), TierIndex.ToString() },
        };

        protected EditTierPriceBase(
            LocalWebApplicationFactory factory,
            Type controller,
            IDictionary<string, string> parameters)
            : base(
                factory,
                controller,
                "EditTierPrice",
                parameters,
                QueryParameters)
        {
        }

        protected abstract Type Controller { get; }

        protected abstract CatalogueItemId CatalogueItemId { get; }

        [Fact]
        public void AllSectionsDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.First(p => p.Id == CatalogueItemId);

            CommonActions.PageTitle().Should().Be($"Edit Tier {TierIndex} price - {catalogueItem.Name}".FormatForComparison());
            CommonActions.LedeText().Should().Be("Provide information about this tier.".FormatForComparison());

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(EditTierPriceObjects.PriceInput).Should().BeTrue();
        }

        [Fact]
        public void ClickGoBackLink_NavigatesToCorrectPage()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "EditTieredListPrice").Should().BeTrue();
        }

        [Fact]
        public void ClickSave_NavigatesToCorrectPage()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                Controller,
                "EditTieredListPrice").Should().BeTrue();
        }
    }
}
