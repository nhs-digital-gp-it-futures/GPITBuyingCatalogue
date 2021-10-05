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

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
    public sealed class ApplicationTypeDetails : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public ApplicationTypeDetails(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SolutionsController),
                  nameof(SolutionsController.ClientApplicationTypes),
                  Parameters)
        {
        }

        [Theory]
        [InlineData("browser-based application")]
        [InlineData("desktop application")]
        [InlineData("mobile or tablet application")]
        public void ApplicationTypeDetails_AllFieldsDisplayed(string rowHeader)
        {
            PublicBrowsePages.SolutionAction.GetTableRowContent(rowHeader).Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ApplicationTypeDetails_SolutionIsSuspended_Redirect()
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
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            context.CatalogueItems.Single(ci => ci.Id == SolutionId).PublishedStatus = PublicationStatus.Published;
            context.SaveChanges();
        }
    }
}
