using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public sealed class OrderDashboard : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(90009, 1);

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), CallOffId.ToString() },
            };

        public OrderDashboard(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(OrderController),
                  nameof(OrderController.Order),
                  Parameters)
        {
        }

        [Fact]
        public async Task OrderDashboard_AllSectionsDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var organisation = await context.Organisations.SingleAsync(o => o.InternalIdentifier == Parameters["InternalOrgId"]);

            CommonActions.PageTitle().Should().BeEquivalentTo($"Order {CallOffId}-{organisation.Name}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDashboard.TaskList)
                .Should().BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDashboard.OrderDescriptionLink)
                .Should().BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDashboard.OrderDescriptionStatus)
                .Should().BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDashboard.LastUpdatedEndNote)
                .Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                Objects.Ordering.OrderDashboard.LastUpdatedEndNote,
                $"Order last updated by Sue Smith on {DateTime.UtcNow.ToString("dd MMMM yyyy")}");
        }

        [Fact]
        public void OrderDashboard_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(DashboardController),
                    nameof(DashboardController.Organisation))
                    .Should().BeTrue();
        }

        [Fact]
        public void OrderDashboard_ClickSolutionSelection_SectionNotStarted_ExpectedResult()
        {
            var completedSectionOrder = new CallOffId(90004, 1);

            var completedSectionParameters = new Dictionary<string, string>()
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), completedSectionOrder.ToString() },
            };

            NavigateToUrl(typeof(OrderController), nameof(OrderController.Order), completedSectionParameters);

            CommonActions.ClickLinkElement(Objects.Ordering.OrderDashboard.SolutionSelectionLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.SelectSolution))
                .Should().BeTrue();
        }

        [Fact]
        public void OrderDashboard_ClickSolutionSelection_SectionInProgress_ExpectedResult()
        {
            var callOffId = new CallOffId(90013, 1);

            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), $"{callOffId}" },
            };

            NavigateToUrl(typeof(OrderController), nameof(OrderController.Order), parameters);

            CommonActions.ClickLinkElement(Objects.Ordering.OrderDashboard.SolutionSelectionLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(TaskListController),
                nameof(TaskListController.TaskList)).Should().BeTrue();
        }

        [Fact]
        public void OrderDashboard_ClickSolutionSelection_SectionCompleted_ExpectedResult()
        {
            var callOffId = new CallOffId(90009, 1);

            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), $"{callOffId}" },
            };

            NavigateToUrl(typeof(OrderController), nameof(OrderController.Order), parameters);

            CommonActions.ClickLinkElement(Objects.Ordering.OrderDashboard.SolutionSelectionLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ReviewSolutionsController),
                nameof(ReviewSolutionsController.ReviewSolutions)).Should().BeTrue();
        }
    }
}
