using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Database;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;
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

        // Need to find a better way of doing this
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "This name is used by the Webapp, so needs to be kept")]
        private const string BC_DB_CONNECTION = "Server=localhost,1450;Database=GPITBuyingCatalogue;User=SA;password=8VSKwQ8xgk35qWFm8VSKwQ8xgk35qWFm!;Integrated Security=false";

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "This name is used by the Webapp, so needs to be kept")]
        private const string BC_BLOB_CONNECTION = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://localhost:10100/devstoreaccount1;QueueEndpoint=http://localhost:10101/devstoreaccount1;TableEndpoint=http://localhost:10102/devstoreaccount1;";

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "This name is used by the Webapp, so needs to be kept")]
        private const string BC_BLOB_CONTAINER = "buyingcatalogue-documents";

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "This name is used by the Webapp, so needs to be kept")]
        private const string BC_SMTP_HOST = "localhost";

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "This name is used by the Webapp, so needs to be kept")]
        private const string BC_SMTP_PORT = "9999";

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "This name is used by the Webapp, so needs to be kept")]
        private const string DOMAIN_NAME = "127.0.0.1";

        private const string SqlliteConnectionString = "DataSource=:memory:"; //"Data Source=C:\\test.db";

        private const string Browser = "chrome";
        private readonly IWebHost host;

        private SqliteConnection sqliteConnection;

        private IServiceProvider scopedServices;

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

        public static int SmtpPort => int.Parse(BC_SMTP_PORT);

        public string BcDbName { get; private set; }

        public string RootUri { get; }

        internal IWebDriver Driver { get; }

        internal EndToEndDbContext DbContext
        {
            get
            {
                var options = new DbContextOptionsBuilder<EndToEndDbContext>()
                    .UseSqlite(sqliteConnection)
                    .Options;

                return new EndToEndDbContext(options);
            }
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            var builder = WebHost.CreateDefaultBuilder(Array.Empty<string>()).UseSerilog();
            builder.UseWebRoot(System.IO.Path.GetFullPath("../../../../../src/NHSD.GPIT.BuyingCatalogue.WebApp/wwwroot"));
            builder.UseStartup<Startup>();
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(BuyingCatalogueDbContext));
                if (descriptor is not null)
                {
                    services.Remove(descriptor);
                }

                sqliteConnection?.Dispose();
                sqliteConnection = new SqliteConnection(SqlliteConnectionString);
                sqliteConnection.Open();

                services.AddDbContext<EndToEndDbContext>(options =>
                {
                    options.UseSqlite(sqliteConnection);
                });
                services.AddDbContext<BuyingCatalogueDbContext, EndToEndDbContext>();

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                scopedServices = scope.ServiceProvider;

                var bcDb = scopedServices.GetRequiredService<EndToEndDbContext>();

                bcDb.Database.EnsureCreated();

                services.AddDistributedMemoryCache();

                services.AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromMinutes(60);
                });

                try
                {
                    ApplyViewsAndTables(bcDb);
                    BuyingCatalogueSeedData.Initialize(bcDb);
                    UserSeedData.Initialize(bcDb);
                    OrderSeedData.Initialize(bcDb);
                }
                catch (Exception ex)
                {
                    // figure out error logging here
                    Trace.WriteLine(ex.Message);
                }
            });

            builder.UseUrls($"{LocalhostBaseAddress}:0");

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
                sqliteConnection?.Dispose();
            }
        }

        private static void SetEnvVariables()
        {
            SetEnvironmentVariable(nameof(BC_DB_CONNECTION), BC_DB_CONNECTION);

            SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "E2ETest");

            SetEnvironmentVariable("SMTPSERVER__PORT", BC_SMTP_PORT);

            SetEnvironmentVariable(nameof(BC_BLOB_CONNECTION), BC_BLOB_CONNECTION);

            SetEnvironmentVariable(nameof(BC_BLOB_CONTAINER), BC_BLOB_CONTAINER);

            SetEnvironmentVariable(nameof(BC_SMTP_HOST), BC_SMTP_HOST);

            SetEnvironmentVariable(nameof(BC_SMTP_PORT), BC_SMTP_PORT);

            SetEnvironmentVariable(nameof(DOMAIN_NAME), DOMAIN_NAME);
        }

        private static void SetEnvironmentVariable(string name, string value)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name)))
            {
                Environment.SetEnvironmentVariable(name, value);
            }
        }

        private void ApplyViewsAndTables(EndToEndDbContext context)
        {
            context.Database.ExecuteSqlRaw(
                @"CREATE VIEW ServiceInstanceItems AS
                    WITH ServiceInstanceIncrement AS
                    (
                        SELECT r.OrderId, r.CatalogueItemId, r.OdsCode,
                                DENSE_RANK() OVER (
                                    PARTITION BY r.OrderId, r.OdsCode
                                        ORDER BY CASE WHEN c.CatalogueItemTypeId = 1 THEN r.CatalogueItemId ELSE a.SolutionId END) AS ServiceInstanceIncrement
                            FROM OrderItemRecipients AS r
                                INNER JOIN CatalogueItems AS c ON c.CatalogueItemId = r.CatalogueItemId
                                        AND c.CatalogueItemTypeId IN (1, 2)
                                LEFT OUTER JOIN AdditionalServices AS a ON a.CatalogueItemId = r.CatalogueItemId
                            WHERE (a.SolutionId IS NULL OR EXISTS (
                                SELECT *
                                    FROM OrderItemRecipients AS r2
                                    WHERE r2.OrderId = r.OrderId
                                    AND r2.OdsCode = r.OdsCode
                                    AND r2.CatalogueItemId = a.SolutionId))
                    )
                    SELECT r.OrderId, r.CatalogueItemId, r.OdsCode,
                            'SI' + CAST(s.ServiceInstanceIncrement AS nvarchar(3)) + '-' + r.OdsCode AS ServiceInstanceId
                        FROM OrderItemRecipients AS r
                            LEFT OUTER JOIN ServiceInstanceIncrement AS s
                                    ON s.OrderId = r.OrderId
                                    AND s.CatalogueItemId = r.CatalogueItemId
                                    AND s.OdsCode = r.OdsCode;");

            context.Database.ExecuteSqlRaw(
                @"CREATE TABLE SessionData
                (
                    Id nvarchar(449) NOT NULL,
                    [Value] varbinary(255) NOT NULL,
                    ExpiresAtTime datetimeoffset(7) NOT NULL,
                    SlidingExpirationInSeconds bigint NULL,
                    AbsoluteExpiration datetimeoffset(7) NULL,
                    CONSTRAINT PK_Sessions PRIMARY KEY (Id)
                );
                CREATE UNIQUE INDEX IX_SessionData_ExpiresAtTime
                ON SessionData(ExpiresAtTime);");
        }
    }
}
