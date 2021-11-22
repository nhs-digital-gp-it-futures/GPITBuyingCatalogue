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
    public sealed class DevelopmentPlans : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
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

        public DevelopmentPlans(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SolutionsController),
                  nameof(SolutionsController.DevelopmentPlans),
                  ParametersWithAllSections)
        {
        }

        [Fact]
        public void DevelopmentPlans_HasAllSections_SectionsDisplayed()
        {
            CommonActions.ElementTextContains(DevelopmentPlansObjects.RoadmapTitle, "Roadmaps").Should().BeTrue();
            CommonActions.ElementIsDisplayed(DevelopmentPlansObjects.SupplierRoadmapSection).Should().BeTrue();
            CommonActions.ElementIsDisplayed(DevelopmentPlansObjects.ProgramRoadmapSection).Should().BeTrue();
            CommonActions.ElementIsDisplayed(DevelopmentPlansObjects.WorkOffPlansSection).Should().BeTrue();
        }

        [Fact]
        public void DevelopmentPlans_HasDefaultSections_SectionsDisplayed()
        {
            NavigateToUrl(typeof(SolutionsController), nameof(SolutionsController.DevelopmentPlans), ParametersWithDefaultSections);

            CommonActions.ElementTextContains(DevelopmentPlansObjects.RoadmapTitle, "Roadmap").Should().BeTrue();
            CommonActions.ElementIsDisplayed(DevelopmentPlansObjects.SupplierRoadmapSection).Should().BeFalse();
            CommonActions.ElementIsDisplayed(DevelopmentPlansObjects.ProgramRoadmapSection).Should().BeTrue();
            CommonActions.ElementIsDisplayed(DevelopmentPlansObjects.WorkOffPlansSection).Should().BeFalse();
        }

        [Fact]
        public async Task DevelopmentPlans_SolutionIsSuspended_Redirect()
        {
            await using var context = GetEndToEndDbContext();
            var solution = await context.CatalogueItems.SingleAsync(ci => ci.Id == SolutionWithAllSections);
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
            context.CatalogueItems.Single(ci => ci.Id == SolutionWithAllSections).PublishedStatus = PublicationStatus.Published;
            context.SaveChanges();
        }
    }
}
