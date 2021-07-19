using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Ordering
{
    internal sealed class OrganisationDashboard : ActionBase
    {
        public OrganisationDashboard(IWebDriver driver)
            : base(driver)
        {
        }

        internal bool CreateOrderButtonDisplayed()
        {
            try
            {
                return Driver.FindElement(Objects.Ordering.OrganisationDashboard.CreateOrderLink).Displayed;
            }
            catch
            {
                return false;
            }
        }

        internal bool IncompleteOrderTableDisplayed()
        {
            try
            {
                return Driver.FindElement(Objects.Common.ByExtensions.DataTestId("incomplete-orders-table")).Displayed;
            }
            catch
            {
                return false;
            }
        }

        internal bool CompleteOrderTableDisplayed()
        {
            try
            {
                return Driver.FindElement(Objects.Common.ByExtensions.DataTestId("complete-orders-table")).Displayed;
            }
            catch
            {
                return false;
            }
        }

        internal void ClickCreateNewOrder()
        {
            Driver.FindElement(Objects.Ordering.OrganisationDashboard.CreateOrderLink).Click();
        }
    }
}
