using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Authorization;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.UrlGenerators;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;
using Actions = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils
{
    public class TestBase
    {
        private readonly string buyerUsername = Environment.GetEnvironmentVariable("RegressionBuyerUsername")!;
        private readonly string buyerPassword = Environment.GetEnvironmentVariable("RegressionBuyerPassword")!;

        private readonly string adminUsername = Environment.GetEnvironmentVariable("RegressionAdminUsername")!;
        private readonly string adminPassword = Environment.GetEnvironmentVariable("RegressionAdminPassword")!;

        private readonly Uri uri;

        private readonly ITestOutputHelper? testOutputHelper;

        public TestBase(
            LocalWebApplicationFactory factory,
            ITestOutputHelper? testOutputHelper,
            string urlArea = "")
        {
            Factory = factory;
            Driver = factory.Driver;
            this.testOutputHelper = testOutputHelper;

            AuthorizationPages = new AuthorizationPages(Driver).PageActions;
            CommonActions = new Actions.Common.CommonActions(Driver);

            Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            uri = new Uri(factory.RootUri);

            OrderingPages = new OrderingPages(Driver, CommonActions, factory);
            CompetitionPages = new CompetitionPages(Driver, CommonActions, factory);

            NavigateToUrl(urlArea);
        }

        public LocalWebApplicationFactory Factory { get; protected set; }

        public IWebDriver Driver { get; protected set; }

        internal Actions.Common.CommonActions CommonActions { get; }

        internal ActionCollection AuthorizationPages { get; }

        internal WebDriverWait Wait { get; }

        internal OrderingPages OrderingPages { get; }

        internal CompetitionPages CompetitionPages { get; }

        internal void AuthorityLogin()
        {
            if (UserAlreadyLoggedIn() || !AuthorizationPages.LoginActions.EmailAddressInputDisplayed())
                return;
            AuthorizationPages.LoginActions.Login(adminUsername, adminPassword);
        }

        internal void BuyerLogin()
        {
            if (UserAlreadyLoggedIn() || !AuthorizationPages.LoginActions.EmailAddressInputDisplayed())
                return;

            AuthorizationPages.LoginActions.Login(buyerUsername, buyerPassword);
        }

        internal void BuyerLogin(string buyerEmail)
        {
            if (UserAlreadyLoggedIn() || !AuthorizationPages.LoginActions.EmailAddressInputDisplayed())
                return;

            AuthorizationPages.LoginActions.Login(buyerEmail, buyerPassword);
        }

        internal BuyingCatalogueDbContext GetEndToEndDbContext()
        {
            return Factory.DbContext;
        }

        protected bool UserAlreadyLoggedIn() => Driver.Manage().Cookies.GetCookieNamed("user-session") != null;

        protected void NavigateToUrl(string relativeUrl)
        {
            NavigateToUrl(new Uri(relativeUrl, UriKind.Relative));
        }

        protected void NavigateToUrl(Uri relativeUri)
        {
            var combinedUri = new Uri(uri, relativeUri);
            Driver.Navigate().GoToUrl(combinedUri);
        }

        protected void NavigateToUrl(
            Type controller,
            string methodName,
            IDictionary<string, string>? parameters = null,
            IDictionary<string, string>? queryParameters = null)
        {
            NavigateToUrl(new Uri(UrlGenerator.GenerateUrlFromMethod(controller, methodName, parameters, queryParameters), UriKind.Relative));
        }
    }
}
