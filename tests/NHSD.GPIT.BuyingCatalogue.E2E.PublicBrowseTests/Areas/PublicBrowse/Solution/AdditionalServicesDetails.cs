using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
    public class AdditionalServicesDetails : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public AdditionalServicesDetails(LocalWebApplicationFactory factory) : base(factory, "solutions/futures/99999-001/additional-services")
        {
        }

        [Fact]
        public async Task AdditionalServicesDetails_AdditionalServicesNameDisplayedAsync()
        {
            await using var context = GetEndToEndDbContext();
            var pageTitle = (await context.CatalogueItems.SingleAsync(s => s.CatalogueItemId == new CatalogueItemId(99999, "001"))).Name;
            PublicBrowsePages.SolutionAction.AdditionalServicesNameDisplayed().Should().BeEquivalentTo($"additional services - {pageTitle}");
        }

        [Fact]
        public void AdditionalServicesDetail_AdditionalServicesTableDisplayed()
        {
            PublicBrowsePages.SolutionAction.AdditionalServicesTableDisplayed().Should().BeTrue();
        }

        [Fact]
        public async Task AdditionalServicesDetail_AdditionalServicesListedInTable()
        {
            await using var context = GetEndToEndDbContext();

            var additionalServicesInDb = await context.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AdditionalService).Where(c => c.SupplierId == "99999").ToListAsync();

            var additionalServicesInTable = PublicBrowsePages.SolutionAction.GetAdditionalServicesNamesFromTable();

            additionalServicesInTable.Should().BeEquivalentTo(additionalServicesInDb.Select(s => s.Name));
        }

        [Fact]
        public async Task AdditionalServicesDetails_AdditionalServicesDescriptionListed()
        {
            await using var context = GetEndToEndDbContext();

            var additionalDescInDb = (await context.CatalogueItems.Include(c => c.AdditionalService).Where(c => c.CatalogueItemType == CatalogueItemType.AdditionalService).Where(c => c.SupplierId == "99999").ToListAsync()).Select(a => a.AdditionalService.FullDescription);

            var additionalDescOnPage = PublicBrowsePages.SolutionAction.GetAdditionalServicesDescription();

            additionalDescOnPage.Should().BeEquivalentTo(additionalDescInDb);
        }
    }
}
