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
    public sealed class ApplicationTypeDetails : AnonymousTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public ApplicationTypeDetails(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
            : base(
                  factory,
                  typeof(SolutionsController),
                  nameof(SolutionsController.ApplicationTypes),
                  Parameters,
                  testOutputHelper)
        {
        }

        [Theory]
        [InlineData("browser-based application")]
        [InlineData("desktop application")]
        [InlineData("mobile or tablet application")]
        public void ApplicationTypeDetails_AllFieldsDisplayed(string rowHeader)
        {
            RunTest(() =>
            {
                PublicBrowsePages.SolutionAction.GetTableRowContent(rowHeader).Should().NotBeNullOrEmpty();
            });
        }

        [Fact]
        public async Task ApplicationTypeDetails_SolutionIsSuspended_Redirect()
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
