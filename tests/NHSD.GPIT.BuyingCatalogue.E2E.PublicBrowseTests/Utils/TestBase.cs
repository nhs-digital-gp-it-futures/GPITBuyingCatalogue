using Microsoft.Extensions.DependencyInjection;
using NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Actions;
using NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Utils;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Utils
{
    public abstract class TestBase
    {
        private readonly Uri uri;

        public TestBase(LocalWebApplicationFactory factory, string urlArea = "")
        {
            client = factory.CreateClient();
            this.factory = factory;
            driver = factory.Driver;
            uri = new Uri(factory.RootUri);
            driver.Navigate().GoToUrl(new Uri(uri, urlArea));
            Pages = new Pages(this.driver).PageActions;
        }

        internal IEnumerable<TDbContext> GetContext<TDbContext>()
        {
            var serviceScopeFactory = (IServiceScopeFactory)factory.Services.GetService<IServiceScopeFactory>();

            var scope = serviceScopeFactory.CreateScope();

            return scope.ServiceProvider.GetServices<TDbContext>();

            //var scopedServices = factory.Services.GetRequiredService<TDbContext>();
            //
            //return scopedServices;
        }

        private readonly HttpClient client;
        protected readonly LocalWebApplicationFactory factory;
        protected readonly IWebDriver driver;

        internal ActionCollection Pages { get; }
    }
}