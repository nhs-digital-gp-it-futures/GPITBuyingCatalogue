using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public sealed class OrderSummary
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string OdsCode = "03F";
        private static readonly CallOffId CallOffId = new(90009, 1);

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(OdsCode), OdsCode },
                { nameof(CallOffId), CallOffId.ToString() },
            };

        public OrderSummary(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(OrderController),
                  nameof(OrderController.Summary),
                  Parameters)
        {
        }

        [Fact]
        public void OrderSummary_OrderNotComplete_AllSectionsDisplayed()
        {
            RedirectToSummaryForOrder(new CallOffId(90004, 1));

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderSummary.PrintPDFButton).Should().BeFalse();

            Driver.FindElements(Objects.Ordering.OrderSummary.PrintPDFButton).Count.Should().Be(0);

            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeFalse();

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                Objects.Ordering.OrderSummary.LastUpdatedEndNote,
                $"Order last updated by Sue Smith on {DateTime.UtcNow.ToString("dd MMM yyyy")}");
        }

        [Fact]
        public void OrderSummary_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions
            .PageLoadedCorrectGetIndex(
                  typeof(OrderController),
                  nameof(OrderController.Order))
            .Should()
            .BeTrue();
        }

        [Fact]
        public void OrderSummary_OrderCompleted_AllSectionsDisplayed()
        {
            RedirectToSummaryForOrder(new CallOffId(90010, 1));

            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeFalse();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderSummary.PrintPDFButton).Should().BeTrue();

            Driver.FindElements(Objects.Ordering.OrderSummary.PrintPDFButton).Count.Should().Be(1);

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                Objects.Ordering.OrderSummary.LastUpdatedEndNote,
                $"Order last updated by Sue Smith on {DateTime.UtcNow.ToString("dd MMM yyyy")}");
        }

        [Fact]
        public void OrderSummary_OrderReadyToComplete_AllSectionsDisplayed()
        {
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderSummary.PrintPDFButton).Should().BeTrue();

            Driver.FindElements(Objects.Ordering.OrderSummary.PrintPDFButton).Count.Should().Be(1);

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                Objects.Ordering.OrderSummary.LastUpdatedEndNote,
                $"Order last updated by Sue Smith on {DateTime.UtcNow.ToString("dd MMM yyyy")}");
        }

        [Fact]
        public void OrderSummary_ClickPrintOrSaveAsPDF_ExpectedResult()
        {
            CommonActions.ClickLinkElement(Objects.Ordering.OrderSummary.PrintPDFButton);

            CommonActions
            .PageLoadedCorrectGetIndex(
                  typeof(OrderController),
                  nameof(OrderController.Summary))
            .Should()
            .BeTrue();

            Driver.Url.EndsWith("?print=true").Should().BeTrue();

            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeFalse();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderSummary.PrintPDFButton).Should().BeFalse();

            CommonActions.GoBackLinkDisplayed().Should().BeFalse();
        }

        [Fact]
        public void OrderSummary_IncompleteOrder_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions
            .PageLoadedCorrectGetIndex(
                  typeof(OrderController),
                  nameof(OrderController.Order))
            .Should()
            .BeTrue();
        }

        [Fact]
        public void OrderSummary_CompletedOrder_ClickGoBackLink_ExpectedResult()
        {
            RedirectToSummaryForOrder(new CallOffId(90010, 1));

            CommonActions.ClickGoBackLink();

            CommonActions
            .PageLoadedCorrectGetIndex(
                  typeof(DashboardController),
                  nameof(DashboardController.Organisation))
            .Should()
            .BeTrue();
        }

        [Fact]
        public void OrderSummary_ClickCompleteOrder_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions
                .PageLoadedCorrectGetIndex(
                      typeof(OrderController),
                      nameof(OrderController.Summary))
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeFalse();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderSummary.PrintPDFButton).Should().BeTrue();

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            Driver.FindElements(Objects.Ordering.OrderSummary.PrintPDFButton).Count.Should().Be(1);

            using var context = GetEndToEndDbContext();

            context.Orders.Single(o => o.Id == CallOffId.Id).Completed.Should().NotBeNull();
        }

        private void RedirectToSummaryForOrder(CallOffId callOffId)
        {
            NavigateToUrl(
                typeof(OrderController),
                nameof(OrderController.Summary),
                new Dictionary<string, string>
                {
                    { nameof(callOffId), callOffId.ToString() },
                    { nameof(OdsCode), OdsCode },
                });
        }
    }
}
