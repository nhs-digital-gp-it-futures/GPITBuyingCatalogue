using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ServiceLevelAgreements
{
    [Collection(nameof(AdminCollection))]
    public sealed class EditServiceAvailabilityTimes : AuthorityTestBase, IDisposable
    {
        private const int SingleAvailabilityTimesId = 1;
        private const int ServiceAvailabilityTimesId = 2;

        private static readonly CatalogueItemId SingleAvailabilityTimeSolutionId = new(99998, "001");
        private static readonly CatalogueItemId SolutionId = new(99998, "002");

        private static readonly Dictionary<string, string> SingleAvailabilityTimeParameters = new()
        {
            { nameof(SolutionId), SingleAvailabilityTimeSolutionId.ToString() },
            { nameof(ServiceAvailabilityTimesId), SingleAvailabilityTimesId.ToString() },
        };

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(ServiceAvailabilityTimesId), ServiceAvailabilityTimesId.ToString() },
        };

        public EditServiceAvailabilityTimes(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(ServiceLevelAgreementsController),
                  nameof(ServiceLevelAgreementsController.EditServiceAvailabilityTimes),
                  Parameters)
        {
        }

        [Fact]
        public async Task EditAvailabilityTimes_CorrectlyDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var solution = await context.CatalogueItems.FirstAsync(ci => ci.Id == SolutionId);

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Service availability times - {solution.Name}".FormatForComparison());

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ServiceAvailabilityTimesObjects.SupportTypeInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ServiceAvailabilityTimesObjects.FromInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ServiceAvailabilityTimesObjects.UntilInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ServiceAvailabilityTimesObjects.ApplicableDaysInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ServiceAvailabilityTimesObjects.DeleteLink).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void EditAvailabilityTimes_ClickGoBackLink()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreement))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void EditAvailabilityTimes_SingleAvailabilityTime_DeleteLinkHidden()
        {
            NavigateToUrl(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceAvailabilityTimes),
                SingleAvailabilityTimeParameters);

            CommonActions.ElementExists(ServiceAvailabilityTimesObjects.DeleteLink).Should().BeFalse();
        }

        [Fact]
        public async Task EditAvailabilityTimes_UnpublishedSolutionSingleAvailabilityTime_DeleteLinkVisible()
        {
            await using var context = GetEndToEndDbContext();
            var catalogueItem = await context.CatalogueItems.FirstAsync(c => c.Id == SingleAvailabilityTimeSolutionId);
            catalogueItem.PublishedStatus = PublicationStatus.Unpublished;
            await context.SaveChangesAsync();

            NavigateToUrl(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceAvailabilityTimes),
                SingleAvailabilityTimeParameters);

            CommonActions.ElementExists(ServiceAvailabilityTimesObjects.DeleteLink).Should().BeTrue();
        }

        [Fact]
        public async Task EditAvailabilityTimes_ClickSave_Valid()
        {
            var currentDate = DateTime.UtcNow;

            var fromTime = currentDate.AddMinutes(-5).ToString("HH:mm");
            var untilTime = currentDate.ToString("HH:mm");

            var supportType = TextGenerators.TextInputAddText(ServiceAvailabilityTimesObjects.SupportTypeInput, 20);
            var applicableDays = TextGenerators.TextInputAddText(ServiceAvailabilityTimesObjects.ApplicableDaysInput, 20);

            CommonActions.ElementAddValue(ServiceAvailabilityTimesObjects.FromInput, fromTime);
            CommonActions.ElementAddValue(ServiceAvailabilityTimesObjects.UntilInput, untilTime);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreement))
                .Should()
                .BeTrue();

            await using var context = GetEndToEndDbContext();
            var serviceLevelAvailabilityTime = await context.ServiceAvailabilityTimes.FirstOrDefaultAsync(t => t.SolutionId == SolutionId && t.Id == ServiceAvailabilityTimesId);

            serviceLevelAvailabilityTime.Category.Should().Be(supportType);
            serviceLevelAvailabilityTime.ApplicableDays.Should().Be(applicableDays);
            serviceLevelAvailabilityTime.TimeFrom.ToString("HH:mm").Should().Be(fromTime);
            serviceLevelAvailabilityTime.TimeUntil.ToString("HH:mm").Should().Be(untilTime);
        }

        [Fact]
        public void EditAvailabilityTimes_ClickSave_NoInput()
        {
            CommonActions.ClearInputElement(ServiceAvailabilityTimesObjects.SupportTypeInput);
            CommonActions.ClearInputElement(ServiceAvailabilityTimesObjects.ApplicableDaysInput);
            CommonActions.ClearInputElement(ServiceAvailabilityTimesObjects.FromInput);
            CommonActions.ClearInputElement(ServiceAvailabilityTimesObjects.UntilInput);

            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed()
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryLinksExist()
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(ServiceAvailabilityTimesObjects.SupportTypeInputError, "Enter a type of support")
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(ServiceAvailabilityTimesObjects.ApplicableDaysInputError, "Enter the applicable days")
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(ServiceAvailabilityTimesObjects.TimeInputError, "Error: Enter a from time")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void EditAvailabilityTimes_ClickSave()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreement))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void EditAvailabilityTimes_ClickSave_InvalidFromFormat()
        {
            var currentDate = DateTime.UtcNow;

            var fromTime = currentDate.AddMinutes(-5).ToString("HH");
            var untilTime = currentDate.ToString("HH:mm");

            TextGenerators.TextInputAddText(ServiceAvailabilityTimesObjects.SupportTypeInput, 20);
            TextGenerators.TextInputAddText(ServiceAvailabilityTimesObjects.ApplicableDaysInput, 20);

            CommonActions.ElementAddValue(ServiceAvailabilityTimesObjects.FromInput, fromTime);
            CommonActions.ElementAddValue(ServiceAvailabilityTimesObjects.UntilInput, untilTime);

            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed()
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryLinksExist()
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(ServiceAvailabilityTimesObjects.TimeInputError, $"Error: Enter time in the correct format")
                 .Should()
                 .BeTrue();
        }

        [Fact]
        public void EditAvailabilityTimes_ClickSave_InvalidUntilFormat()
        {
            var currentDate = DateTime.UtcNow;

            var fromTime = currentDate.AddMinutes(-5).ToString("HH:mm");
            var untilTime = currentDate.ToString("HH");

            TextGenerators.TextInputAddText(ServiceAvailabilityTimesObjects.SupportTypeInput, 20);
            TextGenerators.TextInputAddText(ServiceAvailabilityTimesObjects.ApplicableDaysInput, 20);

            CommonActions.ElementAddValue(ServiceAvailabilityTimesObjects.FromInput, fromTime);
            CommonActions.ElementAddValue(ServiceAvailabilityTimesObjects.UntilInput, untilTime);

            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed()
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryLinksExist()
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(ServiceAvailabilityTimesObjects.TimeInputError, $"Error: Enter time in the correct format")
                 .Should()
                 .BeTrue();
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            var solution = context.CatalogueItems.First(c => c.Id == SingleAvailabilityTimeSolutionId);

            if (solution is not null && solution.PublishedStatus != PublicationStatus.Published)
            {
                solution.PublishedStatus = PublicationStatus.Published;
                context.SaveChanges();
            }
        }
    }
}
