using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
    public sealed class ListPriceDetails : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public ListPriceDetails(LocalWebApplicationFactory factory) : base(factory, "solutions/futures/99999-001/list-price")
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

            using var context = GetBCContext();
            var dbPrices = await context.CataloguePrices.Where(s => s.CatalogueItemId == "99999-001").ToListAsync();

            prices.Should().BeEquivalentTo(dbPrices.Select(s => $"£{s.Price}"));
        }
    }
}
