using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Utils.Files;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public class OrderCompleted : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(90010, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), CallOffId.ToString() },
        };

        public OrderCompleted(LocalWebApplicationFactory factory)
            : base(factory, typeof(OrderController), nameof(OrderController.Completed), Parameters)
        {
        }

        [Fact]
        public void OrderCompleted_AllSectionsDisplayed()
        {
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderCompletedObjects.DownloadPdfButton).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderCompletedObjects.ReturnToDashboardButton).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderCompletedObjects.ContactProcurementLink).Should().BeTrue();
        }

        [Fact]
        public void OrderCompleted_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DashboardController),
                nameof(DashboardController.Organisation)).Should().BeTrue();
        }

        [Fact]
        public void OrderCompleted_ClickDownloadPdfButton_ExpectedResult()
        {
            string filePath = @$"{Path.GetTempPath()}order-summary-completed-C090010-01.pdf";

            FileHelper.DeleteDownloadFile(filePath);

            CommonActions.ClickLinkElement(OrderCompletedObjects.DownloadPdfButton);

            FileHelper.WaitForDownloadFile(filePath);

            FileHelper.FileExists(filePath).Should().BeTrue();
            FileHelper.FileLength(filePath).Should().BePositive();
            FileHelper.ValidateIsPdf(filePath);

            FileHelper.DeleteDownloadFile(filePath);
        }

        [Fact]
        public void OrderCompleted_ClickReturnToDashboardButton_ExpectedResult()
        {
            CommonActions.ClickLinkElement(OrderCompletedObjects.ReturnToDashboardButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DashboardController),
                nameof(DashboardController.Organisation)).Should().BeTrue();
        }

        [Fact]
        public void OrderCompleted_ClickContactProcurementLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(OrderCompletedObjects.ContactProcurementLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ProcurementHubController),
                nameof(ProcurementHubController.Index)).Should().BeTrue();

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DashboardController),
                nameof(DashboardController.Organisation)).Should().BeTrue();
        }
    }
}
