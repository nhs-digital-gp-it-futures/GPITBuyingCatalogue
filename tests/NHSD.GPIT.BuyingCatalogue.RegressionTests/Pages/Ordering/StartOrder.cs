using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering
{
    public class StartOrder : PageBase
    {
        public StartOrder(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void ReadyToStart()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.NewOrder));
        }
    }
}
