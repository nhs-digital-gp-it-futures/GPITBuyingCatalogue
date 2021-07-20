using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Authorization;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Database;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils
{
    public abstract class TestBase
    {
        public static readonly string DefaultPassword = "Th1sIsP4ssword!";

        private readonly Uri uri;

        public TestBase(LocalWebApplicationFactory factory, string urlArea = "")
        {
            Factory = factory;

            Driver = Factory.Driver;
            PublicBrowsePages = new PublicBrowsePages(Driver).PageActions;
            MarketingPages = new MarketingPageActions(Driver).PageActions;
            AuthorizationPages = new AuthorizationPages(Driver).PageActions;
            AdminPages = new AdminPages(Driver).PageActions;
            OrderingPages = new OrderingPages(Driver).PageActions;
            CommonActions = new Actions.Common.CommonActions(Driver);

            TextGenerators = new TextGenerators(Driver);

            uri = new Uri(factory.RootUri);
            var combinedUri = new Uri(uri, urlArea);
            Driver.Navigate().GoToUrl(combinedUri);
        }

        public LocalWebApplicationFactory Factory { get; protected set; }

        public IWebDriver Driver { get; protected set; }

        internal Actions.Common.CommonActions CommonActions { get; }

        internal Actions.PublicBrowse.ActionCollection PublicBrowsePages { get; }

        internal Actions.Marketing.ActionCollection MarketingPages { get; }

        internal Actions.Authorization.ActionCollection AuthorizationPages { get; }

        internal Actions.Admin.ActionCollection AdminPages { get; }

        internal Actions.Ordering.ActionCollection OrderingPages { get; }

        internal TextGenerators TextGenerators { get; }

        internal EndToEndDbContext GetEndToEndDbContext()
        {
            var options = new DbContextOptionsBuilder<EndToEndDbContext>()
                .UseInMemoryDatabase(Factory.BcDbName)
                .Options;

            return new(options);
        }

        internal EndToEndDbContext GetUsersContext()
        {
            var options = new DbContextOptionsBuilder<EndToEndDbContext>()
                .UseInMemoryDatabase(Factory.BcDbName)
                .Options;

            return new(options);
        }

        internal void ClearClientApplication(CatalogueItemId solutionId)
        {
            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.Single(s => s.Id == solutionId);
            solution.ClientApplication = string.Empty;
            context.SaveChanges();
        }

        internal void ClearHostingTypes(CatalogueItemId solutionId)
        {
            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.Single(s => s.Id == solutionId);
            solution.Hosting = null;
            context.SaveChanges();
        }

        internal void ClearFeatures(CatalogueItemId solutionId)
        {
            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.Single(s => s.Id == solutionId);
            solution.Features = string.Empty;
            context.SaveChanges();
        }

        internal void ClearRoadMap(CatalogueItemId solutionId)
        {
            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.Single(s => s.Id == solutionId);
            solution.RoadMap = string.Empty;
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
