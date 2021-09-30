using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin
{
    public sealed class ManageCatalogueSolution : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public ManageCatalogueSolution(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CatalogueSolutionsController),
                  nameof(CatalogueSolutionsController.ManageCatalogueSolution),
                  Parameters)
        {
        }

        public static IEnumerable<object[]> PublicationStatusesTestCases()
            => new object[][]
            {
                new object[]
                {
                    PublicationStatus.Draft,
                    new[]
                    {
                       PublicationStatus.Draft,
                       PublicationStatus.Published,
                    },
                },
                new object[]
                {
                    PublicationStatus.Published,
                    new[]
                    {
                       PublicationStatus.Published,
                       PublicationStatus.InRemediation,
                       PublicationStatus.Unpublished,
                    },
                },
                new object[]
                {
                    PublicationStatus.InRemediation,
                    new[]
                    {
                       PublicationStatus.Published,
                       PublicationStatus.InRemediation,
                       PublicationStatus.Suspended,
                       PublicationStatus.Unpublished,
                    },
                },
                new object[]
                {
                    PublicationStatus.Suspended,
                    new[]
                    {
                       PublicationStatus.Published,
                       PublicationStatus.InRemediation,
                       PublicationStatus.Suspended,
                       PublicationStatus.Unpublished,
                    },
                },
                new object[]
                {
                    PublicationStatus.Unpublished,
                    new[]
                    {
                       PublicationStatus.Published,
                       PublicationStatus.Unpublished,
                    },
                },
            };

        [Fact]
        public void ManageCatalogueSolution_AllElementsDisplayed()
        {
            CommonActions
                .ElementIsDisplayed(ManageCatalogueSolutionObjects.CatalogueSolutionDashboardTable)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsDisplayed(ManageCatalogueSolutionObjects.PublicationStatusInput)
                .Should()
                .BeTrue();

            CommonActions
                .SaveButtonDisplayed()
                .Should()
                .BeTrue();
        }

        [Theory]
        [MemberData(nameof(PublicationStatusesTestCases))]
        public async Task ManageCatalogueSolution_DisplaysPublicationStatuses(
            PublicationStatus publicationStatus,
            PublicationStatus[] expectedPublicationStatuses)
        {
            await using var context = GetEndToEndDbContext();
            (await context.CatalogueItems.SingleAsync(c => c.Id == SolutionId)).PublishedStatus = publicationStatus;
            await context.SaveChangesAsync();

            Driver.Navigate().Refresh();

            CommonActions
                .GetNumberOfRadioButtonsDisplayed()
                .Should()
                .Be(expectedPublicationStatuses.Length);

            CommonActions
                .GetRadioButtonsOptions()
                .Should()
                .BeEquivalentTo(expectedPublicationStatuses.Select(p => p.Description()));
        }

        [Fact]
        public async Task ManageCatalogueSolution_SetPublicationStatus()
        {
            await using (var context = GetEndToEndDbContext())
            {
                (await context.CatalogueItems.SingleAsync(c => c.Id == SolutionId)).PublishedStatus = PublicationStatus.Draft;
                await context.SaveChangesAsync();
            }

            Driver.Navigate().Refresh();

            CommonActions
                .ClickRadioButtonWithText(PublicationStatus.Published.Description());

            CommonActions.ClickSave();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(CatalogueSolutionsController),
                    nameof(CatalogueSolutionsController.Index))
                    .Should()
                .BeTrue();

            await using (var context = GetEndToEndDbContext())
            {
                var publishedStatus = (await context.CatalogueItems.SingleAsync(c => c.Id == SolutionId)).PublishedStatus;
                publishedStatus
                    .Should()
                    .Be(PublicationStatus.Published);
            }
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            context.CatalogueItems.Single(ci => ci.Id == SolutionId).PublishedStatus = PublicationStatus.Published;
            context.SaveChanges();
        }
    }
}
