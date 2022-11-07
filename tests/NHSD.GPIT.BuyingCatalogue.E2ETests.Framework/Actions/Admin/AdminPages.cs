using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Admin
{
    public sealed class AdminPages
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
                AddSolution = new(driver),
                Features = new(driver),
                CommonActions = new(driver),
                Details = new(driver),
            };
        }

        public ActionCollection PageActions { get; set; }
    }
}
