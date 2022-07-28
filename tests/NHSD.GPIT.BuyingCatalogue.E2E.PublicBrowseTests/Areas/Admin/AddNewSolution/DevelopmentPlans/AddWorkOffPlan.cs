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
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.DevelopmentPlans;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.DevelopmentPlans
{
    public sealed class AddWorkOffPlan : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
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

            CommonActions.ClickSave();
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

            using var context = GetEndToEndDbContext();
            var workOffPlan = await context.WorkOffPlans.FirstOrDefaultAsync(wp => wp.SolutionId == SolutionId && wp.Details == details);

            workOffPlan.StandardId.Should().BeEquivalentTo(standardId);
            workOffPlan.Details.Should().BeEquivalentTo(details);
            workOffPlan.CompletionDate.Date.Should().Be(agreedDate.Date);
        }

        [Fact]
        public void AddWorkOffPlan_InvalidModel_ErrorsThrown()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DevelopmentPlansController),
                nameof(DevelopmentPlansController.AddWorkOffPlan))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(DevelopmentPlanObjects.SelectStandardsError)
              .Should()
              .BeTrue();

            CommonActions
                .ElementIsDisplayed(DevelopmentPlanObjects.DetailsError)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsDisplayed(DevelopmentPlanObjects.AgreeCompletionDateError)
                .Should()
                .BeTrue();
        }
    }
}
