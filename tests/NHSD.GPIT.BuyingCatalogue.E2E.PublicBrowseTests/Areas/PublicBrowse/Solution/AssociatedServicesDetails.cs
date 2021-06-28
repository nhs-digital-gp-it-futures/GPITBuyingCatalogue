using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
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
            var pageTitle = (await context.CatalogueItems.SingleAsync(s => s.CatalogueItemId == "99999-001")).Name;
            PublicBrowsePages.SolutionAction.ImplementationNameDisplayed().Should().BeEquivalentTo($"associated services - {pageTitle}");
        }

        [Fact]
        public void AssociatedServicesDetails_AssociatedServicesTableDisplayed()
        {
            PublicBrowsePages.SolutionAction.AssociatedServicesTableDisplayed().Should().BeTrue();
        }

        [Fact]
        public async Task AssociatedServicesDetails_VerifyAssociatedServices()
        {
            {       
                await using var context = GetEndToEndDbContext();
                             
                var associatedDbInf = (context.CatalogueItems.Include(s => s.AssociatedService).ThenInclude(s => s.AssociatedServiceNavigation).ThenInclude(s => s.Supplier).ThenInclude(s => s.CatalogueItems)
                .Where(s => s.CatalogueItemId == s.CatalogueItemId).ToListAsync());

                //var associatedDbInf = (context.CatalogueItems.Include(s => s.AssociatedService).ThenInclude(s => s.AssociatedServiceNavigation).ThenInclude(s => s.Supplier).ThenInclude(s => s.CatalogueItems)
               // .Where(s => s.CatalogueItemId == s.CatalogueItemId).Select(s => s.AssociatedService).ToListAsync());

                //var associatedInfo = PublicBrowsePages.SolutionAction.GetAssociationServiesInfo().ToArray();
            }
        }
    }  
}
