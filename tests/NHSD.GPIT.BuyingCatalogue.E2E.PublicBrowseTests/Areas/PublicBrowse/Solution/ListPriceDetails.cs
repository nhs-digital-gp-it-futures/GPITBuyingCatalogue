using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
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
    public sealed class ListPriceDetails : AnonymousTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionWithAllSections = new(99998, "001");
        private static readonly CatalogueItemId SolutionWithFlatPriceSection = new(99998, "002");

        private static readonly Dictionary<string, string> ParametersWithAllSections = new()
        {
            { "SolutionId", SolutionWithAllSections.ToString() },
        };

        private static readonly Dictionary<string, string> ParametersWithDefaultSections = new()
        {
            { "SolutionId", SolutionWithFlatPriceSection.ToString() },
        };

        public ListPriceDetails(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
            : base(
                  factory,
                  typeof(SolutionsController),
                  nameof(SolutionsController.ListPrice),
                  ParametersWithAllSections,
                  testOutputHelper)
        {
        }

        [Fact]
        public void ListPriceDetails_HasFlatAndTieredPriceSections_SectionsDisplayed()
        {
            RunTest(() =>
            {
                NavigateToUrl(typeof(SolutionsController), nameof(SolutionsController.ListPrice), ParametersWithAllSections);
                CommonActions.ElementIsDisplayed(ListPriceDetailsObjects.ListPriceTable).Should().BeTrue();
                CommonActions.ElementIsDisplayed(ListPriceDetailsObjects.TieredPriceTable).Should().BeTrue();
                CommonActions.ElementIsDisplayed(ListPriceDetailsObjects.PriceDetails).Should().BeTrue();
            });
        }

        [Fact]
        public void ListPriceDetails_HasFlatPriceSection_SectionsDisplayed()
        {
            NavigateToUrl(typeof(SolutionsController), nameof(SolutionsController.ListPrice), ParametersWithDefaultSections);

            CommonActions.ElementIsDisplayed(ListPriceDetailsObjects.ListPriceTable).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ListPriceDetailsObjects.TieredPriceTable).Should().BeFalse();
            CommonActions.ElementIsDisplayed(ListPriceDetailsObjects.PriceDetails).Should().BeFalse();
        }

        [Fact]
        public async Task ListPriceDetails_SolutionIsSuspended_Redirect()
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
