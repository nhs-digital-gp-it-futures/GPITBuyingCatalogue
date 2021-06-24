using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Authorization;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Database;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils
{
    public abstract class TestBase
    {
        internal static string DefaultPassword = "Th1sIsP4ssword!";
        protected readonly LocalWebApplicationFactory factory;
        protected readonly IWebDriver driver;
        private readonly Uri uri;

        public TestBase(LocalWebApplicationFactory factory, string urlArea = "")
        {
            this.factory = factory;

            driver = this.factory.Driver;
            PublicBrowsePages = new PublicBrowsePages(driver).PageActions;
            MarketingPages = new MarketingPageActions(driver).PageActions;
            AuthorizationPages = new AuthorizationPages(driver).PageActions;
            AdminPages = new AdminPages(driver).PageActions;
            CommonActions = new Actions.Common.CommonActions(driver);

            TextGenerators = new TextGenerators(driver);

            uri = new Uri(factory.RootUri);
            var combinedUri = new Uri(uri, urlArea);
            driver.Navigate().GoToUrl(combinedUri);
        }

        internal Actions.Common.CommonActions CommonActions { get; }

        internal Actions.PublicBrowse.ActionCollection PublicBrowsePages { get; }

        internal Actions.Marketing.ActionCollection MarketingPages { get; }

        internal Actions.Authorization.ActionCollection AuthorizationPages { get; }

        internal Actions.Admin.ActionCollection AdminPages { get; }

        internal TextGenerators TextGenerators { get; }

        internal EndToEndDbContext GetEndToEndDbContext()
        {
            var options = new DbContextOptionsBuilder<EndToEndDbContext>()
                .UseInMemoryDatabase(factory.BcDbName)
                .Options;

            return new(options);
        }

        internal EndToEndDbContext GetUsersContext()
        {
            var options = new DbContextOptionsBuilder<EndToEndDbContext>()
                .UseInMemoryDatabase(factory.BcDbName)
                .Options;

            return new(options);
        }

        internal void ClearClientApplication(string solutionId)
        {
            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.Single(s => s.Id == solutionId);
            solution.ClientApplication = string.Empty;
            context.SaveChanges();
        }

        internal void ClearHostingTypes(string solutionId)
        {
            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.Single(s => s.Id == solutionId);
            solution.Hosting = null;
            context.SaveChanges();
        }

        internal void ClearFeatures(string solutionId)
        {
            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.Single(s => s.Id == solutionId);
            solution.Features = string.Empty;
            context.SaveChanges();
        }

        internal void Login()
        {
            if (AuthorizationPages.LoginActions.EmailAddressInputDisplayed())
            {
                using var context = GetEndToEndDbContext();
                var user = context.AspNetUsers.First(s => s.OrganisationFunction == "Authority").Email;
                AuthorizationPages.LoginActions.Login(user, DefaultPassword);
            }
        }
    }
}
