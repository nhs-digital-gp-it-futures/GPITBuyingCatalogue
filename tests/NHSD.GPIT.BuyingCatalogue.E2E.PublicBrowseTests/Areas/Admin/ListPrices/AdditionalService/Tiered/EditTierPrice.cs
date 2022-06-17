using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ListPrices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices.AdditionalService.Tiered
{
    public sealed class EditTierPrice : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const int CataloguePriceId = 14;
        private const int TierId = 17;
        private const int TierIndex = 1;
        private static readonly CatalogueItemId SolutionId = new(99998, "001");
        private static readonly CatalogueItemId AdditionalServiceId = new(99998, "001A99");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
            { nameof(CataloguePriceId), CataloguePriceId.ToString() },
            { nameof(TierId), TierId.ToString() },
        };

        private static readonly Dictionary<string, string> QueryParameters = new()
        {
            { nameof(TierIndex), TierIndex.ToString() },
        };

        public EditTierPrice(LocalWebApplicationFactory factory)
            : base(
                factory,
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.EditTierPrice),
                Parameters,
                QueryParameters)
        {
        }

        [Fact]
        public void AllSectionsDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems.Single(p => p.Id == AdditionalServiceId);

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
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.EditTieredListPrice)).Should().BeTrue();
        }

        [Fact]
        public void ClickSave_NavigatesToCorrectPage()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.EditTieredListPrice)).Should().BeTrue();
        }
    }
}
