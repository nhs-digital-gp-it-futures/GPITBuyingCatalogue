using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using netDumbster.smtp;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public sealed class OrderSummary
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const string OdsCode = "03F";
        private static readonly CallOffId CallOffId = new(90009, 1);

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(OdsCode), OdsCode },
                { nameof(CallOffId), CallOffId.ToString() },
            };

        private readonly SimpleSmtpServer smtp;

        public OrderSummary(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(OrderController),
                  nameof(OrderController.Summary),
                  Parameters)
        {
            smtp = SimpleSmtpServer.Start(9999);
        }

        [Fact]
        public void OrderSummary_OrderNotComplete_AllSectionsDisplayed()
        {
            RedirectToSummaryForOrder(new CallOffId(90004, 1));

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderSummary.PrintPDFButton).Should().BeFalse();

            Driver.FindElements(Objects.Ordering.OrderSummary.PrintPDFButton).Count.Should().Be(0);

            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeFalse();

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
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
        }

        [Fact]
        public void OrderSummary_OrderReadyToComplete_AllSectionsDisplayed()
        {
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderSummary.PrintPDFButton).Should().BeTrue();

            Driver.FindElements(Objects.Ordering.OrderSummary.PrintPDFButton).Count.Should().Be(1);

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
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

            smtp.ReceivedEmailCount.Should().Be(1);

            smtp.ReceivedEmail[0].Subject.Equals($"New Order {CallOffId}_{OdsCode}").Should().BeTrue();

            using var context = GetEndToEndDbContext();

            context.Orders.Single(o => o.Id == CallOffId.Id).Completed.Should().NotBeNull();
        }

        public void Dispose()
        {
            smtp.Dispose();
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
