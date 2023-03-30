using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin
{
    [Collection(nameof(AdminCollection))]
    public sealed class ManageCatalogueSolution : AuthorityTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");
        private static readonly CatalogueItemId UnpublishedSolutionId = new(99999, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        private static readonly Dictionary<string, string> UnpublishedSolutionParameters = new()
        {
            { nameof(SolutionId), UnpublishedSolutionId.ToString() },
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
            (await context.CatalogueItems.FirstAsync(c => c.Id == SolutionId)).PublishedStatus = publicationStatus;
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
        public void Publish_IncompleteSections_ThrowsError()
        {
            NavigateToUrl(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ManageCatalogueSolution),
                UnpublishedSolutionParameters);

            CommonActions.ClickRadioButtonWithText(PublicationStatus.Published.Description());

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed()
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryLinksExist()
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                ManageCatalogueSolutionObjects.PublicationStatusInputError,
                "Complete all mandatory sections before publishing");
        }

        [Fact]
        public async Task Publish_CompleteSections_NoError()
        {
            await using var context = GetEndToEndDbContext();
            var supplierContact = context.SupplierContacts.First();
            var catalogueItem = context.CatalogueItems.First(c => c.Id == SolutionId);
            catalogueItem.CatalogueItemContacts.Add(supplierContact);
            catalogueItem.PublishedStatus = PublicationStatus.Unpublished;

            await context.SaveChangesAsync();

            Driver.Navigate().Refresh();

            CommonActions.ClickRadioButtonWithText(PublicationStatus.Published.Description());

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.Index))
                .Should()
                .BeTrue();

            CommonActions.ElementExists(CommonSelectors.NhsErrorSection)
                 .Should()
                 .BeFalse();

            var updatedCatalogueItem = context.CatalogueItems.AsNoTracking().First(c => c.Id == SolutionId);
            updatedCatalogueItem.PublishedStatus.Should().Be(PublicationStatus.Published);
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            context.CatalogueItems.First(ci => ci.Id == SolutionId).PublishedStatus = PublicationStatus.Published;
            context.SaveChanges();
        }
    }
}
