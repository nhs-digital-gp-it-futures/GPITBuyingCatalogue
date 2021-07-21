using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
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
using OpenQA.Selenium.Support.UI;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases
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
            Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));

            TextGenerators = new TextGenerators(Driver);

            uri = new Uri(factory.RootUri);

            NavigateToUrl(urlArea);
        }

        public LocalWebApplicationFactory Factory { get; protected set; }

        public IWebDriver Driver { get; protected set; }

        internal Actions.Common.CommonActions CommonActions { get; }

        internal WebDriverWait Wait { get; private set; }

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

        internal void AuthorityLogin()
        {
            if (AuthorizationPages.LoginActions.EmailAddressInputDisplayed())
            {
                using var context = GetEndToEndDbContext();
                var user = context.AspNetUsers.First(s => s.OrganisationFunction == "Authority").Email;
                AuthorizationPages.LoginActions.Login(user, DefaultPassword);
            }
        }

        internal void BuyerLogin()
        {
            if (AuthorizationPages.LoginActions.EmailAddressInputDisplayed())
            {
                using var context = GetEndToEndDbContext();
                var user = context.AspNetUsers.First(s => s.OrganisationFunction == "Buyer").Email;
                AuthorizationPages.LoginActions.Login(user, DefaultPassword);
            }
        }

        protected static string GenerateUrlFromMethod(
            Type controller,
            string methodName,
            IDictionary<string, string> parameters,
            IDictionary<string, string> queryParameters = null)
        {
            if (controller.BaseType != typeof(Controller))
                throw new InvalidOperationException($"{nameof(controller)} is not a type of {nameof(Controller)}");

            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentNullException(nameof(methodName), $"{nameof(methodName)} should not be null");

            if (parameters is null)
                throw new ArgumentNullException(nameof(parameters), $"{nameof(parameters)} should not be null");

            var controllerRoute =
                (controller.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.RouteAttribute), false)
                            ?.FirstOrDefault() as Microsoft.AspNetCore.Mvc.RouteAttribute)
                                ?.Template;

            var methodRoute =
                (controller.GetMethods()
                .Where(m =>
                    m.Name == methodName
                    && m.GetCustomAttributes(typeof(HttpGetAttribute), false)
                        .Any())
                        ?.FirstOrDefault()
                        .GetCustomAttributes(typeof(HttpGetAttribute), false)
                            .FirstOrDefault() as HttpGetAttribute)?.Template;

            var absoluteRoute = methodRoute switch
            {
                null => new StringBuilder(controllerRoute.ToLowerInvariant()),
                _ => methodRoute[0] != '~'
                    ? new StringBuilder(controllerRoute.ToLowerInvariant() + "/" + methodRoute.ToLowerInvariant())
                    : new StringBuilder(methodRoute[2..].ToLowerInvariant()),
            };

            if (parameters.Any())
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
            IDictionary<string, string> parameters,
            IDictionary<string, string> queryParameters = null)
        {
            NavigateToUrl(new Uri(GenerateUrlFromMethod(controller, methodName, parameters, queryParameters), UriKind.Relative));
        }
    }
}
