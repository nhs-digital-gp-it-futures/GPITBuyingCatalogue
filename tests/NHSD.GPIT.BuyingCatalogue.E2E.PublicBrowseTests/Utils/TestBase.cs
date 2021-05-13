using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using OpenQA.Selenium;
using System;
using System.Linq;
using System.Net.Http;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils
{
    public abstract class TestBase
    {
        private readonly Uri uri;
        //private readonly HttpClient client;

        protected readonly LocalWebApplicationFactory factory;
        protected readonly IWebDriver driver;

        internal Actions.PublicBrowse.ActionCollection PublicBrowsePages { get; }
        internal Actions.Marketing.ActionCollection MarketingPages { get; }
        internal Actions.Common.CommonActions CommonActions { get; }
        internal TextGenerators TextGenerators { get; }

        public TestBase(LocalWebApplicationFactory factory, string urlArea = "")
        {
            //client = factory.CreateClient();
            this.factory = factory;

            driver = this.factory.Driver;
            PublicBrowsePages = new PublicBrowsePages(driver).PageActions;
            MarketingPages = new MarketingPageActions(driver).PageActions;
            CommonActions = new Actions.Common.CommonActions(driver);
            TextGenerators = new TextGenerators(driver);

            uri = new Uri(factory.RootUri);
            var combinedUri = new Uri(uri, urlArea);
            driver.Navigate().GoToUrl(combinedUri);
        }

        internal BuyingCatalogueDbContext GetBCContext()
        {
            var options = new DbContextOptionsBuilder<BuyingCatalogueDbContext>()
                .UseInMemoryDatabase(factory.DbName)
                .Options;

            return new(options);
        }

        internal void ClearClientApplication(string solutionId)
        {
            using var context = GetBCContext();
            var solution = context.Solutions.Single(s => s.Id == solutionId);
            solution.ClientApplication = null;
            context.SaveChanges();
        }

        internal void ClearHostingTypes(string solutionId)
        {
            using var context = GetBCContext();
            var solution = context.Solutions.Single(s => s.Id == solutionId);
            solution.Hosting = null;
            context.SaveChanges();
        }

    }
}
