using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
    public sealed class AdditionalServicesCapabilities : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public AdditionalServicesCapabilities(LocalWebApplicationFactory factory)
            : base(factory, "solutions/futures/99999-001/additional-services/99999-001A999/capabilities")
        {
        }

        [Fact]
        public async Task AdditionalServicesCapabilities_AllCapabilitiesDisplayed()
        {
            var capabilitiesNames = PublicBrowsePages.SolutionAction.GetCapabilitiesListNames();
            var numberEpicLinks = PublicBrowsePages.SolutionAction.GetCheckEpicLinks();

            await using var context = GetEndToEndDbContext();
            var dbCapabilities = await context.CatalogueItemCapabilities.Include(c => c.Capability).Where(c => c.CatalogueItemId == new CatalogueItemId(99999, "001A999")).ToListAsync();

            capabilitiesNames.Should().BeEquivalentTo(dbCapabilities.Select(c => c.Capability.Name));
            numberEpicLinks.Should().HaveCount(dbCapabilities.Count);
        }
    }
}
