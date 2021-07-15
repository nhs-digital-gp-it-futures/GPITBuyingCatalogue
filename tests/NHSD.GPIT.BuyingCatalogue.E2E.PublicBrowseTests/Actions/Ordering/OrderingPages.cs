using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Ordering
{
    internal sealed class OrderingPages
    {
        public OrderingPages(IWebDriver driver)
        {
            PageActions = new ActionCollection
            {
                OrganisationDashboard = new(driver),
                OrderDashboard = new(driver),
                OrderDescription = new(driver),
                CallOffPartyInformation = new(driver),
                SupplierInformation = new(driver),
                CommencementDate = new(driver),
            };
        }

        internal ActionCollection PageActions { get; }
    }
}
