using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Database;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Browsers;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.Services;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.WebApp;
using OpenQA.Selenium;
using Serilog;
using Serilog.Events;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils
{
    public sealed class LocalWebApplicationFactory : WebApplicationFactory<Startup>
    {
        private const string LocalhostBaseAddress = "https://127.0.0.1";

        // Need to find a better way of doing this
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "This name is used by the Webapp, so needs to be kept")]
        private const string BC_DB_CONNECTION = "Server=localhost,1450;Database=GPITBuyingCatalogue;User=SA;password=8VSKwQ8xgk35qWFm8VSKwQ8xgk35qWFm!;Integrated Security=false";

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "This name is used by the Webapp, so needs to be kept")]
        private const string DOMAIN_NAME = "127.0.0.1";

        private const string Browser = "chrome";

        // Sqllite constants
        private const string SqlliteConnectionStringInMemory = "DataSource=:memory:";

        private const string SqlliteFileSystemFileLocation = "C:/test.db";

        private const bool UseFileSystemSqlite = false;

        private static readonly string SqlliteConnectionStringFileSystem = $"DataSource={SqlliteFileSystemFileLocation}";

        private readonly IHost host;

        private bool disposed = false;

        private SqliteConnection sqliteConnection;

        private IServiceProvider scopedServices;

        public LocalWebApplicationFactory()
        {
            ClientOptions.BaseAddress = new Uri(LocalhostBaseAddress);

            BcDbName = Guid.NewGuid().ToString();

            SetEnvVariables();

            host = CreateHostBuilder().Build();
            host.Start();

            RootUri = host.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>().Addresses.LastOrDefault();

            var browserFactory = new BrowserFactory(Browser);
            Driver = browserFactory.Driver;

            if (!Browser.Contains("local") && !Debugger.IsAttached && browserFactory.GridRunning)
            {
                RootUri = RootUri.Replace("127.0.0.1", "host.docker.internal");
            }
        }

        ~LocalWebApplicationFactory()
        {
            Dispose(true);
        }

        public static ITestOutputHelper TestOutputHelper
        {
            set
            {
                if (value != null)
                {
                    Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Information()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                        .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                        .Enrich.FromLogContext()
                        .WriteTo.TestOutput(value)
                        .CreateLogger();
                }
            }
        }

        public string BcDbName { get; private set; }

        public string RootUri { get; }

        internal IWebDriver Driver { get; }

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

        protected override IHostBuilder CreateHostBuilder()
        {
            IHostBuilder builder = Host.CreateDefaultBuilder();
            builder.UseSerilog();
            builder.ConfigureWebHost(webHost =>
            {
                webHost.UseKestrel();
                webHost.UseWebRoot(Path.GetFullPath("../../../../../src/NHSD.GPIT.BuyingCatalogue.WebApp/wwwroot"));
                webHost.UseStartup<Startup>();
                webHost.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IUrlValidator, StubbedUrlValidator>();
                    services.AddSingleton<IServiceRecipientImportService, StubbedServiceRecipientImportService>();
                    services.AddSingleton<IGpPracticeProvider, StubbedGpPracticeProvider>();
                });
                webHost.UseUrls($"{LocalhostBaseAddress}:0");
            });
            builder.ConfigureServices(services =>
            {
                var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(BuyingCatalogueDbContext));
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
                    RolesSeedData.Initialize(bcDb);
                    UserSeedData.Initialize(bcDb);
                    BuyingCatalogueSeedData.Initialize(bcDb);
                    OrderSeedData.Initialize(bcDb);
                    ContractSeedData.Initialize(bcDb);
                    EmailDomainSeedData.Initialize(bcDb);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                    throw;
                }

                bcDb.Dispose();
            });

            builder.ConfigureAppConfiguration((hostContext, config) =>
            {
                foreach (var s in config.Sources)
                {
                    if (s is FileConfigurationSource)
                        ((FileConfigurationSource)s).ReloadOnChange = false;
                }
            });
            return builder;
        }

        [ExcludeFromCodeCoverage]
        protected override void Dispose(bool disposing)
        {
            Driver?.Quit();
            base.Dispose(disposing);
            if (disposing && !disposed)
            {
                host?.Dispose();
                sqliteConnection?.Dispose();
                disposed = true;
            }
        }

        private static void SetEnvVariables()
        {
            SetEnvironmentVariable(nameof(BC_DB_CONNECTION), BC_DB_CONNECTION);

            SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "E2ETest");

            SetEnvironmentVariable(nameof(DOMAIN_NAME), DOMAIN_NAME);

            SetEnvironmentVariable("DOTNET_USE_POLLING_FILE_WATCHER", "true");

            SetEnvironmentVariable("SESSION_IDLE_TIMEOUT", "60");
        }

        private static void SetEnvironmentVariable(string name, string value)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name)))
            {
                Environment.SetEnvironmentVariable(name, value);
            }
        }
    }
}
