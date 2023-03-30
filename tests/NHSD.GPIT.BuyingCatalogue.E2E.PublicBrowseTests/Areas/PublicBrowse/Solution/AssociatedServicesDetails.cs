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
    public sealed class AssociatedServicesDetails : AnonymousTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public AssociatedServicesDetails(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
            : base(
                  factory,
                  typeof(SolutionsController),
                  nameof(SolutionsController.AssociatedServices),
                  Parameters,
                  testOutputHelper)
        {
        }

        [Fact]
        public async Task AssociatedServicesDetails_AssociatedServicesNameDisplayedAsync()
        {
            await RunTestAsync(async () =>
            {
                await using var context = GetEndToEndDbContext();
                var pageTitle = (await context.CatalogueItems.FirstAsync(s => s.Id == new CatalogueItemId(99999, "001"))).Name;

                CommonActions
                    .PageTitle()
                    .Should()
                    .BeEquivalentTo($"associated services - {pageTitle}".FormatForComparison());
            });
        }

        [Fact]
        public void AssociatedServicesDetails_AssociatedServicesTableDisplayed()
        {
            RunTest(() =>
            {
                CommonActions.ElementIsDisplayed(SolutionObjects.AssociatedServicesTieredTable).Should().BeTrue();
                CommonActions.ElementIsDisplayed(SolutionObjects.AssociatedServicesFlatTable).Should().BeTrue();
            });
        }

        [Fact]
        public async Task AssociatedServicesDetails_AssociatedServicesListedInTable()
        {
            await RunTestAsync(async () =>
            {
                await using var context = GetEndToEndDbContext();

                var associatedServicesInDb = await context.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AssociatedService).Where(c => c.SupplierId == 99999).ToListAsync();

                var solution = await context.CatalogueItems.Include(s => s.SupplierServiceAssociations).FirstAsync(s => s.Id == SolutionId);
                associatedServicesInDb = associatedServicesInDb.Where(x => solution.SupplierServiceAssociations.Any(y => y.AssociatedServiceId == x.Id)).ToList();

                var associatedServicesInTable = Driver.FindElement(SolutionObjects.AssociatedServicesTieredTable).FindElements(By.TagName("a"))
                    .Concat(Driver.FindElement(SolutionObjects.AssociatedServicesFlatTable).FindElements(By.TagName("a"))).Select(s => s.Text);

                associatedServicesInTable.Should().BeEquivalentTo(associatedServicesInDb.Select(s => s.Name));
            });
        }

        [Fact]
        public async Task AssociatedServicesDetails_AssociatedServicesDetailsListed()
        {
            await RunTestAsync(async () =>
            {
                await using var context = GetEndToEndDbContext();

                var associatedServicesInDb = await context.CatalogueItems
                    .Include(s => s.AssociatedService)
                    .Where(c => c.CatalogueItemType == CatalogueItemType.AssociatedService)
                    .Where(c => c.SupplierId == 99999)
                    .ToListAsync();

                var solution = await context.CatalogueItems.Include(s => s.SupplierServiceAssociations).FirstAsync(s => s.Id == SolutionId);
                associatedServicesInDb = associatedServicesInDb.Where(x => solution.SupplierServiceAssociations.Any(y => y.AssociatedServiceId == x.Id)).ToList();

                var associatedServicesOnPage = PublicBrowsePages.SolutionAction.GetAssociatedServicesInfo();

                associatedServicesOnPage.Should().BeEquivalentTo(
                    associatedServicesInDb.Select(s => s.AssociatedService),
                    options =>
                        options.Including(s => s.Description)
                        .Including(s => s.OrderGuidance));
            });
        }

        [Fact]
        public async Task AssociatedServicesDetails_SolutionIsSuspended_Redirect()
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
