using Microsoft.Extensions.DependencyInjection;
using NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Actions;
using NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Utils;
using OpenQA.Selenium;
using System;

namespace NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Areas.Homepage
{
    public abstract class TestBase
    {
        private readonly Uri uri;

        public TestBase(LocalWebApplicationFactory factory, string urlArea = "")
        {
            this.factory = factory;
            driver = factory.Driver;
            uri = new Uri(factory.RootUri);
            driver.Navigate().GoToUrl(new Uri(uri, urlArea));
            Pages = new Pages(this.driver).PageActions;
        }

        internal TDbContext GetContext<TDbContext>()
        {
            var scopedServices = factory.Server.Host.Services.GetService<IServiceScopeFactory>();
            using var scope = scopedServices.CreateScope();
            return scope.ServiceProvider.GetService<TDbContext>();
        }

        protected readonly LocalWebApplicationFactory factory;
        protected readonly IWebDriver driver;

        internal ActionCollection Pages { get; }
    }
}