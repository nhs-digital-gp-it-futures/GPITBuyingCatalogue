using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
    public sealed class Standards : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private static readonly CatalogueItemId SolutionWithInteropId = new(99998, "001");
        private static readonly CatalogueItemId SolutionWithoutInteropId = new(99998, "002");

        private static readonly Dictionary<string, string> ParametersWithInterop = new()
        {
            { "SolutionId", SolutionWithInteropId.ToString() },
        };

        private static readonly Dictionary<string, string> ParametersWithoutInterop = new()
        {
            { "SolutionId", SolutionWithoutInteropId.ToString() },
        };

        public Standards(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SolutionsController),
                  nameof(SolutionsController.Standards),
                  ParametersWithInterop)
        {
        }

        [Fact]
        public void Standards_HasInterop_SectionsDisplayed()
        {
            CommonActions.ElementIsDisplayed(StandardsObjects.OverarchingTable).Should().BeTrue();
            CommonActions.ElementIsDisplayed(StandardsObjects.InteroperabilityTable).Should().BeTrue();
        }

        [Fact]
        public void Standards_NoInterop_SectionsDisplayed()
        {
            NavigateToUrl(typeof(SolutionsController), nameof(SolutionsController.Standards), ParametersWithoutInterop);

            CommonActions.ElementIsDisplayed(StandardsObjects.OverarchingTable).Should().BeTrue();
            CommonActions.ElementIsDisplayed(StandardsObjects.InteroperabilityTable).Should().BeFalse();
        }

        [Fact]
        public async Task Standards_SolutionIsSuspended_Redirect()
        {
            await using var context = GetEndToEndDbContext();
            var solution = await context.CatalogueItems.SingleAsync(ci => ci.Id == SolutionWithInteropId);
            solution.PublishedStatus = PublicationStatus.Suspended;
            await context.SaveChangesAsync();

            Driver.Navigate().Refresh();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(SolutionsController),
                    nameof(SolutionsController.Description))
                .Should()
                .BeTrue();
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            context.CatalogueItems.Single(ci => ci.Id == SolutionWithInteropId).PublishedStatus = PublicationStatus.Published;
            context.SaveChanges();
        }
    }
}
