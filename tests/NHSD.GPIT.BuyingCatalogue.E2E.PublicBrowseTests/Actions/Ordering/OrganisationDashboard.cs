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
                Wait.Until(d => d.FindElement(Objects.Ordering.OrganisationDashboard.CreateOrderLink));
                return true;
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
                Wait.Until(d => d.FindElement(Objects.Common.ByExtensions.DataTestId("incomplete-orders-table")));
                return true;
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
                Wait.Until(d => d.FindElement(Objects.Common.ByExtensions.DataTestId("complete-orders-table")));
                return true;
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
