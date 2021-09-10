using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Authorization;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Database;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.MemoryCache;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.Session;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases
{
    public abstract class TestBase
    {
        public static readonly string DefaultPassword = "Th1sIsP4ssword!";

        private readonly Uri uri;

        protected TestBase(
            LocalWebApplicationFactory factory,
            string urlArea = "")
        {
            Factory = factory;

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
            if (!AuthorizationPages.LoginActions.EmailAddressInputDisplayed())
                return;

            using var context = GetEndToEndDbContext();
            var user = context.AspNetUsers.First(s => s.OrganisationFunction == "Authority").Email;
            AuthorizationPages.LoginActions.Login(user, DefaultPassword);
        }

        internal void BuyerLogin()
        {
            if (!AuthorizationPages.LoginActions.EmailAddressInputDisplayed())
                return;

            using var context = GetEndToEndDbContext();
            var user = context.AspNetUsers.First(s => s.OrganisationFunction == "Buyer").Email;
            AuthorizationPages.LoginActions.Login(user, DefaultPassword);
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

        internal void InitializeMemoryCacheHander()
        {
            MemoryCache = new MemoryCacheHandler(
                Factory.GetMemoryCache);
        }

        internal void InitializeServiceRecipientMemoryCacheHandler(string odsCode)
        {
            MemoryCache = new MemoryCacheHandler(
                Factory.GetMemoryCache);

            MemoryCache.InitialiseServiceRecipients(odsCode);
        }

        internal Task DisposeSession()
        {
            if (Session is null)
            {
                InitializeSessionHandler();
            }

            return Session.Clear();
        }

        protected static string GenerateUrlFromMethod(
            Type controllerType,
            string methodName,
            IDictionary<string, string> parameters = null,
            IDictionary<string, string> queryParameters = null)
        {
            if (controllerType.BaseType != typeof(Controller))
                throw new InvalidOperationException($"{nameof(controllerType)} is not a type of {nameof(Controller)}");

            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentNullException(nameof(methodName), $"{nameof(methodName)} should not be null");

            var controllerRoute = controllerType.GetCustomAttribute<RouteAttribute>(false)?.Template ?? string.Empty;

            var methodRoute = controllerType.GetMethods()
                .FirstOrDefault(m => m.Name == methodName && m.GetCustomAttribute<HttpGetAttribute>(false) is not null)?
                .GetCustomAttribute<HttpGetAttribute>(false)?.Template;

            var absoluteRoute = methodRoute switch
            {
                null => new StringBuilder(controllerRoute.ToLowerInvariant()),
                _ => methodRoute[0] != '~'
                    ? new StringBuilder(controllerRoute.ToLowerInvariant() + "/" + methodRoute.ToLowerInvariant())
                    : new StringBuilder(methodRoute[2..].ToLowerInvariant()),
            };

            if (parameters is not null && parameters.Any())
            {
                foreach (var param in parameters)
                    absoluteRoute.Replace('{' + param.Key.ToLowerInvariant() + '}', param.Value);
            }

            if (queryParameters is not null && queryParameters.Any())
            {
                absoluteRoute.Append('?');
                foreach (var param in queryParameters)
                    absoluteRoute.Append($"{param.Key}={param.Value}&");

                absoluteRoute.Remove(absoluteRoute.Length - 1, 1);
            }

            var absoluteRouteUrl = absoluteRoute.ToString();

            if (absoluteRouteUrl.Contains('{'))
            {
                Regex rx = new Regex(@"{[^}]*}", RegexOptions.IgnoreCase);

                var exceptionMessage = $"Not all Parameters in the URL String has been given values." +
                    $" Theses Parameters are missing {string.Join(",", rx.Matches(absoluteRouteUrl).Select(m => m.Value))}";

                throw new InvalidOperationException(exceptionMessage);
            }

            return new Uri("https://www.fake.com/" + absoluteRoute.ToString(), UriKind.Absolute)
                .PathAndQuery[1..];
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
            NavigateToUrl(new Uri(GenerateUrlFromMethod(controller, methodName, parameters, queryParameters), UriKind.Relative));
        }
    }
}
