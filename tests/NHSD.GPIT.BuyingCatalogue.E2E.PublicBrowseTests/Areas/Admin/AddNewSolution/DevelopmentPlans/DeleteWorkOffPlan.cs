using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.DevelopmentPlans
{
    [Collection(nameof(AdminCollection))]
    public class DeleteWorkOffPlan : AuthorityTestBase
    {
        private const int WorkOffPlanId = 2;
        private static readonly CatalogueItemId SolutionId = new(99998, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(WorkOffPlanId), WorkOffPlanId.ToString() },
        };

        public DeleteWorkOffPlan(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(DevelopmentPlansController),
                  nameof(DevelopmentPlansController.DeleteWorkOffPlan),
                  Parameters)
        {
        }

        [Fact]
        public void DeleteWorkOffPlan_DisplayedCorrectly()
        {
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.CancelLinkDisplayed().Should().BeTrue();
        }

        [Fact]
        public void DeleteWorkOffPlan_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DevelopmentPlansController),
                nameof(DevelopmentPlansController.EditWorkOffPlan))
            .Should()
            .BeTrue();
        }

        [Fact]
        public void DeleteWorkOffPlan_ClickCancelLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(CommonSelectors.CancelLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DevelopmentPlansController),
                nameof(DevelopmentPlansController.EditWorkOffPlan))
            .Should()
            .BeTrue();
        }

        [Fact]
        public async Task DeleteWorkOffPlan_DeleteWorkOffPlan_ExpectedResult()
        {
            var workOffPlan = await AddWorkOffPlan();

            NavigateToUrl(
                typeof(DevelopmentPlansController),
                nameof(DevelopmentPlansController.DeleteWorkOffPlan),
                new Dictionary<string, string>
                {
                    { nameof(SolutionId), SolutionId.ToString() },
                    { nameof(WorkOffPlanId), workOffPlan.Id.ToString() },
                });

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DevelopmentPlansController),
                nameof(DevelopmentPlansController.DevelopmentPlans))
            .Should()
            .BeTrue();

            await using var context = GetEndToEndDbContext();
            context.WorkOffPlans.Count(wp => wp.Id == workOffPlan.Id).Should().Be(0);
        }

        private async Task<WorkOffPlan> AddWorkOffPlan()
        {
            await using var context = GetEndToEndDbContext();

            var workOffPlan = new WorkOffPlan
            {
                SolutionId = SolutionId,
                StandardId = "S1",
                Details = "Standard for Deletion Test",
                CompletionDate = DateTime.UtcNow.AddDays(5),
            };

            context.WorkOffPlans.Add(workOffPlan);
            await context.SaveChangesAsync();

            return workOffPlan;
        }
    }
}
