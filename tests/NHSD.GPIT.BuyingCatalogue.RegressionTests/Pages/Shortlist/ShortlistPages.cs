using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.SolutionApplicationType;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Shortlist
{
    public class ShortlistPages
    {
        private const int MaxNumberOfShortlists = 10;

        public ShortlistPages(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
        {
            ShortlistDashboard = new ShortlistDashboard(driver, commonActions);
            Factory = factory;
            Driver = driver;
        }

        internal ShortlistDashboard ShortlistDashboard { get; }

        internal LocalWebApplicationFactory Factory { get; private set; }

        internal IWebDriver Driver { get; }

        public void CreateNewShortlist()
        {
            var existingShortlists = GetShortlisst();

            ShortlistDashboard.ShortlistTriage();

            if (existingShortlists < MaxNumberOfShortlists)
            {
                ShortlistDashboard.CreateShortlist();
            }
            else
            {
                ShortlistDashboard.MaximumShortlists();
            }
        }

        public void CreateShortListForFoundationCapabilities(string shortlistName)
        {
            ShortlistDashboard.FilterByFoundationCapabilities();
            ShortlistDashboard.SaveFilter(shortlistName);
        }

        public void CreateShortListForFramework(string shortlistName)
        {
            ShortlistDashboard.FilterByFramework();
            ShortlistDashboard.SaveFilter(shortlistName);
        }

        public void CreateShortListForApplicationType(string shortlistName, ApplicationTypes applicationType)
        {
            ShortlistDashboard.FilterByApplicationType(applicationType);
            ShortlistDashboard.SaveFilter(shortlistName);
        }

        private int GetShortlisst()
        {
            using var dbContext = Factory.DbContext;
            var result = dbContext.Filters.Count();

            return result;
        }
    }
}
