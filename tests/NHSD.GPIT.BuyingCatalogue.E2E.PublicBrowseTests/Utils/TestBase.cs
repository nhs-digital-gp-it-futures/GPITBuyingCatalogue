using Microsoft.Extensions.DependencyInjection;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.PublicBrowse;
using OpenQA.Selenium;
using System;
using System.Linq;
using System.Net.Http;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils
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
            PublicBrowsePages = new PublicBrowsePages(this.driver).PageActions;
        }

        internal TDbContext GetContext<TDbContext>()
        {
            var serviceScopeFactory = (IServiceScopeFactory)factory.Services.GetService<IServiceScopeFactory>();

            var scope = serviceScopeFactory.CreateScope();

            return scope.ServiceProvider.GetServices<TDbContext>().First();

            //var scopedServices = factory.Services.GetRequiredService<TDbContext>();
            //
            //return scopedServices;
        }

        private readonly HttpClient client;
        protected readonly LocalWebApplicationFactory factory;
        protected readonly IWebDriver driver;

        internal ActionCollection PublicBrowsePages { get; }
    }
}
