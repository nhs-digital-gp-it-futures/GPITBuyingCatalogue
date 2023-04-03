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
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
    [Collection(nameof(SharedContextCollection))]
    public sealed class AdditionalServiceEpics : AnonymousTestBase, IDisposable
    {
        private static readonly int CapabilityId = 2;

        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly CatalogueItemId AdditionalServiceId = new(99999, "001A99");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
            { nameof(CapabilityId), CapabilityId.ToString() },
        };

        public AdditionalServiceEpics(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
            : base(
                  factory,
                  typeof(SolutionsController),
                  nameof(SolutionsController.CheckEpicsAdditionalServices),
                  Parameters,
                  testOutputHelper)
        {
        }

        [Fact]
        public async Task AdditionalServiceEpics_CapabilityNameDisplayed()
        {
            await RunTestAsync(async () =>
            {
                await using var context = GetEndToEndDbContext();
                var capabilityName = (await context.Capabilities.FirstAsync(c => c.Id == CapabilityId)).Name;
                var solutionName = (await context.CatalogueItems.FirstAsync(c => c.Id == AdditionalServiceId)).Name;

                CommonActions.PageTitle().Should().BeEquivalentTo($"{capabilityName} - {solutionName}".FormatForComparison());
            });
        }

        [Fact]
        public async Task AdditionalServiceEpics_EpicsListDisplayedCorrectly()
        {
            await RunTestAsync(async () =>
            {
                await using var context = GetEndToEndDbContext();
                var epics = await context.CatalogueItemEpics.Include(e => e.Epic).Where(e => e.CatalogueItemId == AdditionalServiceId).ToListAsync();

                PublicBrowsePages.SolutionAction.GetNhsSolutionEpics().Should().BeEquivalentTo(epics.Where(e => !e.Epic.SupplierDefined).Select(e => e.Epic.Name));
                PublicBrowsePages.SolutionAction.GetSupplierSolutionEpics().Should().BeEquivalentTo(epics.Where(e => e.Epic.SupplierDefined).Select(e => e.Epic.Name));
            });
        }

        [Fact]
        public async Task AdditionalServiceEpics_SolutionIsSuspended_Redirect()
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
