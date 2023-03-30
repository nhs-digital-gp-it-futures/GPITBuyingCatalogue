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
    public sealed class EditWorkOffPlan : AuthorityTestBase, IDisposable
    {
        private const int WorkOffPlanId = 1;
        private static readonly CatalogueItemId SolutionId = new(99998, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(WorkOffPlanId), WorkOffPlanId.ToString() },
        };

        public EditWorkOffPlan(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(DevelopmentPlansController),
                  nameof(DevelopmentPlansController.EditWorkOffPlan),
                  Parameters)
        {
        }

        [Fact]
        public void EditWorkOffPlan_DisplayedCorrectly()
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
        public void EditWorkOffPlan_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DevelopmentPlansController),
                nameof(DevelopmentPlansController.DevelopmentPlans))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task EditWorkOffPlan_ValidSave_ExpectedResult()
        {
            var standardId = CommonActions.SelectDropDownItem(DevelopmentPlanObjects.SelectStandards, 2);
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
            var workOffPlan = await context.WorkOffPlans.FirstOrDefaultAsync(wp => wp.Id == WorkOffPlanId);

            workOffPlan.StandardId.Should().BeEquivalentTo(standardId);
            workOffPlan.Details.Should().BeEquivalentTo(details);
            workOffPlan.CompletionDate.Date.Should().Be(agreedDate.Date);
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            var workOffPlan = context.WorkOffPlans.FirstOrDefaultAsync(wp => wp.Id == WorkOffPlanId).Result;

            workOffPlan.StandardId = "S1";
            workOffPlan.Details = "Some Details";

            context.SaveChanges();
        }
    }
}
