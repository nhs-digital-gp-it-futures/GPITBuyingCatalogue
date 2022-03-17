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
    public sealed class ListPriceDetails : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public ListPriceDetails(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
            : base(
                  factory,
                  typeof(SolutionsController),
                  nameof(SolutionsController.ListPrice),
                  Parameters,
                  testOutputHelper)
        {
        }

        [Fact]
        public void ListPriceDetails_FlatListPriceTableDisplayed()
        {
            RunTest(() =>
            {
                PublicBrowsePages.SolutionAction.FlatListPriceTableDisplayed().Should().BeTrue();
            });
        }

        [Fact]
        public async Task ListPriceDetails_FlatListPricesDisplayedCorrectlyAsync()
        {
            await RunTestAsync(async () =>
            {
                var prices = PublicBrowsePages.SolutionAction.GetPrices();

                await using var context = GetEndToEndDbContext();
                var dbPrices = await context.CataloguePrices.Where(s =>
                s.CatalogueItemId == new CatalogueItemId(99999, "001")
                && s.PublishedStatus == PublicationStatus.Published).ToListAsync();

                prices.Should().Contain(dbPrices.Select(s => s.Price.ToString()));
            });
        }

        [Fact]
        public async Task ListPriceDetails_SolutionIsSuspended_Redirect()
        {
            await RunTestAsync(async () =>
            {
                await using var context = GetEndToEndDbContext();
                var solution = await context.CatalogueItems.SingleAsync(ci => ci.Id == SolutionId);
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
            context.CatalogueItems.Single(ci => ci.Id == SolutionId).PublishedStatus = PublicationStatus.Published;
            context.SaveChanges();
        }
    }
}
