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
        public readonly string BuyerUsername = Environment.GetEnvironmentVariable("BuyerUsername");
        public readonly string BuyerPassword = Environment.GetEnvironmentVariable("BuyerPassword");

        public readonly string AdminUsername = Environment.GetEnvironmentVariable("AdminUsername");
        public readonly string AdminPassword = Environment.GetEnvironmentVariable("AdminPassword");

        public readonly Uri uri;

        public WebApplicationConnector Connector { get; protected set; }

        public IWebDriver Driver { get; protected set; }

        internal Actions.Common.CommonActions CommonActions { get; }
        internal Actions.Authorization.ActionCollection AuthorizationPages { get; }

        private readonly ITestOutputHelper testOutputHelper;
        internal WebDriverWait Wait { get; }

        public TestBase(WebApplicationConnector connector,
            ITestOutputHelper testOutputHelper,
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
            IDictionary<string, string> parameters = null,
            IDictionary<string, string> queryParameters = null)
        {
            NavigateToUrl(new Uri(UrlGenerator.GenerateUrlFromMethod(controller, methodName, parameters, queryParameters), UriKind.Relative));
        }

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
    }
}
