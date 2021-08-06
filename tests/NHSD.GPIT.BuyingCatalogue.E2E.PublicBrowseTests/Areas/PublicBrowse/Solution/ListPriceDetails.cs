using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
    public sealed class ListPriceDetails : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public ListPriceDetails(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SolutionDetailsController),
                  nameof(SolutionDetailsController.ListPrice),
                  Parameters)
        {
        }

        [Fact]
        public void ListPriceDetails_FlatListPriceTableDisplayed()
        {
            PublicBrowsePages.SolutionAction.FlatListPriceTableDisplayed().Should().BeTrue();
        }

        [Fact]
        public async Task ListPriceDetails_FlatListPricesDisplayedCorrectlyAsync()
        {
            var prices = PublicBrowsePages.SolutionAction.GetPrices();

            await using var context = GetEndToEndDbContext();
            var dbPrices = await context.CataloguePrices.Where(s => s.CatalogueItemId == new CatalogueItemId(99999, "001")).ToListAsync();

            prices.Should().Contain(dbPrices.Select(s => s.Price.ToString()));
        }
    }
}
