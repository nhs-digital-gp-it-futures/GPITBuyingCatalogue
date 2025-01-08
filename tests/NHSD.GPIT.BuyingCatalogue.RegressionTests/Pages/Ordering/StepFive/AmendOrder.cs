using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.Step_Five
{
    internal class AmendOrder : PageBase
    {
        public AmendOrder(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
            : base(driver, commonActions)
        {
            Factory = factory;
        }

        public LocalWebApplicationFactory Factory { get; }

        public void AmendOrderClickAmend()
        {
            var order = MostRecentOrder();
            Driver.Navigate().Refresh();

            LoadOrderSummary(order);

            CommonActions.ClickLinkElement(OrderSummaryObjects.AmendContract);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.AmendOrder)).Should().BeTrue();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        public void AmendOrderClickChangeDescription()
        {
            var order = MostRecentOrder();
            Driver.Navigate().Refresh();

            LoadOrderSummary(order);
            CommonActions.ClickLinkElement(OrderSummaryObjects.ChangeOrderDescription);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderDescriptionController),
                nameof(OrderDescriptionController.OrderDescription));
        }

        public void AmendOrderValidateChangedDescription(string expectedDescription)
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Summary)).Should().BeTrue();

            Driver.FindElement(OrderSummaryObjects.OrderDescriptionSummary).Text.Should().Contain(expectedDescription);
        }

        private void LoadOrderSummary(Order order)
        {
            CommonActions.ClickLinkElement(ByExtensions.DataTestId($"link-{order.CallOffId}"));

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(OrderController),
                    nameof(OrderController.Summary))
                .Should().BeTrue();
        }

        private Order MostRecentOrder()
        {
            using var dbContext = Factory.DbContext;
            var order = dbContext.Orders.OrderByDescending(x => x.Completed).FirstOrDefault();
            order.Completed = DateTime.UtcNow;

            Driver.Navigate().Refresh();
            return order;
        }
    }
}
