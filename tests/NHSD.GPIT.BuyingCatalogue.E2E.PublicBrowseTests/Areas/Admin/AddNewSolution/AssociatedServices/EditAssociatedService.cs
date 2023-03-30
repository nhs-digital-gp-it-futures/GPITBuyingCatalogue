using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution
{
    [Collection(nameof(AdminCollection))]
    public sealed class EditAssociatedService : AuthorityTestBase, IDisposable
    {
        private static readonly CatalogueItemId IncompleteSolutionId = new(99999, "001");
        private static readonly CatalogueItemId IncompleteAssociatedServiceId = new(99999, "S-998");

        private static readonly CatalogueItemId SolutionId = new(99998, "001");
        private static readonly CatalogueItemId AssociatedServiceId = new(99998, "S-999");

        private static readonly Dictionary<string, string> IncompleteServiceParameters = new()
        {
            { nameof(SolutionId), IncompleteSolutionId.ToString() },
            { nameof(AssociatedServiceId), IncompleteAssociatedServiceId.ToString() },
        };

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(AssociatedServiceId), AssociatedServiceId.ToString() },
        };

        public EditAssociatedService(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AssociatedServicesController),
                  nameof(AssociatedServicesController.EditAssociatedService),
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
        public async Task EditAssociatedService_CorrectlyDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var supplierName = (await context.CatalogueItems.Include(ci => ci.Supplier).FirstAsync(s => s.Id == SolutionId)).Supplier.Name;
            var associatedServiceName = (await context.CatalogueItems.FirstAsync(s => s.Id == AssociatedServiceId)).Name;

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"{associatedServiceName} information - {supplierName}".FormatForComparison());

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.CommonObjects.SaveButton).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AssociatedServicesObjects.AssociatedServiceDashboardTable).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AssociatedServicesObjects.AssociatedServiceRelatedSolutionsTable).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AssociatedServicesObjects.AssociatedServiceRelatedSolutionsInset).Should().BeFalse();
        }

        [Fact]
        public void EditAssociatedService_ClickGoBackLink_NavigatesToCorrectPage()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AssociatedServicesController),
                    nameof(AssociatedServicesController.AssociatedServices))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void EditAssociatedService_ClickContinueButton_NavigatesToCorrectPage()
        {
            CommonActions.ClickLinkElement(Objects.Admin.CommonObjects.SaveButton);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AssociatedServicesController),
                    nameof(AssociatedServicesController.AssociatedServices))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void EditAssociatedService_EditServiceAssociation_NavigatesToCorrectPage()
        {
            using var context = GetEndToEndDbContext();
            var associatedService = context.AssociatedServices.First(a => a.CatalogueItemId == AssociatedServiceId);
            var solution = context.Solutions.Include(s => s.CatalogueItem).ThenInclude(c => c.AssociatedService).First();
            solution.CatalogueItem.SupplierServiceAssociations = new HashSet<SupplierServiceAssociation> { new(solution.CatalogueItemId, AssociatedServiceId) };

            context.SaveChanges();

            Driver.Navigate().Refresh();

            CommonActions.ClickLinkElement(ByExtensions.DataTestId($"edit-service-association-{solution.CatalogueItemId}"));

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.AssociatedServices))
                .Should().BeTrue();

            solution.CatalogueItem.SupplierServiceAssociations.Clear();
            context.SaveChanges();
        }

        [Fact]
        public void EditAssociatedService_WithServiceAssociations_TableIsDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var associatedService = context.AssociatedServices.First(a => a.CatalogueItemId == AssociatedServiceId);
            var solution = context.Solutions.Include(s => s.CatalogueItem).ThenInclude(c => c.SupplierServiceAssociations).First();
            solution.CatalogueItem.SupplierServiceAssociations = new HashSet<SupplierServiceAssociation> { new(solution.CatalogueItemId, AssociatedServiceId) };

            context.SaveChanges();

            Driver.Navigate().Refresh();

            CommonActions.ElementIsDisplayed(AssociatedServicesObjects.AssociatedServiceRelatedSolutionsTable).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AssociatedServicesObjects.AssociatedServiceRelatedSolutionsInset).Should().BeFalse();

            solution.CatalogueItem.SupplierServiceAssociations.Clear();
            context.SaveChanges();
        }

        [Fact]
        public void AddAssociatedService_Unpublish_ActiveSolutions_ValidationError()
        {
            using var context = GetEndToEndDbContext();
            var associatedService = context.AssociatedServices.First(a => a.CatalogueItemId == AssociatedServiceId);
            var solution = context.Solutions.Include(s => s.CatalogueItem).ThenInclude(c => c.AssociatedService).First(s => s.CatalogueItemId != SolutionId);
            solution.CatalogueItem.SupplierServiceAssociations = new HashSet<SupplierServiceAssociation> { new(solution.CatalogueItemId, AssociatedServiceId) };
            solution.CatalogueItem.PublishedStatus = PublicationStatus.Published;

            context.SaveChanges();

            Driver.Navigate().Refresh();

            CommonActions.ClickRadioButtonWithValue(PublicationStatus.Unpublished.ToString());

            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();

            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                AssociatedServicesObjects.PublicationStatusInputError,
                "Error: This Associated Service cannot be unpublished as it is referenced by at least one solution")
                .Should()
                .BeTrue();

            solution.CatalogueItem.SupplierServiceAssociations.Clear();
            context.SaveChanges();
        }

        [Theory]
        [MemberData(nameof(PublicationStatusesTestCases))]
        public async Task EditAssociatedService_DisplaysPublicationStatuses(
            PublicationStatus publicationStatus,
            PublicationStatus[] expectedPublicationStatuses)
        {
            await using var context = GetEndToEndDbContext();
            (await context.CatalogueItems.FirstAsync(c => c.Id == AssociatedServiceId)).PublishedStatus = publicationStatus;
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
        public async Task Publish_IncompleteSections_ThrowsError()
        {
            await using var context = GetEndToEndDbContext();
            var item = await context.CatalogueItems.FirstAsync(c => c.CatalogueItemType == CatalogueItemType.AssociatedService && c.Id == IncompleteAssociatedServiceId);
            var prices = await context.CataloguePrices.Where(cp => cp.CatalogueItemId == IncompleteAssociatedServiceId).ToListAsync();
            prices.ForEach(p => p.PublishedStatus = PublicationStatus.Unpublished);
            item.PublishedStatus = PublicationStatus.Draft;
            await context.SaveChangesAsync();

            NavigateToUrl(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.EditAssociatedService),
                IncompleteServiceParameters);

            CommonActions.ClickRadioButtonWithText(PublicationStatus.Published.Description());

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.EditAssociatedService))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed()
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryLinksExist()
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                AssociatedServicesObjects.PublicationStatusInputError,
                "Complete all mandatory sections before publishing");
        }

        [Fact]
        public async Task Publish_CompleteSections_SetPublicationStatus()
        {
            await using var context = GetEndToEndDbContext();
            (await context.CatalogueItems.FirstAsync(c => c.Id == AssociatedServiceId)).PublishedStatus = PublicationStatus.Draft;
            await context.SaveChangesAsync();

            Driver.Navigate().Refresh();

            CommonActions
                .ClickRadioButtonWithText(PublicationStatus.Published.Description());

            CommonActions.ClickSave();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(AssociatedServicesController),
                    nameof(AssociatedServicesController.AssociatedServices))
                .Should()
                .BeTrue();

            await using var updatedContext = GetEndToEndDbContext();
            var publishedStatus = (await updatedContext.CatalogueItems.FirstAsync(c => c.Id == AssociatedServiceId)).PublishedStatus;
            publishedStatus
                .Should()
                .Be(PublicationStatus.Published);
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            context.CatalogueItems.First(ci => ci.Id == AssociatedServiceId).PublishedStatus = PublicationStatus.Published;
            context.SaveChanges();
        }
    }
}
