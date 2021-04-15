using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.WebApp;
using OpenQA.Selenium;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Utils
{
    public class LocalWebApplicationFactory : WebApplicationFactory<Startup>
    {
        private const string LocalhostBaseAddress = "https://localhost";

        private readonly IWebHost host;
        private readonly string DbName;

        internal IWebDriver Driver { get; }

        // Need to find a better way of doing this
        private const string BC_DB_CONNECTION = "Server=localhost,1450;Database=buyingcatalogue;User=SA;password=8VSKwQ8xgk35qWFm8VSKwQ8xgk35qWFm!;Integrated Security=false";
        private const string ID_DB_CONNECTION = "Server=localhost,1450;Database=CatalogueUsers;User=SA;password=8VSKwQ8xgk35qWFm8VSKwQ8xgk35qWFm!;Integrated Security=false";

        public LocalWebApplicationFactory()
        {
            ClientOptions.BaseAddress = new Uri(LocalhostBaseAddress);
            
            DbName = Guid.NewGuid().ToString();
            Environment.SetEnvironmentVariable(nameof(BC_DB_CONNECTION), BC_DB_CONNECTION);
            Environment.SetEnvironmentVariable(nameof(ID_DB_CONNECTION), BC_DB_CONNECTION);
            host = CreateWebHostBuilder().Build();
            host.Start();
            RootUri = host.ServerFeatures.Get<IServerAddressesFeature>().Addresses.LastOrDefault();
            Driver = new BrowserFactory("chrome-local").Driver;
        }

        public string RootUri { get; private set; }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            var builder = WebHost.CreateDefaultBuilder(Array.Empty<string>());
            
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(BuyingCatalogueDbContext));

                if (descriptor is not null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<BuyingCatalogueDbContext>(options =>
                {
                    options.UseInMemoryDatabase(DbName);
                });

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;

                var appDb = scopedServices.GetRequiredService<BuyingCatalogueDbContext>();

                appDb.Database.EnsureCreated();

                // TODO: Seed database with test data here
            });

            builder.UseStartup<Startup>();
            builder.UseUrls($"{LocalhostBaseAddress}:{new Random().Next(10000, 50000)}");
            return builder;
        }

        [ExcludeFromCodeCoverage]
        protected override void Dispose(bool disposing)
        {
            Driver?.Quit();
            base.Dispose(disposing);
            if (disposing)
            {
                host?.Dispose();
            }
        }
    }
}
