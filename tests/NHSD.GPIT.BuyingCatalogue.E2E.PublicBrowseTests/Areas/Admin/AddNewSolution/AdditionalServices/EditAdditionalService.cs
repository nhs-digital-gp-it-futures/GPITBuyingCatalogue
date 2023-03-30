using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.AdditionalServices
{
    [Collection(nameof(AdminCollection))]
    public sealed class EditAdditionalService : AuthorityTestBase, IDisposable
    {
        private static readonly CatalogueItemId IncompleteSolutionId = new(99999, "001");
        private static readonly CatalogueItemId IncompleteAdditionalServiceId = new(99999, "001A99");

        private static readonly CatalogueItemId SolutionId = new(99998, "001");
        private static readonly CatalogueItemId AdditionalServiceId = new(99998, "001A99");

        private static readonly Dictionary<string, string> IncompleteServiceParameters = new()
        {
            { nameof(SolutionId), IncompleteSolutionId.ToString() },
            { nameof(AdditionalServiceId), IncompleteAdditionalServiceId.ToString() },
        };

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
        };

        public EditAdditionalService(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AdditionalServicesController),
                  nameof(AdditionalServicesController.EditAdditionalService),
                  Parameters)
        {
        }

        [Fact]
        public async Task EditAdditionalService_CorrectlyDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var solutionName = (await context.CatalogueItems.FirstAsync(s => s.Id == SolutionId)).Name;
            var additionalServiceName = (await context.CatalogueItems.FirstAsync(s => s.Id == AdditionalServiceId)).Name;

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"{additionalServiceName} information - {solutionName}".FormatForComparison());

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AdditionalServicesObjects.AdditionalServicesTableDashboard).Should().BeTrue();
        }

        [Fact]
        public void EditAdditionalService_ClickGoBackLink_NavigatesToCorrectPage()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AdditionalServicesController),
                    nameof(AdditionalServicesController.Index))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task Publish_IncompleteSections_ThrowsError()
        {
            await using var context = GetEndToEndDbContext();
            var item = await context.CatalogueItems.FirstAsync(c => c.CatalogueItemType == CatalogueItemType.AdditionalService && c.Id == IncompleteAdditionalServiceId);
            var prices = await context.CataloguePrices.Where(cp => cp.CatalogueItemId == IncompleteAdditionalServiceId).ToListAsync();
            prices.ForEach(p => p.PublishedStatus = PublicationStatus.Unpublished);
            item.PublishedStatus = PublicationStatus.Draft;
            await context.SaveChangesAsync();

            NavigateToUrl(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.EditAdditionalService),
                IncompleteServiceParameters);

            CommonActions.ClickRadioButtonWithText(PublicationStatus.Published.Description());

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.EditAdditionalService))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed()
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryLinksExist()
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                AdditionalServicesObjects.PublicationStatusInputError,
                "Complete all mandatory sections before publishing");
        }

        [Fact]
        public async Task Publish_CompleteSections_NoError()
        {
            await using var context = GetEndToEndDbContext();
            var item = await context.CatalogueItems.FirstAsync(c => c.CatalogueItemType == CatalogueItemType.AdditionalService && c.Id == AdditionalServiceId);
            item.PublishedStatus = PublicationStatus.Draft;
            await context.SaveChangesAsync();

            Driver.Navigate().Refresh();

            CommonActions.ClickRadioButtonWithText(PublicationStatus.Published.Description());

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AdditionalServicesController),
                    nameof(AdditionalServicesController.Index))
                .Should()
                .BeTrue();

            await using var updatedContext = GetEndToEndDbContext();
            var publishedStatus = (await updatedContext.CatalogueItems.FirstAsync(c => c.Id == AdditionalServiceId)).PublishedStatus;
            publishedStatus
                .Should()
                .Be(PublicationStatus.Published);
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            var catalogueItems = context.CatalogueItems
                .Where(c => c.CatalogueItemType == CatalogueItemType.AdditionalService && new[] { AdditionalServiceId, IncompleteAdditionalServiceId }.Contains(c.Id))
                .ToList();

            catalogueItems.ForEach(c => c.PublishedStatus = PublicationStatus.Published);

            context.SaveChanges();
        }
    }
}
