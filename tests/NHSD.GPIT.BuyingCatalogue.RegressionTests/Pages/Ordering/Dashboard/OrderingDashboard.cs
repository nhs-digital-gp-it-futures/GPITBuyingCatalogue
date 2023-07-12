using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.Dashboard
{
    public sealed class OrderingDashboard : PageBase
    {
        public OrderingDashboard(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void CreateNewOrder()
        {
            CommonActions.ClickLinkElement(OrganisationDashboard.CreateManageOrders);

            CommonActions.ClickLinkElement(OrganisationDashboard.CreateOrderLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderTriageController),
                nameof(OrderTriageController.OrderItemType)).Should().BeTrue();
        }
    }
}
