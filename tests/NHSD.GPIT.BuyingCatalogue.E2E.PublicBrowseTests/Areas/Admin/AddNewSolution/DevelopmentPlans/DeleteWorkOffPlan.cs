using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.DevelopmentPlans
{
    public class DeleteWorkOffPlan : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
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
        public void DeleteWorkOffPlan_DeleteWorkOffPlan_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DevelopmentPlansController),
                nameof(DevelopmentPlansController.DevelopmentPlans))
            .Should()
            .BeTrue();

            using var context = GetEndToEndDbContext();
            context.WorkOffPlans.SingleOrDefaultAsync(wp => wp.Id == WorkOffPlanId).Result.Should().BeNull();
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            var workOffPlan = context.WorkOffPlans.SingleOrDefaultAsync(wp => wp.Id == WorkOffPlanId).Result;

            if (workOffPlan is null)
            {
                workOffPlan = new EntityFramework.Catalogue.Models.WorkOffPlan()
                {
                    Id = WorkOffPlanId,
                    SolutionId = SolutionId,
                    StandardId = "S1",
                    Details = "Standard for Deletion Test",
                    CompletionDate = DateTime.UtcNow.AddDays(5),
                };

                context.WorkOffPlans.Add(workOffPlan);
            }

            context.SaveChanges();
        }
    }
}
