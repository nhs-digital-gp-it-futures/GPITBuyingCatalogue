using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.DevelopmentPlans
{
    [Collection(nameof(AdminCollection))]
    public sealed class AddWorkOffPlan : AuthorityTestBase
    {
        private const string NoStandardSelectedError = "Select a Standard";
        private const string NoDetailsError = "Enter Work-off Plan item details";
        private const string NoDateDayError = "Error: Agreed completion date must include a day";
        private const string NoDateMonthError = "Error: Agreed completion date must include a month";
        private const string NoDateYearError = "Error: Agreed completion date must include a year";
        private const string DateErrorYearSize = "Error: Year must be four numbers";
        private const string DateIncorrectFormatError = "Error: Enter an agreed completion date in a valid format";
        private const string DuplicateWorkOffPlanError = "A Work-Off Plan with these details already exists";
        private const string DateInPastError = "Error: Agreed completion date must be in the future or within the last 12 weeks";

        private static readonly CatalogueItemId SolutionId = new(99998, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public AddWorkOffPlan(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(DevelopmentPlansController),
                  nameof(DevelopmentPlansController.AddWorkOffPlan),
                  Parameters)
        {
        }

        [Fact]
        public void AddWorkOffPlan_DisplayedCorrectly()
        {
            CommonActions.ElementIsDisplayed(DevelopmentPlanObjects.SelectStandards).Should().BeTrue();
            CommonActions.ElementIsDisplayed(DevelopmentPlanObjects.Details).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Common.CommonSelectors.DateDay).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Common.CommonSelectors.DateMonth).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Common.CommonSelectors.DateYear).Should().BeTrue();

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void AddWorkOffPlan_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DevelopmentPlansController),
                nameof(DevelopmentPlansController.DevelopmentPlans))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AddWorkOffPlan_NoInput_ErrorThrown()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DevelopmentPlansController),
                nameof(DevelopmentPlansController.AddWorkOffPlan))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions
                .ElementShowingCorrectErrorMessage(DevelopmentPlanObjects.SelectStandardsError, NoStandardSelectedError)
                .Should()
                .BeTrue();

            CommonActions
                .ElementShowingCorrectErrorMessage(DevelopmentPlanObjects.DetailsError, NoDetailsError)
                .Should()
                .BeTrue();

            CommonActions
                .ElementShowingCorrectErrorMessage(DevelopmentPlanObjects.AgreeCompletionDateError, NoDateDayError)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AddWorkOffPlan_NoMonth_ErrorThrown()
        {
            CommonActions.ElementAddValue(Objects.Common.CommonSelectors.DateDay, "1");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DevelopmentPlansController),
                nameof(DevelopmentPlansController.AddWorkOffPlan))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions
                .ElementShowingCorrectErrorMessage(DevelopmentPlanObjects.AgreeCompletionDateError, NoDateMonthError)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AddWorkOffPlan_NoYear_ErrorThrown()
        {
            CommonActions.ElementAddValue(Objects.Common.CommonSelectors.DateDay, "1");
            CommonActions.ElementAddValue(Objects.Common.CommonSelectors.DateMonth, "1");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DevelopmentPlansController),
                nameof(DevelopmentPlansController.AddWorkOffPlan))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions
                .ElementShowingCorrectErrorMessage(DevelopmentPlanObjects.AgreeCompletionDateError, NoDateYearError)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AddWorkOffPlan_YearNotLength4_ErrorThrown()
        {
            CommonActions.ElementAddValue(Objects.Common.CommonSelectors.DateDay, "1");
            CommonActions.ElementAddValue(Objects.Common.CommonSelectors.DateMonth, "1");
            CommonActions.ElementAddValue(Objects.Common.CommonSelectors.DateYear, "1");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DevelopmentPlansController),
                nameof(DevelopmentPlansController.AddWorkOffPlan))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions
                .ElementShowingCorrectErrorMessage(DevelopmentPlanObjects.AgreeCompletionDateError, DateErrorYearSize)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AddWorkOffPlan_IncorrectFormat_ErrorThrown()
        {
            CommonActions.ElementAddValue(Objects.Common.CommonSelectors.DateDay, "50");
            CommonActions.ElementAddValue(Objects.Common.CommonSelectors.DateMonth, "13");
            CommonActions.ElementAddValue(Objects.Common.CommonSelectors.DateYear, DateTime.UtcNow.Year.ToString());

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DevelopmentPlansController),
                nameof(DevelopmentPlansController.AddWorkOffPlan))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions
                .ElementShowingCorrectErrorMessage(DevelopmentPlanObjects.AgreeCompletionDateError, DateIncorrectFormatError)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AddWorkOffPlan_DateTooFarInPast_ErrorThrown()
        {
            CommonActions.ElementAddValue(Objects.Common.CommonSelectors.DateDay, DateTime.UtcNow.Day.ToString());
            CommonActions.ElementAddValue(Objects.Common.CommonSelectors.DateMonth, DateTime.UtcNow.Month.ToString());
            CommonActions.ElementAddValue(Objects.Common.CommonSelectors.DateYear, DateTime.UtcNow.AddYears(-1).Year.ToString());

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DevelopmentPlansController),
                nameof(DevelopmentPlansController.AddWorkOffPlan))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions
                .ElementShowingCorrectErrorMessage(DevelopmentPlanObjects.AgreeCompletionDateError, DateInPastError)
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task AddWorkOffPlan_DuplicateWorkOffPlan_ErrorThrown()
        {
            await using var context = GetEndToEndDbContext();
            var workOffPlan = await context.WorkOffPlans.FirstOrDefaultAsync(wp => wp.SolutionId == SolutionId);

            CommonActions.SelectDropDownItemByValue(DevelopmentPlanObjects.SelectStandards, workOffPlan.StandardId);
            CommonActions.ElementAddValue(DevelopmentPlanObjects.Details, workOffPlan.Details);
            TextGenerators.DateInputAddDateSoon(
                Objects.Common.CommonSelectors.DateDay,
                Objects.Common.CommonSelectors.DateMonth,
                Objects.Common.CommonSelectors.DateYear);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DevelopmentPlansController),
                nameof(DevelopmentPlansController.AddWorkOffPlan))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions
                .ElementShowingCorrectErrorMessage(DevelopmentPlanObjects.SelectStandardsError, DuplicateWorkOffPlanError)
                .Should()
                .BeTrue();

            CommonActions
                .ElementShowingCorrectErrorMessage(DevelopmentPlanObjects.DetailsError, DuplicateWorkOffPlanError)
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task AddWorkOffPlan_CorrectInput_ExpectedResult()
        {
            var standardId = CommonActions.SelectDropDownItem(DevelopmentPlanObjects.SelectStandards, 1);
            var details = TextGenerators.TextInputAddText(DevelopmentPlanObjects.Details, 300);
            var agreedDate = TextGenerators.DateInputAddDateSoon(
                                Objects.Common.CommonSelectors.DateDay,
                                Objects.Common.CommonSelectors.DateMonth,
                                Objects.Common.CommonSelectors.DateYear);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DevelopmentPlansController),
                nameof(DevelopmentPlansController.DevelopmentPlans))
                .Should()
                .BeTrue();

            await using var context = GetEndToEndDbContext();
            var workOffPlan = await context.WorkOffPlans.FirstOrDefaultAsync(wp => wp.SolutionId == SolutionId && wp.Details == details);

            workOffPlan.StandardId.Should().BeEquivalentTo(standardId);
            workOffPlan.Details.Should().BeEquivalentTo(details);
            workOffPlan.CompletionDate.Date.Should().Be(agreedDate.Date);
        }
    }
}
