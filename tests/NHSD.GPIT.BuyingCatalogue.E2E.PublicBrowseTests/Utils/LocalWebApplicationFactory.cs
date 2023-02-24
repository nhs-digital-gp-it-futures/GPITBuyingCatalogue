using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
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
using Testcontainers.MsSql;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils
{
    public sealed class LocalWebApplicationFactory : WebApplicationFactory<Startup>, IAsyncLifetime
    {
        private const string LocalhostBaseAddress = "https://127.0.0.1";

        // Need to find a better way of doing this
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "This name is used by the Webapp, so needs to be kept")]
        private const string BC_DB_CONNECTION = "Server=localhost,1450;Database=GPITBuyingCatalogue;User=SA;password=8VSKwQ8xgk35qWFm8VSKwQ8xgk35qWFm!;Integrated Security=false";

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "This name is used by the Webapp, so needs to be kept")]
        private const string DOMAIN_NAME = "127.0.0.1";

        private const string Browser = "chrome";

        private MsSqlContainer sqlContainer;
        private BrowserFactory browserFactory;
        private IHost host;

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

        public string RootUri { get; private set; }

        internal IWebDriver Driver => browserFactory.Driver;

        internal BuyingCatalogueDbContext DbContext
        {
            get
            {
                var scope = host.Services.CreateScope();

                return scope.ServiceProvider.GetRequiredService<BuyingCatalogueDbContext>();
            }
        }

        internal IDistributedCache GetDistributedCache => host.Services.GetRequiredService<IDistributedCache>();

        internal IDataProtectionProvider GetDataProtectionProvider => host.Services.GetRequiredService<IDataProtectionProvider>();

        internal ILoggerFactory GetLoggerFactory => host.Services.GetRequiredService<ILoggerFactory>();

        public async Task InitializeAsync()
        {
            sqlContainer = new MsSqlBuilder()
                .WithName(Guid.NewGuid().ToString())
                .WithPassword("Abc123Abc123")
                .WithCleanUp(true)
                .Build();

            await sqlContainer.StartAsync();
            SetEnvVariables();

            host = CreateHostBuilder().Build();
            await host.StartAsync();

            RootUri = host
                .Services
                .GetRequiredService<IServer>()!
                .Features
                .Get<IServerAddressesFeature>()!
                .Addresses
                .LastOrDefault();

            browserFactory = new(Browser);

            if (!Browser.Contains("local") && !Debugger.IsAttached && browserFactory.GridRunning)
            {
                RootUri = RootUri.Replace("127.0.0.1", "host.docker.internal");
            }
        }

        public new async Task DisposeAsync()
        {
            await sqlContainer.DisposeAsync();
            
            browserFactory?.Dispose();
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = base.CreateHostBuilder()!;
            builder.UseSerilog();
            builder.ConfigureWebHost(
                webHost =>
                {
                    webHost.UseKestrel();
                    webHost.UseWebRoot(Path.GetFullPath("../../../../../src/NHSD.GPIT.BuyingCatalogue.WebApp/wwwroot"));
                    webHost.UseUrls($"{LocalhostBaseAddress}:0");

                    webHost.ConfigureTestServices(
                        services =>
                        {
                            services.AddSingleton<IUrlValidator, StubbedUrlValidator>();
                            services.AddSingleton<IServiceRecipientImportService, StubbedServiceRecipientImportService>();
                            services.AddSingleton<IGpPracticeProvider, StubbedGpPracticeProvider>();

                            services.AddSession(
                                options =>
                                {
                                    options.IdleTimeout = TimeSpan.FromMinutes(60);
                                });
                        });
                });

            builder.ConfigureServices(services =>
            {
                services.AddHttpContextAccessor();
                services.AddDistributedMemoryCache();

                services.AddSingleton<IIdentityService>(new IdentityService(new HttpContextAccessor()));

                var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(BuyingCatalogueDbContext));
                if (descriptor is not null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<BuyingCatalogueDbContext, EndToEndDbContext>(
                    options =>
                    {
                        options.UseSqlServer(sqlContainer.GetConnectionString());
                        options.EnableSensitiveDataLogging();
                    });

                var sp = services.BuildServiceProvider();
                var scope = sp.CreateScope();
                var bcDb = scope.ServiceProvider.GetRequiredService<EndToEndDbContext>();

                bcDb.Database.EnsureCreated();

                try
                {
                    OdsOrganisationsSeedData.Initialize(bcDb);
                    RolesSeedData.Initialize(bcDb);
                    UserSeedData.Initialize(bcDb);
                    OdsOrganisationsSeedData.Initialize(bcDb);
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
            });

            builder.ConfigureAppConfiguration(config =>
            {
                foreach (var s in config.Sources)
                {
                    if (s is FileConfigurationSource source)
                        source.ReloadOnChange = false;
                }
            });

            return builder;
        }

        private static void SetEnvironmentVariable(string name, string value)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name)))
            {
                Environment.SetEnvironmentVariable(name, value);
            }
        }

        private void SetEnvVariables()
        {
            SetEnvironmentVariable(nameof(BC_DB_CONNECTION), sqlContainer.GetConnectionString());

            SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "E2ETest");

            SetEnvironmentVariable(nameof(DOMAIN_NAME), DOMAIN_NAME);

            SetEnvironmentVariable("DOTNET_USE_POLLING_FILE_WATCHER", "true");

            SetEnvironmentVariable("SESSION_IDLE_TIMEOUT", "60");
        }
    }
}
