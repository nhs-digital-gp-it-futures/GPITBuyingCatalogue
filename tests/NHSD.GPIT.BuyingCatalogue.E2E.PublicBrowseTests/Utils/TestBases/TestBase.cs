using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Database;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Authorization;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.UrlGenerators;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.MemoryCache;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.Session;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Polly;
using Xunit.Abstractions;
using Actions = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases
{
    public abstract class TestBase
    {
        public static readonly string DefaultPassword = "Th1sIsP4ssword!";

        private const int MaxRetries = 3;

        private static readonly PolicyBuilder RetryPolicy = Policy
                .Handle<Exception>();

        private readonly Uri uri;

        private readonly ITestOutputHelper testOutputHelper;

        protected TestBase(
            LocalWebApplicationFactory factory,
            ITestOutputHelper testOutputHelper,
            string urlArea = "")
        {
            Factory = factory;
            LocalWebApplicationFactory.TestOutputHelper = testOutputHelper;
            this.testOutputHelper = testOutputHelper;
            Driver = Factory.Driver;
            PublicBrowsePages = new PublicBrowsePages(Driver).PageActions;
            MarketingPages = new MarketingPageActions(Driver).PageActions;
            AuthorizationPages = new AuthorizationPages(Driver).PageActions;
            AdminPages = new AdminPages(Driver).PageActions;
            OrderingPages = new OrderingPages(Driver).PageActions;
            CommonActions = new Actions.Common.CommonActions(Driver);
            Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            TextGenerators = new TextGenerators(Driver);

            uri = new Uri(factory.RootUri);

            NavigateToUrl(urlArea);
        }

        public LocalWebApplicationFactory Factory { get; protected set; }

        public IWebDriver Driver { get; protected set; }

        internal Actions.Common.CommonActions CommonActions { get; }

        internal SessionHandler Session { get; private set; }

        internal MemoryCacheHandler MemoryCache { get; private set; }

        internal WebDriverWait Wait { get; }

        internal Actions.PublicBrowse.ActionCollection PublicBrowsePages { get; }

        internal Actions.Marketing.ActionCollection MarketingPages { get; }

        internal Actions.Authorization.ActionCollection AuthorizationPages { get; }

        internal Actions.Admin.ActionCollection AdminPages { get; }

        internal Actions.Ordering.ActionCollection OrderingPages { get; }

        internal TextGenerators TextGenerators { get; }

        internal EndToEndDbContext GetEndToEndDbContext()
        {
            return Factory.DbContext;
        }

        internal EndToEndDbContext GetUsersContext()
        {
            return Factory.DbContext;
        }

        internal void ClearClientApplication(CatalogueItemId solutionId)
        {
            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.Single(s => s.CatalogueItemId == solutionId);
            solution.ClientApplication = string.Empty;
            context.SaveChanges();
        }

        internal void ClearHostingTypes(CatalogueItemId solutionId)
        {
            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.Single(s => s.CatalogueItemId == solutionId);
            solution.Hosting = null;
            context.SaveChanges();
        }

        internal void ClearFeatures(CatalogueItemId solutionId)
        {
            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.Single(s => s.CatalogueItemId == solutionId);
            solution.Features = string.Empty;
            context.SaveChanges();
        }

        internal void ClearRoadMap(CatalogueItemId solutionId)
        {
            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.Single(s => s.CatalogueItemId == solutionId);
            solution.RoadMap = string.Empty;
            context.SaveChanges();
        }

        internal void AuthorityLogin()
        {
            if (UserAlreadyLoggedIn() || !AuthorizationPages.LoginActions.EmailAddressInputDisplayed())
                return;

            using var context = GetEndToEndDbContext();
            var user = context.AspNetUsers.First(s => s.OrganisationFunction == "Authority").UserName;
            AuthorizationPages.LoginActions.Login(user, DefaultPassword);
        }

        internal void BuyerLogin()
        {
            if (UserAlreadyLoggedIn() || !AuthorizationPages.LoginActions.EmailAddressInputDisplayed())
                return;

            using var context = GetEndToEndDbContext();
            var user = context.AspNetUsers.First(s => s.OrganisationFunction == "Buyer").UserName;
            AuthorizationPages.LoginActions.Login(user, DefaultPassword);
        }

        internal void BuyerLogin(string buyerEmail)
        {
            if (UserAlreadyLoggedIn() || !AuthorizationPages.LoginActions.EmailAddressInputDisplayed())
                return;

            AuthorizationPages.LoginActions.Login(buyerEmail, DefaultPassword);
        }

        internal async Task SetValuesToSession(Dictionary<string, object> sessionValues)
        {
            foreach (var (key, value) in sessionValues)
            {
                switch (value)
                {
                    case string:
                        await Session.SetStringAsync(key, value.ToString());
                        break;

                    default:
                        await Session.SetObjectAsync(key, value);
                        break;
                }
            }
        }

        internal void InitializeSessionHandler()
        {
            Session = new SessionHandler(
                Factory.GetDataProtectionProvider,
                Factory.GetDistributedCache,
                Factory.Driver,
                Factory.GetLoggerFactory);
        }

        internal void InitializeMemoryCacheHandler()
        {
            MemoryCache = new MemoryCacheHandler(
                Factory.GetMemoryCache);
        }

        internal void InitializeServiceRecipientMemoryCacheHandler(string odsCode)
        {
            MemoryCache = new MemoryCacheHandler(
                Factory.GetMemoryCache);

            MemoryCache.InitializeServiceRecipients(odsCode);
        }

        internal Task DisposeSession()
        {
            if (Session is null)
            {
                InitializeSessionHandler();
            }

            return Session.Clear();
        }

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

        protected bool UserAlreadyLoggedIn() => Driver.Manage().Cookies.GetCookieNamed("user-session") != null;

        protected async Task RunTestAsync(Func<Task> task, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "")
        {
            try
            {
                await task();
            }
            catch
            {
                TakeScreenShot(callerMemberName, callerFilePath);
                throw;
            }
        }

        protected Task RunTestWithRetryAsync(
            Func<Task> task,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            TimeSpan? retryInterval = null)
        {
            var policy = RetryPolicy
                .WaitAndRetryAsync(
                    MaxRetries,
                    _ => retryInterval ?? TimeSpan.FromSeconds(2),
                    (ex, sleepDuration, attempt, context) =>
                    {
                        if (attempt == MaxRetries)
                            TakeScreenShot(callerMemberName, callerFilePath);

                        Driver.Navigate().Refresh();
                        return Task.CompletedTask;
                    });

            return policy.ExecuteAsync(task);
        }

        protected void RunTest(Action action, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "")
        {
            try
            {
                action();
            }
            catch
            {
                TakeScreenShot(callerMemberName, callerFilePath);
                throw;
            }
        }

        protected void RunTestWithRetry(
            Action action,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            TimeSpan? retryInterval = null)
        {
            var policy = RetryPolicy
                .WaitAndRetry(
                    MaxRetries,
                    _ => retryInterval ?? TimeSpan.FromSeconds(2),
                    (ex, sleepDuration, attempt, context) =>
                    {
                        if (attempt == MaxRetries)
                            TakeScreenShot(callerMemberName, callerFilePath);

                        Driver.Navigate().Refresh();
                    });

            policy.Execute(action);
        }

        private void TakeScreenShot(string memberName, string fileName)
        {
            var outputFolder = $"{AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin"))}ScreenShots";

            LogMessage($"Writing screenshot to {outputFolder} folder");

            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            var filePath = $@"{outputFolder}/{Path.GetFileNameWithoutExtension(fileName)}-{memberName}.png";

            LogMessage($"Writing screenshot to {filePath}");

            if (File.Exists(filePath))
                File.Delete(filePath);

            var screenshot = (Driver as ITakesScreenshot).GetScreenshot();
            screenshot.SaveAsFile(filePath);

            if (!File.Exists(filePath))
                LogMessage("Screenshot file was not written");
            else
                LogMessage("Screenshot file was written");
        }

        private void LogMessage(string message)
        {
            if (testOutputHelper != null)
            {
                testOutputHelper.WriteLine(message);
            }
        }
    }
}
