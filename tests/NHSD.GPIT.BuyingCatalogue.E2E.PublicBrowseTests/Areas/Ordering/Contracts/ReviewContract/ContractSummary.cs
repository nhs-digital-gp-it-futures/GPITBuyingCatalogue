using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Contracts;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Utils.Files;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.Extensions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.Contracts.ReviewContract
{
    [Collection(nameof(OrderingCollection))]
    public class ContractSummary : BuyerTestBase, IDisposable
    {
        private const int OrderId = 90009;
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        public ContractSummary(LocalWebApplicationFactory factory)
            : base(factory, typeof(ReviewContractController), nameof(ReviewContractController.ContractSummary), Parameters(OrderId))
        {
        }

        [Fact]
        public void ContractSummary_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo($"Contract overview - Order {CallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(ReviewContractObjects.ImplementationPlanExpander).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ReviewContractObjects.BespokeImplementationPlan).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ReviewContractObjects.AssociatedServicesExpander).Should().BeFalse();
            CommonActions.ElementIsDisplayed(ReviewContractObjects.DataProcessingExpander).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ReviewContractObjects.BespokeDataProcessing).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ReviewContractObjects.DownloadPdfButton).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ReviewContractObjects.SaveForLaterButton).Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void ContractSummary_DefaultImplementationPlan_AllSectionsDisplayed()
        {
            var context = GetEndToEndDbContext();
            var flags = context.GetContractFlags(OrderId);

            flags.UseDefaultImplementationPlan = true;

            context.SaveChanges();

            Driver.Navigate().Refresh();

            CommonActions.ElementIsDisplayed(ReviewContractObjects.ImplementationPlanExpander).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ReviewContractObjects.BespokeImplementationPlan).Should().BeFalse();
        }

        [Fact]
        public void ContractSummary_DefaultDataProcessing_AllSectionsDisplayed()
        {
            var context = GetEndToEndDbContext();
            var flags = context.GetContractFlags(OrderId);

            flags.UseDefaultDataProcessing = true;

            context.SaveChanges();

            Driver.Navigate().Refresh();

            CommonActions.ElementIsDisplayed(ReviewContractObjects.DataProcessingExpander).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ReviewContractObjects.BespokeDataProcessing).Should().BeFalse();
        }

        [Fact]
        public void ContractSummary_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Summary)).Should().BeTrue();
        }

        [Fact]
        public void ContractSummary_ClickSaveForLaterButton_ExpectedResult()
        {
            CommonActions.ClickLinkElement(ReviewContractObjects.SaveForLaterButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DashboardController),
                nameof(DashboardController.Organisation)).Should().BeTrue();
        }

        [Fact]
        public void ContractSummary_ClickCompleteOrder_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Completed)).Should().BeTrue();
        }

        [Fact]
        public void ContractSummary_ClickDownloadPdfButton_ExpectedResult()
        {
            string filePath = @$"{Path.GetTempPath()}order-summary-in-progress-C0{OrderId}-01.pdf";

            FileHelper.DeleteDownloadFile(filePath);

            CommonActions.ClickLinkElement(ReviewContractObjects.DownloadPdfButton);

            FileHelper.WaitForDownloadFile(filePath);

            FileHelper.FileExists(filePath).Should().BeTrue();
            FileHelper.FileLength(filePath).Should().BePositive();
            FileHelper.ValidateIsPdf(filePath);

            FileHelper.DeleteDownloadFile(filePath);
        }

        [Fact]
        public void ContractSummary_OrderNotReadyToComplete_ClickContinue_ExpectedResult()
        {
            const int orderId = 90001;

            NavigateToUrl(
                typeof(ReviewContractController),
                nameof(ReviewContractController.ContractSummary),
                Parameters(orderId));

            CommonActions.ElementIsDisplayed(ReviewContractObjects.CompleteOrderButton).Should().BeFalse();
            CommonActions.ElementIsDisplayed(CommonSelectors.ContinueButton).Should().BeTrue();

            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        [Fact]
        public void ContractSummary_OrderCompleted_ClickContinue_ExpectedResult()
        {
            const int orderId = 90010;

            NavigateToUrl(
                typeof(ReviewContractController),
                nameof(ReviewContractController.ContractSummary),
                Parameters(orderId));

            CommonActions.ElementIsDisplayed(ReviewContractObjects.CompleteOrderButton).Should().BeFalse();
            CommonActions.ElementIsDisplayed(CommonSelectors.ContinueButton).Should().BeTrue();

            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DashboardController),
                nameof(DashboardController.Organisation)).Should().BeTrue();
        }

        [Fact]
        public void ContractSummary_OrderCompleted_ClickDownloadPdfButton_ExpectedResult()
        {
            const int orderId = 90010;

            NavigateToUrl(
                typeof(ReviewContractController),
                nameof(ReviewContractController.ContractSummary),
                Parameters(orderId));

            string filePath = @$"{Path.GetTempPath()}order-summary-completed-C0{orderId}-01.pdf";

            FileHelper.DeleteDownloadFile(filePath);

            CommonActions.ClickLinkElement(ReviewContractObjects.DownloadPdfButton);

            FileHelper.WaitForDownloadFile(filePath);

            FileHelper.FileExists(filePath).Should().BeTrue();
            FileHelper.FileLength(filePath).Should().BePositive();
            FileHelper.ValidateIsPdf(filePath);

            FileHelper.DeleteDownloadFile(filePath);
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();
            var order = context.Orders.First(x => x.Id == OrderId);
            order.Completed = null;

            var flags = context.GetContractFlags(OrderId);

            flags.HasSpecificRequirements = null;
            flags.UseDefaultBilling = null;
            flags.UseDefaultDataProcessing = null;
            flags.UseDefaultImplementationPlan = null;

            context.SaveChanges();
        }

        private static Dictionary<string, string> Parameters(int orderId) => new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{new CallOffId(orderId, 1)}" },
        };
    }
}
