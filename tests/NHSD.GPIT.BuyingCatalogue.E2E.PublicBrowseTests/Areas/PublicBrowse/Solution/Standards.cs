using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse;
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
    public sealed class Standards : AnonymousTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionWithAllSections = new(99998, "001");
        private static readonly CatalogueItemId SolutionWithDefaultSection = new(99998, "002");

        private static readonly Dictionary<string, string> ParametersWithAllSections = new()
        {
            { "SolutionId", SolutionWithAllSections.ToString() },
        };

        private static readonly Dictionary<string, string> ParametersWithDefaultSections = new()
        {
            { "SolutionId", SolutionWithDefaultSection.ToString() },
        };

        public Standards(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
            : base(
                  factory,
                  typeof(SolutionsController),
                  nameof(SolutionsController.Standards),
                  ParametersWithAllSections,
                  testOutputHelper)
        {
        }

        [Fact]
        public void Standards_HasAllSections_SectionsDisplayed()
        {
            RunTest(() =>
            {
                CommonActions.ElementIsDisplayed(StandardsObjects.OverarchingTable).Should().BeTrue();
                CommonActions.ElementIsDisplayed(StandardsObjects.InteroperabilityTable).Should().BeTrue();
                CommonActions.ElementIsDisplayed(StandardsObjects.CapabilityTable).Should().BeTrue();
            });
        }

        [Fact]
        public void Standards_NoInterop_SectionsDisplayed()
        {
            NavigateToUrl(typeof(SolutionsController), nameof(SolutionsController.Standards), ParametersWithDefaultSections);

            CommonActions.ElementIsDisplayed(StandardsObjects.OverarchingTable).Should().BeTrue();
            CommonActions.ElementIsDisplayed(StandardsObjects.InteroperabilityTable).Should().BeFalse();
            CommonActions.ElementIsDisplayed(StandardsObjects.CapabilityTable).Should().BeFalse();
        }

        [Fact]
        public async Task Standards_SolutionIsSuspended_Redirect()
        {
            await RunTestAsync(async () =>
            {
                await using var context = GetEndToEndDbContext();
                var solution = await context.CatalogueItems.FirstAsync(ci => ci.Id == SolutionWithAllSections);
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
            context.CatalogueItems.First(ci => ci.Id == SolutionWithAllSections).PublishedStatus = PublicationStatus.Published;
            context.SaveChanges();
        }
    }
}
