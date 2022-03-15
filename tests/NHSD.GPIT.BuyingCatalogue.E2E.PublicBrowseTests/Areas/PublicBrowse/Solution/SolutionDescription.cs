using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using OpenQA.Selenium;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
    public sealed class SolutionDescription : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public SolutionDescription(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
            : base(
                  factory,
                  typeof(SolutionsController),
                  nameof(SolutionsController.Description),
                  Parameters,
                  testOutputHelper)
        {
        }

        [Fact]
        public async Task Description_InRemediationDisplayed()
        {
            await RunTestAsync(async () =>
            {
                await using var context = GetEndToEndDbContext();
                var solution = await context.CatalogueItems.SingleAsync(ci => ci.Id == SolutionId);
                solution.PublishedStatus = PublicationStatus.InRemediation;
                await context.SaveChangesAsync();

                Driver.Navigate().Refresh();

                CommonActions
                    .ElementExists(Objects.PublicBrowse.SolutionObjects.InRemediationNotice)
                    .Should()
                    .BeTrue();
            });
        }

        [Fact]
        public void Description_InRemediationNotDisplayed()
        {
            RunTest(() =>
            {
                CommonActions
                    .ElementExists(Objects.PublicBrowse.SolutionObjects.InRemediationNotice)
                    .Should()
                    .BeFalse();
            });
        }

        [Fact]
        public async Task Description_IsSuspended_SuspendedNoticeDisplayed()
        {
            await RunTestAsync(async () =>
            {
                await using var context = GetEndToEndDbContext();
                var solution = await context.CatalogueItems.SingleAsync(ci => ci.Id == SolutionId);
                solution.PublishedStatus = PublicationStatus.Suspended;
                await context.SaveChangesAsync();

                Driver.Navigate().Refresh();

                CommonActions
                    .ElementExists(Objects.PublicBrowse.SolutionObjects.SolutionSuspendedNotice)
                    .Should()
                    .BeTrue();

                CommonActions
                    .ElementExists(Objects.PublicBrowse.SolutionObjects.SolutionNavigationMenu)
                    .Should()
                    .BeFalse();

                CommonActions
                    .ElementExists(CommonSelectors.PaginationNext)
                    .Should()
                    .BeFalse();
            });
        }

        [Fact]
        public void Description_NotSuspended_SuspendedNoticeNotDisplayed()
        {
            RunTest(() =>
            {
                CommonActions
                    .ElementExists(Objects.PublicBrowse.SolutionObjects.SolutionSuspendedNotice)
                    .Should()
                    .BeFalse();

                CommonActions
                    .ElementExists(Objects.PublicBrowse.SolutionObjects.SolutionNavigationMenu)
                    .Should()
                    .BeTrue();

                CommonActions
                    .ElementExists(CommonSelectors.PaginationNext)
                    .Should()
                    .BeTrue();
            });
        }

        [Fact]
        public void Description_Link_DisplaysEmbeddedLink()
        {
            RunTest(() =>
            {
                using var context = GetEndToEndDbContext();
                var catalogueItem = context.CatalogueItems.Include(c => c.Solution).Single(c => c.Id == SolutionId);

                catalogueItem.Solution.AboutUrl = "https://www.fake.com";
                context.SaveChanges();

                Driver.Navigate().Refresh();

                CommonActions.ElementIsDisplayed(By.LinkText("More about this solution"))
                    .Should()
                    .BeTrue();

                context.SaveChanges();
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
