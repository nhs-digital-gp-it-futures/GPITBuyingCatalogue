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
    public sealed class AdditionalServicesCapabilities : AnonymousTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly CatalogueItemId AdditionalServiceId = new(99999, "001A99");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
        };

        public AdditionalServicesCapabilities(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
            : base(
                  factory,
                  typeof(SolutionsController),
                  nameof(SolutionsController.CapabilitiesAdditionalServices),
                  Parameters,
                  testOutputHelper)
        {
        }

        [Fact]
        public async Task AdditionalServicesCapabilities_AllCapabilitiesDisplayed()
        {
            await RunTestAsync(async () =>
            {
                var capabilitiesNames = PublicBrowsePages.SolutionAction.GetCapabilitiesListNames().OrderBy(c => c);
                var numberEpicLinks = PublicBrowsePages.SolutionAction.GetCheckEpicLinks();

                await using var context = GetEndToEndDbContext();
                var dbCapabilities = await context.CatalogueItemCapabilities.Include(c => c.Capability).Where(c => c.CatalogueItemId == new CatalogueItemId(99999, "001A99")).ToListAsync();

                capabilitiesNames.Should().HaveCount(dbCapabilities.Count);
                numberEpicLinks.Should().HaveCount(dbCapabilities.Count);
            });
        }

        [Fact]
        public async Task AdditionalServicesCapabilities_SolutionIsSuspended_Redirect()
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
