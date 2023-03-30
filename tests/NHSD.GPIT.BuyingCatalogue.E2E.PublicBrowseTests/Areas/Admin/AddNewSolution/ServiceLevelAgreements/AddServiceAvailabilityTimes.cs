using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ServiceLevelAgreements
{
    [Collection(nameof(AdminCollection))]
    public sealed class AddServiceAvailabilityTimes : AuthorityTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99998, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public AddServiceAvailabilityTimes(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(ServiceLevelAgreementsController),
                  nameof(ServiceLevelAgreementsController.AddServiceAvailabilityTimes),
                  Parameters)
        {
        }

        [Fact]
        public async Task AddAvailabilityTimes_CorrectlyDisplayed()
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
            CommonActions.ElementExists(ServiceAvailabilityTimesObjects.DeleteLink).Should().BeFalse();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void AddAvailabilityTimes_ClickGoBackLink()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreement))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task AddAvailabilityTimes_ClickSave_Valid()
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
            var serviceLevelAvailabilityTime = await context.ServiceAvailabilityTimes.FirstOrDefaultAsync(t => t.SolutionId == SolutionId
                && string.Equals(t.Category, supportType)
                && string.Equals(t.ApplicableDays, applicableDays));

            serviceLevelAvailabilityTime.Should().NotBeNull();
        }

        [Fact]
        public void AddAvailabilityTimes_ClickSave_NoInput()
        {
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
        public async Task AddAvailabilityTimes_ClickSave_Duplicate()
        {
            await using var context = GetEndToEndDbContext();
            var serviceAvailabilityTime = await context.ServiceAvailabilityTimes.FirstAsync(t => t.Id == 1);

            CommonActions.ElementAddValue(ServiceAvailabilityTimesObjects.SupportTypeInput, serviceAvailabilityTime.Category);
            CommonActions.ElementAddValue(ServiceAvailabilityTimesObjects.ApplicableDaysInput, serviceAvailabilityTime.ApplicableDays);
            CommonActions.ElementAddValue(ServiceAvailabilityTimesObjects.FromInput, serviceAvailabilityTime.TimeFrom.ToString("HH:mm"));
            CommonActions.ElementAddValue(ServiceAvailabilityTimesObjects.UntilInput, serviceAvailabilityTime.TimeUntil.ToString("HH:mm"));

            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed()
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryLinksExist()
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(ServiceAvailabilityTimesObjects.SupportTypeInputError, "Service availability time with these details already exists")
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(ServiceAvailabilityTimesObjects.ApplicableDaysInputError, "Service availability time with these details already exists")
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(ServiceAvailabilityTimesObjects.TimeInputError, "Error: Service availability time with these details already exists")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AddAvailabilityTimes_ClickSave_InvalidFromFormat()
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
        public void AddAvailabilityTimes_ClickSave_InvalidUntilFormat()
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

            var serviceAvailabilityTimes = context.ServiceAvailabilityTimes.OrderByDescending(x => x.LastUpdated)
                .Where(x => x.SolutionId == SolutionId)
                .ToList();

            if (serviceAvailabilityTimes.Count <= 1)
                return;

            context.ServiceAvailabilityTimes.Remove(serviceAvailabilityTimes.First());
            context.SaveChanges();
        }
    }
}
