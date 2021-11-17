using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Database;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;
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
        private const string BC_SMTP_HOST = "localhost";

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "This name is used by the Webapp, so needs to be kept")]
        private const string BC_SMTP_PORT = "9999";

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "This name is used by the Webapp, so needs to be kept")]
        private const string DOMAIN_NAME = "127.0.0.1";

        private const string Browser = "chrome";

        private const string SqlliteFileSystemFileLocation = "C:/test.db";

        private const bool UseFileSystemSqlite = false;

        private static readonly string SqlliteConnectionStringFileSystem = $"DataSource={SqlliteFileSystemFileLocation}";

        private readonly string pathToServiceInstanceViewSqlFile =
            Path.GetFullPath(Path.Combine(
                Directory.GetCurrentDirectory(),
                @"../../../../../database/NHSD.GPITBuyingCatalogue.Database/Ordering/Views",
                @"ServiceInstanceItems.sql"));

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

        internal string SqlliteConnectionStringInMemory => $"DataSource={Guid.NewGuid()};Mode=memory";

        internal EndToEndDbContext DbContext
        {
            get
            {
                var options = new DbContextOptionsBuilder<EndToEndDbContext>()
                    .UseSqlite(sqliteConnection)
                    .EnableSensitiveDataLogging()
                    .Options;

                return new EndToEndDbContext(options, GetIdentityService);
            }
        }

        internal IIdentityService GetIdentityService => host.Services.GetRequiredService<IIdentityService>();

        internal IDistributedCache GetDistributedCache => host.Services.GetRequiredService<IDistributedCache>();

        internal IMemoryCache GetMemoryCache => host.Services.GetRequiredService<IMemoryCache>();

        internal IDataProtectionProvider GetDataProtectionProvider => host.Services.GetRequiredService<IDataProtectionProvider>();

        internal ILoggerFactory GetLoggerFactory => host.Services.GetRequiredService<ILoggerFactory>();

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            var builder = WebHost.CreateDefaultBuilder(Array.Empty<string>()).UseSerilog();
            builder.UseWebRoot(Path.GetFullPath("../../../../../src/NHSD.GPIT.BuyingCatalogue.WebApp/wwwroot"));
            builder.UseStartup<Startup>();
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(BuyingCatalogueDbContext));
                if (descriptor is not null)
                {
                    services.Remove(descriptor);
                }

                sqliteConnection?.Dispose();

                if (UseFileSystemSqlite && File.Exists(SqlliteFileSystemFileLocation))
                {
                    File.Delete(SqlliteFileSystemFileLocation);
                }

                sqliteConnection = new SqliteConnection(
                    UseFileSystemSqlite ? SqlliteConnectionStringFileSystem : SqlliteConnectionStringInMemory);
                sqliteConnection.Open();

                services.AddDbContext<EndToEndDbContext>(options =>
                {
                    options.UseSqlite(sqliteConnection);
                    options.EnableSensitiveDataLogging();
                });
                services.AddDbContext<BuyingCatalogueDbContext, EndToEndDbContext>();

                services.AddHttpContextAccessor();

                services.AddSingleton<IIdentityService>(new IdentityService(new HttpContextAccessor()));

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
                    UserSeedData.Initialize(bcDb);
                    BuyingCatalogueSeedData.Initialize(bcDb);
                    OrderSeedData.Initialize(bcDb);
                }
                catch (Exception ex)
                {
                    // figure out error logging here
                    Trace.WriteLine(ex.Message);
                }

                bcDb.Dispose();
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
            var serviceInstanceItemsSql = File.ReadAllText(pathToServiceInstanceViewSqlFile);

            if (string.IsNullOrWhiteSpace(serviceInstanceItemsSql))
                throw new FormatException($"{nameof(serviceInstanceItemsSql)} was empty when it shouldn't be.");

            // remove ordering and catalogue two part name
            serviceInstanceItemsSql = serviceInstanceItemsSql.Replace("ordering.", string.Empty);
            serviceInstanceItemsSql = serviceInstanceItemsSql.Replace("catalogue.", string.Empty);

            context.Database.ExecuteSqlRaw(serviceInstanceItemsSql);
        }
    }
}
