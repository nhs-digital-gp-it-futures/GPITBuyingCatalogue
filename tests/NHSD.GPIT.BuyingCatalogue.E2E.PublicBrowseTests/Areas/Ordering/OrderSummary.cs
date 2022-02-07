using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderSummary.DownloadPDFCompletedOrder).Should().BeFalse();

            Driver.FindElements(Objects.Ordering.OrderSummary.DownloadPDFCompletedOrder).Count.Should().Be(0);

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderSummary.DownloadPDFIncompleteOrder).Should().BeFalse();

            Driver.FindElements(Objects.Ordering.OrderSummary.DownloadPDFIncompleteOrder).Count.Should().Be(0);

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

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderSummary.DownloadPDFCompletedOrder).Should().BeTrue();

            Driver.FindElements(Objects.Ordering.OrderSummary.DownloadPDFCompletedOrder).Count.Should().Be(1);

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderSummary.DownloadPDFIncompleteOrder).Should().BeFalse();

            Driver.FindElements(Objects.Ordering.OrderSummary.DownloadPDFIncompleteOrder).Count.Should().Be(0);

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
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

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderSummary.DownloadPDFCompletedOrder).Should().BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderSummary.DownloadPDFIncompleteOrder).Should().BeFalse();

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            Driver.FindElements(Objects.Ordering.OrderSummary.DownloadPDFCompletedOrder).Count.Should().Be(1);

            Driver.FindElements(Objects.Ordering.OrderSummary.DownloadPDFIncompleteOrder).Count.Should().Be(0);

            smtp.ReceivedEmailCount.Should().Be(1);

            smtp.ReceivedEmail[0].Subject.Equals($"New Order {CallOffId}_{OdsCode}").Should().BeTrue();

            using var context = GetEndToEndDbContext();

            context.Orders.Single(o => o.Id == CallOffId.Id).Completed.Should().NotBeNull();
        }

        [Fact]
        public void OrderSummary_OrderReadyToComplete_ClickDownloadPDF_FileDownloaded()
        {
            System.Threading.Thread.Sleep(10000);

            string filePath = @$"{Path.GetTempPath()}order-summary-in-progress-C090009-01.pdf";

            DeleteDownloadFile(filePath);

            Driver.FindElement(Objects.Ordering.OrderSummary.DownloadPDFIncompleteOrder).Click();

            WaitForDownloadFile(filePath);

            File.Exists(filePath).Should().BeTrue();

            new FileInfo(filePath).Length.Should().BePositive();

            ValidateIsPdf(filePath);

            DeleteDownloadFile(filePath);
        }

        [Fact]
        public void OrderSummary_OrderComplete_ClickDownloadPDF_FileDownloaded()
        {
            System.Threading.Thread.Sleep(10000);

            string filePath = @$"{Path.GetTempPath()}order-summary-completed-C090010-01.pdf";

            DeleteDownloadFile(filePath);

            RedirectToSummaryForOrder(new CallOffId(90010, 1));

            Driver.FindElement(Objects.Ordering.OrderSummary.DownloadPDFCompletedOrder).Click();

            WaitForDownloadFile(filePath);

            File.Exists(filePath).Should().BeTrue();

            new FileInfo(filePath).Length.Should().BePositive();

            ValidateIsPdf(filePath);

            DeleteDownloadFile(filePath);
        }

        public void Dispose()
        {
            smtp.Dispose();
        }

        private static void DeleteDownloadFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        private static void WaitForDownloadFile(string filePath)
        {
            for (int i = 0; i < 10; i++)
            {
                if (File.Exists(filePath))
                    break;

                System.Threading.Thread.Sleep(1000);
            }
        }

        private static void ValidateIsPdf(string filePath)
        {
            var buffer = new byte[5];

            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var bytesRead = fs.Read(buffer, 0, buffer.Length);
            fs.Close();
            bytesRead.Should().Be(buffer.Length);
            buffer.Should().BeEquivalentTo(Encoding.ASCII.GetBytes("%PDF-"));
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
