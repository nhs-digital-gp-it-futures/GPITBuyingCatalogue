using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Admin
{
    internal sealed class AdminPages
    {
        public AdminPages(IWebDriver driver)
        {
            PageActions = new()
            {
                Dashboard = new(driver),
                Organisation = new(driver),
                AddUser = new(driver),
                AddRelatedOrganisation = new(driver),
                UserDetails = new(driver),
            };
        }

        public ActionCollection PageActions { get; set; }
    }
}
