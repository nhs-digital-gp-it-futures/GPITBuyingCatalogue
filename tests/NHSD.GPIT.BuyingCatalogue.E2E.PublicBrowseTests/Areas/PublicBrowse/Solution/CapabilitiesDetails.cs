using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
    [Collection(nameof(SharedContextCollection))]
    public sealed class CapabilitiesDetails : AnonymousTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public CapabilitiesDetails(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
            : base(
                  factory,
                  typeof(SolutionsController),
                  nameof(SolutionsController.Capabilities),
                  Parameters,
                  testOutputHelper)
        {
        }

        [Fact]
        public async Task CapabilitiesDetails_VerifyCapabilities()
        {
            await RunTestAsync(async () =>
            {
                await using var context = GetEndToEndDbContext();
                var capabilitiesInfo =
                    (await context.CatalogueItems
                        .Include(ci => ci.CatalogueItemCapabilities)
                            .ThenInclude(s => s.Capability)
                        .FirstAsync(s => s.Id == new CatalogueItemId(99999, "001")))
                    .CatalogueItemCapabilities
                    .Select(s => s.Capability);

                var capabilitiesList = PublicBrowsePages.SolutionAction.GetCapabilitiesContent().ToArray()[0];

                var capabilitiesTitle = capabilitiesInfo.Select(c => c.Name.Trim());
                foreach (var name in capabilitiesTitle)
                {
                    capabilitiesList.Should().Contain(name);
                }

                var capabilitiesDescription = capabilitiesInfo.Select(b => b.Description.Trim());
                foreach (var description in capabilitiesDescription)
                {
                    capabilitiesList.Should().Contain(description);
                }
            });
        }

        [Fact]
        public async Task CapabilitiesDetails_CheckEpics_NhsDefinedSolutionEpics()
        {
            await RunTestAsync(async () =>
            {
                PublicBrowsePages.SolutionAction.ClickEpics();
                var nhsEpicsList = PublicBrowsePages.SolutionAction.GetNhsSolutionEpics().ToArray();

                await using var context = GetEndToEndDbContext();
                var nhsEpicsInfo = (await context.CatalogueItems.Include(s => s.CatalogueItemEpics).ThenInclude(s => s.Epic).FirstAsync(s => s.Id == new CatalogueItemId(99999, "001"))).CatalogueItemEpics.Select(s => s.Epic);

                nhsEpicsList.Should().BeEquivalentTo(nhsEpicsInfo.Where(e => !e.SupplierDefined).Select(c => c.Name));
            });
        }

        [Fact]
        public async Task CapabilitiesDetails_CheckEpics_SupplierDefinedSolutionEpics()
        {
            await RunTestAsync(async () =>
            {
                PublicBrowsePages.SolutionAction.ClickEpics();
                var supplierEpicsList = PublicBrowsePages.SolutionAction.GetSupplierSolutionEpics();

                await using var context = GetEndToEndDbContext();
                var supplierEpicsInfo = (await context.CatalogueItems.Include(s => s.CatalogueItemEpics).ThenInclude(s => s.Epic).FirstAsync(s => s.Id == new CatalogueItemId(99999, "001"))).CatalogueItemEpics.Select(s => s.Epic);

                supplierEpicsList.Should().BeEquivalentTo(supplierEpicsInfo.Where(e => e.SupplierDefined).Select(c => c.Name));
            });
        }

        [Fact]
        public async Task CapabilitiesDetails_SolutionIsSuspended_Redirect()
        {
            await RunTestAsync(async () =>
            {
                await using var context = GetEndToEndDbContext();
                var solution = await context.CatalogueItems.FirstAsync(ci => ci.Id == SolutionId);
                solution.PublishedStatus = PublicationStatus.Suspended;
                await context.SaveChangesAsync();

                Driver.Navigate().Refresh();

                CommonActions
                    .PageLoadedCorrectGetIndex(
                        typeof(SolutionsController),
                        nameof(SolutionsController.Description))
                    .Should()
                    .BeTrue();
            });
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            context.CatalogueItems.First(ci => ci.Id == SolutionId).PublishedStatus = PublicationStatus.Published;
            context.SaveChanges();
        }
    }
}
