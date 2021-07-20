using System;
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
    public sealed class AdditionalServiceEpics : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string CapabilityId = "4F09E77B-E3A3-4A25-8EC1-815921F83628";

        public AdditionalServiceEpics(LocalWebApplicationFactory factory)
            : base(factory, "solutions/futures/99999-001/additional-services/99999-001A999/capability/4F09E77B-E3A3-4A25-8EC1-815921F83628")
        {
        }

        [Fact]
        public async Task AdditionalServiceEpics_CapabilityNameDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var capabilityName = (await context.Capabilities.SingleAsync(c => c.Id == Guid.Parse(CapabilityId))).Name;
            var solutionName = (await context.CatalogueItems.SingleAsync(c => c.CatalogueItemId == new CatalogueItemId(99999, "001"))).Name;

            CommonActions.PageTitle().Should().BeEquivalentTo(CommonActions.FormatStringForComparison($"{capabilityName} - {solutionName}"));
        }

        [Fact]
        public async Task AdditionalServiceEpics_EpicsListDisplayedCorrectly()
        {
            await using var context = GetEndToEndDbContext();
            var epics = await context.CatalogueItemEpics.Include(e => e.Epic).Where(e => e.CatalogueItemId == new CatalogueItemId(99999, "001A999")).ToListAsync();

            PublicBrowsePages.SolutionAction.GetNhsSolutionEpics().Should().BeEquivalentTo(epics.Where(e => !e.Epic.SupplierDefined).Select(e => e.Epic.Name));
            PublicBrowsePages.SolutionAction.GetSupplierSolutionEpics().Should().BeEquivalentTo(epics.Where(e => e.Epic.SupplierDefined).Select(e => e.Epic.Name));
        }
    }
}
