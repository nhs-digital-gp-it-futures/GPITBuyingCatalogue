using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
    public class AssociatedServicesDetails : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public AssociatedServicesDetails(LocalWebApplicationFactory factory) : base(factory, "solutions/futures/99999-001/associated-services")
        {
        }

        [Fact]
        public async Task AssociatedServicesDetails_AssociatedServicesNameDisplayedAsync()
        {
            await using var context = GetEndToEndDbContext();
            var pageTitle = (await context.CatalogueItems.SingleAsync(s => s.CatalogueItemId == new CatalogueItemId(99999,"001"))).Name;
            PublicBrowsePages.SolutionAction.ImplementationNameDisplayed().Should().BeEquivalentTo($"associated services - {pageTitle}");
        }

        [Fact]
        public void AssociatedServicesDetails_AssociatedServicesTableDisplayed()
        {
            PublicBrowsePages.SolutionAction.AssociatedServicesTableDisplayed().Should().BeTrue();
        }

        [Fact]
        public async Task AssociatedServicesDetails_AssociatedServicesListedInTable()
        {
            await using var context = GetEndToEndDbContext();

            var associatedServicesInDb = await context.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AssociatedService).Where(c => c.SupplierId == "99999").ToListAsync();

            var associatedServicesInTable = PublicBrowsePages.SolutionAction.GetAssociatedServicesNamesFromTable();

            associatedServicesInTable.Should().BeEquivalentTo(associatedServicesInDb.Select(s => s.Name));
        }

        [Fact]
        public async Task AssociatedServicesDetails_AssociatedServicesDetailsListed()
        {
            await using var context = GetEndToEndDbContext();

            var associatedServicesInDb = await context.CatalogueItems.Include(s => s.AssociatedService).Where(c => c.CatalogueItemType == CatalogueItemType.AssociatedService).Where(c => c.SupplierId == "99999").ToListAsync();

            var associatedServicesOnPage = PublicBrowsePages.SolutionAction.GetAssociatedServicesInfo();

            associatedServicesOnPage.Should().BeEquivalentTo(associatedServicesInDb.Select(s => s.AssociatedService),
                options =>
                    options.Including(s => s.Description)
                    .Including(s => s.OrderGuidance));
        }
    }  
}
