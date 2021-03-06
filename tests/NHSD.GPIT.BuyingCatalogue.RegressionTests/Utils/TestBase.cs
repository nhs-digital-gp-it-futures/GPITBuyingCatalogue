using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Authorization;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.UrlGenerators;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;
using Actions = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils
{
    public class TestBase
    {
        public readonly string BuyerUsername = Environment.GetEnvironmentVariable("RegressionBuyerUsername")!;
        public readonly string BuyerPassword = Environment.GetEnvironmentVariable("RegressionBuyerPassword")!;

        public readonly string AdminUsername = Environment.GetEnvironmentVariable("RegressionAdminUsername")!;
        public readonly string AdminPassword = Environment.GetEnvironmentVariable("RegressionAdminPassword")!;

        public readonly Uri uri;

        private readonly ITestOutputHelper? testOutputHelper;

        public TestBase(WebApplicationConnector connector,
            ITestOutputHelper? testOutputHelper,
            string urlArea = "")
        {
            Connector = connector;
            Driver = connector.Driver;
            this.testOutputHelper = testOutputHelper;

            AuthorizationPages = new AuthorizationPages(Driver).PageActions;
            CommonActions = new Actions.Common.CommonActions(Driver);

            Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            uri = new Uri(connector.RootUri);

            NavigateToUrl(urlArea);
        }

        public WebApplicationConnector Connector { get; protected set; }

        public IWebDriver Driver { get; protected set; }

        internal Actions.Common.CommonActions CommonActions { get; }

        internal Actions.Authorization.ActionCollection AuthorizationPages { get; }

        internal WebDriverWait Wait { get; }

        internal void AuthorityLogin()
        {
            if (UserAlreadyLoggedIn() || !AuthorizationPages.LoginActions.EmailAddressInputDisplayed())
                return;
            AuthorizationPages.LoginActions.Login(AdminUsername, AdminPassword);
        }

        internal void BuyerLogin()
        {
            if (UserAlreadyLoggedIn() || !AuthorizationPages.LoginActions.EmailAddressInputDisplayed())
                return;

            AuthorizationPages.LoginActions.Login(BuyerUsername, BuyerPassword);
        }

        internal void BuyerLogin(string buyerEmail)
        {
            if (UserAlreadyLoggedIn() || !AuthorizationPages.LoginActions.EmailAddressInputDisplayed())
                return;

            AuthorizationPages.LoginActions.Login(buyerEmail, BuyerPassword);
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
