using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.Files;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public sealed class OrderSummary
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(90009, 1);

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
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

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderSummary.DownloadPDFCompletedOrder).Should().BeFalse();

            Driver.FindElements(Objects.Ordering.OrderSummary.DownloadPDFCompletedOrder).Count.Should().Be(0);

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderSummary.DownloadPDFIncompleteOrder).Should().BeFalse();

            Driver.FindElements(Objects.Ordering.OrderSummary.DownloadPDFIncompleteOrder).Count.Should().Be(0);

            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeFalse();

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                Objects.Ordering.OrderSummary.LastUpdatedEndNote,
                $"Order last updated by Sue Smith on {DateTime.UtcNow.ToString("dd MMMM yyyy")}");
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

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderSummary.DownloadPDFCompletedOrder).Should().BeTrue();

            Driver.FindElements(Objects.Ordering.OrderSummary.DownloadPDFCompletedOrder).Count.Should().Be(1);

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderSummary.DownloadPDFIncompleteOrder).Should().BeFalse();

            Driver.FindElements(Objects.Ordering.OrderSummary.DownloadPDFIncompleteOrder).Count.Should().Be(0);

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                Objects.Ordering.OrderSummary.LastUpdatedEndNote,
                $"Order last updated by Sue Smith on {DateTime.UtcNow.ToString("dd MMMM yyyy")}");
        }

        [Fact]
        public void OrderSummary_OrderReadyToComplete_AllSectionsDisplayed()
        {
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderSummary.DownloadPDFCompletedOrder).Should().BeFalse();

            Driver.FindElements(Objects.Ordering.OrderSummary.DownloadPDFCompletedOrder).Count.Should().Be(0);

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderSummary.DownloadPDFIncompleteOrder).Should().BeTrue();

            Driver.FindElements(Objects.Ordering.OrderSummary.DownloadPDFIncompleteOrder).Count.Should().Be(1);

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                Objects.Ordering.OrderSummary.LastUpdatedEndNote,
                $"Order last updated by Sue Smith on {DateTime.UtcNow.ToString("dd MMMM yyyy")}");
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

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Completed)).Should().BeTrue();

            using var context = GetEndToEndDbContext();

            context.Orders.Single(o => o.Id == CallOffId.Id).Completed.Should().NotBeNull();
        }

        [Fact]
        public void OrderSummary_OrderComplete_ClickDownloadPDF_FileDownloaded()
        {
            string filePath = @$"{Path.GetTempPath()}order-summary-completed-C090010-01.pdf";

            FileHelper.DeleteDownloadFile(filePath);

            RedirectToSummaryForOrder(new CallOffId(90010, 1));

            Driver.FindElement(Objects.Ordering.OrderSummary.DownloadPDFCompletedOrder).Click();

            FileHelper.WaitForDownloadFile(filePath);

            FileHelper.FileExists(filePath).Should().BeTrue();
            FileHelper.FileLength(filePath).Should().BePositive();
            FileHelper.ValidateIsPdf(filePath);

            FileHelper.DeleteDownloadFile(filePath);
        }

        [Fact]
        public void OrderSummary_OrderReadyToComplete_ClickDownloadPDF_FileDownloaded()
        {
            string filePath = @$"{Path.GetTempPath()}order-summary-in-progress-C090009-01.pdf";

            FileHelper.DeleteDownloadFile(filePath);

            Driver.FindElement(Objects.Ordering.OrderSummary.DownloadPDFIncompleteOrder).Click();

            FileHelper.WaitForDownloadFile(filePath);

            FileHelper.FileExists(filePath).Should().BeTrue();
            FileHelper.FileLength(filePath).Should().BePositive();
            FileHelper.ValidateIsPdf(filePath);

            FileHelper.DeleteDownloadFile(filePath);
        }

        private void RedirectToSummaryForOrder(CallOffId callOffId)
        {
            NavigateToUrl(
                typeof(OrderController),
                nameof(OrderController.Summary),
                new Dictionary<string, string>
                {
                    { nameof(callOffId), callOffId.ToString() },
                    { nameof(InternalOrgId), InternalOrgId },
                });
        }
    }
}
