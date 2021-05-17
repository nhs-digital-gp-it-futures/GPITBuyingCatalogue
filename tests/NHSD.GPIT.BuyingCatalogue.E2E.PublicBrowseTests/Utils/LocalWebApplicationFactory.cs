using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.WebApp;
using OpenQA.Selenium;
using Serilog;
using Serilog.Events;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils
{
    public class LocalWebApplicationFactory : WebApplicationFactory<Startup>
    {
        private const string LocalhostBaseAddress = "https://127.0.0.1";

        private readonly IWebHost host;
        internal readonly string DbName;

        internal IWebDriver Driver { get; }

        // Need to find a better way of doing this
        private const string BC_DB_CONNECTION = "Server=localhost,1450;Database=buyingcatalogue;User=SA;password=8VSKwQ8xgk35qWFm8VSKwQ8xgk35qWFm!;Integrated Security=false";

        private const string ID_DB_CONNECTION = "Server=localhost,1450;Database=CatalogueUsers;User=SA;password=8VSKwQ8xgk35qWFm8VSKwQ8xgk35qWFm!;Integrated Security=false";
        private const string OPERATING_MODE = "private";

        private const string Browser = "chrome";

        public LocalWebApplicationFactory()
        {
            ClientOptions.BaseAddress = new Uri(LocalhostBaseAddress);

            DbName = Guid.NewGuid().ToString();

            SetEnvVariables();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .CreateLogger();

            host = CreateWebHostBuilder().Build();
            host.Start();

            RootUri = host.ServerFeatures.Get<IServerAddressesFeature>().Addresses.LastOrDefault();

            var browserFactory = new BrowserFactory(Browser);
            Driver = browserFactory.Driver;

            if (!Browser.Contains("local") && !Debugger.IsAttached && browserFactory.GridRunning)
            {
                RootUri = RootUri.Replace("127.0.0.1", "host.docker.internal");
            }
        }

        public string RootUri { get; private set; }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            var builder = WebHost.CreateDefaultBuilder(Array.Empty<string>()).UseSerilog();
            builder.UseStartup<Startup>();
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

                var bcDb = scopedServices.GetRequiredService<BuyingCatalogueDbContext>();

                bcDb.Database.EnsureCreated();

                try
                {
                    BuyingCatalogueSeedData.Initialize(bcDb);
                }
                catch
                {
                    // figure out error logging here
                }
            });

            builder.UseUrls($"{LocalhostBaseAddress}:0");
            return builder;
        }

        private void SetEnvVariables()
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(nameof(BC_DB_CONNECTION))))
            {
                Environment.SetEnvironmentVariable(nameof(BC_DB_CONNECTION), BC_DB_CONNECTION);
            }

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(nameof(ID_DB_CONNECTION))))
            {
                Environment.SetEnvironmentVariable(nameof(ID_DB_CONNECTION), BC_DB_CONNECTION);
            }

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(nameof(OPERATING_MODE))))
            {
                Environment.SetEnvironmentVariable(nameof(OPERATING_MODE), OPERATING_MODE);
            }
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
