using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepFive
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
                nameof(OrderDescriptionController.OrderDescription)).Should().BeTrue();
        }

        public void AmendOrderValidateChangedDescription(string expectedDescription)
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Summary)).Should().BeTrue();

            Driver.FindElement(OrderSummaryObjects.OrderDescriptionSummary).Text.Should().Contain(expectedDescription);
        }

        public void AmendOrderClickChangeOrderingPartyContact()
        {
            var order = MostRecentOrder();
            Driver.Navigate().Refresh();

            LoadOrderSummary(order);
            CommonActions.ClickLinkElement(OrderSummaryObjects.ChangeOrderOrderingPartyContact);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderingPartyController),
                nameof(OrderingPartyController.OrderingParty)).Should().BeTrue();
        }

        public void AmendOrderValidateChangedOrderingPartyContact(Contact contact)
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Summary)).Should().BeTrue();

            Driver.FindElement(OrderSummaryObjects.OrderingPartyContact).Text.Should().Contain(contact.FullName);
            Driver.FindElement(OrderSummaryObjects.OrderingPartyContact).Text.Should().Contain(contact.Email);
        }

        public void AmendOrderClickChangeSupplierContactDetails()
        {
            var order = MostRecentOrder();
            Driver.Navigate().Refresh();

            LoadOrderSummary(order);
            CommonActions.ClickLinkElement(OrderSummaryObjects.ChangeOrderSupplierContact);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.Supplier)).Should().BeTrue();
        }

        public void AmendOrderValidateChangedSupplierContactDetails(Contact contact)
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Summary)).Should().BeTrue();

            Driver.FindElement(OrderSummaryObjects.SupplierContact).Text.Should().Contain(contact.FullName);
            Driver.FindElement(OrderSummaryObjects.SupplierContact).Text.Should().Contain(contact.Email);
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
