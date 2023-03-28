using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.FundingSource;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    [Collection(nameof(OrderingCollection))]
    public sealed class OrderDashboard : BuyerTestBase
    {
        private const string InternalOrgId = "IB-QWO";
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
            var organisation =
                await context.Organisations.FirstAsync(o => o.InternalIdentifier == Parameters["InternalOrgId"]);

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Order {CallOffId}-{organisation.Name}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDashboard.TaskList)
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDashboard.OrderDescriptionLink)
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDashboard.OrderDescriptionStatus)
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDashboard.CallOffParty)
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDashboard.SupplierContact)
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDashboard.Timescales)
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDashboard.SolutionsAndServices)
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDashboard.PlannedDeliveryDates)
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDashboard.FundingSources)
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDashboard.ImplementationMilestones)
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDashboard.AssociatedServiceBilling)
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDashboard.DataProcessingInformation)
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDashboard.ReviewOrder)
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDashboard.LastUpdatedEndNote)
                .Should()
                .BeTrue();

            CommonActions.ElementTextEqualTo(
                Objects.Ordering.OrderDashboard.LastUpdatedEndNote,
                $"Order last updated by Sue Smith on {DateTime.UtcNow.ToString("d MMMM yyyy")}");
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

        [Fact]
        public void OrderDashboard_ClickDeliveryDates_NoDeliveryDatesEntered_ExpectedResult()
        {
            var callOffId = new CallOffId(90005, 1);

            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), $"{callOffId}" },
            };

            NavigateToUrl(typeof(OrderController), nameof(OrderController.Order), parameters);

            CommonActions.ClickLinkElement(Objects.Ordering.OrderDashboard.PlannedDeliveryDates);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.SelectDate)).Should().BeTrue();
        }

        [Fact]
        public void OrderDashboard_ClickDeliveryDates_DeliveryDatesEntered_ExpectedResult()
        {
            var callOffId = new CallOffId(90020, 1);

            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), $"{callOffId}" },
            };

            NavigateToUrl(typeof(OrderController), nameof(OrderController.Order), parameters);

            CommonActions.ClickLinkElement(Objects.Ordering.OrderDashboard.PlannedDeliveryDates);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.Review)).Should().BeTrue();
        }

        [Fact]
        public void OrderDashboard_ClickFundingSource_NoFrameworkSelected_ExpectedResult()
        {
            var callOffId = new CallOffId(90020, 1);

            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), $"{callOffId}" },
            };

            NavigateToUrl(typeof(OrderController), nameof(OrderController.Order), parameters);

            CommonActions.ClickLinkElement(Objects.Ordering.OrderDashboard.FundingSources);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(FundingSourceController),
                nameof(FundingSourceController.SelectFramework))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void OrderDashboard_ClickFundingSource_FrameworkSelected_ExpectedResult()
        {
            var callOffId = new CallOffId(90005, 1);

            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), $"{callOffId}" },
            };

            NavigateToUrl(typeof(OrderController), nameof(OrderController.Order), parameters);

            CommonActions.ClickLinkElement(Objects.Ordering.OrderDashboard.FundingSources);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(FundingSourceController),
                nameof(FundingSourceController.FundingSources))
                .Should()
                .BeTrue();
        }
    }
}
