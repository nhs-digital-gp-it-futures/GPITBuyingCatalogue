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
    public sealed class LocalWebApplicationFactory : WebApplicationFactory<Startup>
    {
        private const string LocalhostBaseAddress = "https://127.0.0.1";

        private readonly IWebHost host;
        internal readonly string BcDbName;

        internal IWebDriver Driver { get; }

        // Need to find a better way of doing this
        private const string BC_DB_CONNECTION = "Server=localhost,1450;Database=GPITBuyingCatalogue;User=SA;password=8VSKwQ8xgk35qWFm8VSKwQ8xgk35qWFm!;Integrated Security=false";
        private const string CO_DB_CONNECTION = "Server=localhost,1450;Database=CatalogueOrdering;User=SA;password=8VSKwQ8xgk35qWFm8VSKwQ8xgk35qWFm!;Integrated Security=false";
        private const string OPERATING_MODE = "private";
        private const string BC_BLOB_CONNECTION = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://localhost:10100/devstoreaccount1;QueueEndpoint=http://localhost:10101/devstoreaccount1;TableEndpoint=http://localhost:10102/devstoreaccount1;";
        private const string BC_BLOB_CONTAINER = "buyingcatalogue-documents";
        private const string BC_SMTP_HOST = "localhost";
        private const string BC_SMTP_PORT = "9999";

        private const string Browser = "chrome";

        public LocalWebApplicationFactory()
        {
            ClientOptions.BaseAddress = new Uri(LocalhostBaseAddress);

            BcDbName = Guid.NewGuid().ToString();

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

        public string RootUri { get; }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            var builder = WebHost.CreateDefaultBuilder(Array.Empty<string>()).UseSerilog();
            builder.UseStartup<Startup>();
            builder.ConfigureServices(services =>
            {
                var dbTypes = new [] { typeof(GPITBuyingCatalogueDbContext) };

                foreach (var type in dbTypes)
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == type);
                    if (descriptor is not null)
                    {
                        services.Remove(descriptor);
                    }
                }

                services.AddDbContext<GPITBuyingCatalogueDbContext>(options =>
                {
                    options.UseInMemoryDatabase(BcDbName);
                });

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;

                var bcDb = scopedServices.GetRequiredService<GPITBuyingCatalogueDbContext>();

                bcDb.Database.EnsureCreated();

                try
                {
                    BuyingCatalogueSeedData.Initialize(bcDb);
                    UserSeedData.Initialize(bcDb);
                }
                catch
                {
                    // figure out error logging here
                }
            });

            builder.UseUrls($"{LocalhostBaseAddress}:0");
            return builder;
        }

        private static void SetEnvVariables()
        {
            SetEnvironmentVariable(nameof(BC_DB_CONNECTION), BC_DB_CONNECTION);

            SetEnvironmentVariable(nameof(CO_DB_CONNECTION), CO_DB_CONNECTION);

            SetEnvironmentVariable(nameof(OPERATING_MODE), OPERATING_MODE);

            SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

            SetEnvironmentVariable("SMTPSERVER__PORT", "9999");

            SetEnvironmentVariable(nameof(BC_BLOB_CONNECTION), BC_BLOB_CONNECTION);

            SetEnvironmentVariable(nameof(BC_BLOB_CONTAINER), BC_BLOB_CONTAINER);

            SetEnvironmentVariable(nameof(BC_SMTP_HOST), BC_SMTP_HOST);

            SetEnvironmentVariable(nameof(BC_SMTP_PORT), BC_SMTP_PORT);
        }

        private static void SetEnvironmentVariable(string name, string value)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name)))
            {
                Environment.SetEnvironmentVariable(name, value);
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
