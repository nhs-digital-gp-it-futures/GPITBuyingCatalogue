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
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using OpenQA.Selenium;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
    [Collection(nameof(SharedContextCollection))]
    public sealed class AdditionalServicesDetails : AnonymousTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public AdditionalServicesDetails(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
            : base(
                  factory,
                  typeof(SolutionsController),
                  nameof(SolutionsController.AdditionalServices),
                  Parameters,
                  testOutputHelper)
        {
        }

        [Fact]
        public async Task AdditionalServicesDetails_AdditionalServicesNameDisplayedAsync()
        {
            await RunTestAsync(async () =>
            {
                await using var context = GetEndToEndDbContext();
                var pageTitle = (await context.CatalogueItems.FirstAsync(s => s.Id == new CatalogueItemId(99999, "001"))).Name;
                CommonActions.PageTitle()
                    .Should()
                    .BeEquivalentTo($"additional services - {pageTitle}".FormatForComparison());
            });
        }

        [Fact]
        public void AdditionalServicesDetail_AdditionalServicesTableDisplayed()
        {
            RunTest(() =>
            {
                CommonActions.ElementIsDisplayed(SolutionObjects.AdditionalServicesTieredTable).Should().BeTrue();
                CommonActions.ElementIsDisplayed(SolutionObjects.AdditionalServicesFlatTable).Should().BeTrue();
            });
        }

        [Fact]
        public async Task AdditionalServicesDetail_AdditionalServicesListedInTable()
        {
            await RunTestAsync(async () =>
            {
                await using var context = GetEndToEndDbContext();
                var additionalServicesInDb = await context.CatalogueItems
                .Where(c => c.CatalogueItemType == CatalogueItemType.AdditionalService)
                .Where(c => c.SupplierId == 99999)
                    .ToListAsync();

                var additionalServicesInTable = Driver.FindElement(SolutionObjects.AdditionalServicesTieredTable).FindElements(By.TagName("a"))
                    .Concat(Driver.FindElement(SolutionObjects.AdditionalServicesFlatTable).FindElements(By.TagName("a"))).Select(s => s.Text);
                additionalServicesInTable.Should().BeEquivalentTo(additionalServicesInDb.Select(s => s.Name));
            });
        }

        [Fact]
        public async Task AdditionalServicesDetails_AdditionalServicesDescriptionListed()
        {
            await RunTestAsync(async () =>
            {
                await using var context = GetEndToEndDbContext();
                var additionalDescInDb = await context.CatalogueItems
                .Include(c => c.AdditionalService).Where(c => c.CatalogueItemType == CatalogueItemType.AdditionalService)
                .Where(c => c.SupplierId == 99999)
                .Select(a => a.AdditionalService.FullDescription)
                .ToListAsync();

                var additionalDescOnPage = PublicBrowsePages.SolutionAction.GetAdditionalServicesDescription();
                additionalDescOnPage.Should().BeEquivalentTo(additionalDescInDb);
            });
        }

        [Fact]
        public async Task AdditionalServicesDetails_SolutionIsSuspended_Redirect()
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
