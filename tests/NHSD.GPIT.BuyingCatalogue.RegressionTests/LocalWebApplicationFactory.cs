using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Browsers;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Environments;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.WebApp;
using OpenQA.Selenium;
using Serilog;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests
{
    public sealed class LocalWebApplicationFactory : WebApplicationFactory<Startup>
    {
        private const string LocalhostBaseAddress = "https://127.0.0.1";

        // Need to find a better way of doing this
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "This name is used by the Webapp, so needs to be kept")]
        private const string BC_DB_CONNECTION = "Server=localhost,1432;Database=GPITBuyingCatalogue;User=SA;password=Abc123Abc123;Integrated Security=false;TrustServerCertificate=true";

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "This name is used by the Webapp, so needs to be kept")]
        private const string DOMAIN_NAME = "127.0.0.1";

        private const string BcConnectionString = BC_DB_CONNECTION;

        private const string Browser = "chrome";

        private readonly IHost host;

        private bool disposed = false;

        private SqlConnection sqlConnection;

        public LocalWebApplicationFactory()
        {
            ClientOptions.BaseAddress = new Uri(LocalhostBaseAddress);

            SetEnvVariables();

            sqlConnection?.Dispose();

            sqlConnection = new SqlConnection(BcConnectionString);
            sqlConnection.Open();

            host = CreateHostBuilder().Build();
            CurrentEnvironment.IsDevelopment = true;
            host.Start();

            RootUri = host.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.LastOrDefault()!;

            var browserFactory = new BrowserFactory(Browser);
            BrowserFactory = browserFactory;

            if (!Browser.Contains("local") && !Debugger.IsAttached && browserFactory.GridRunning)
            {
                RootUri = RootUri.Replace("127.0.0.1", "host.docker.internal");
            }
        }

        ~LocalWebApplicationFactory()
        {
            Dispose(true);
        }

        public string RootUri { get; }

        internal BrowserFactory BrowserFactory { get; }

        internal IWebDriver Driver => BrowserFactory.Driver;

        internal IIdentityService GetIdentityService => host.Services.GetRequiredService<IIdentityService>();

        internal IDistributedCache GetDistributedCache => host.Services.GetRequiredService<IDistributedCache>();

        internal IMemoryCache GetMemoryCache => host.Services.GetRequiredService<IMemoryCache>();

        internal IDataProtectionProvider GetDataProtectionProvider => host.Services.GetRequiredService<IDataProtectionProvider>();

        internal ILoggerFactory GetLoggerFactory => host.Services.GetRequiredService<ILoggerFactory>();

        internal BuyingCatalogueDbContext DbContext
        {
            get
            {
                var options = new DbContextOptionsBuilder<BuyingCatalogueDbContext>()
                    .UseSqlServer(sqlConnection)
                    .EnableSensitiveDataLogging()
                    .Options;

                return new BuyingCatalogueDbContext(options, GetIdentityService);
            }
        }

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

                services.AddDbContext<BuyingCatalogueDbContext>(options =>
                {
                    options.UseSqlServer(sqlConnection);
                    options.EnableSensitiveDataLogging();
                });

                services.AddHttpContextAccessor();

                services.AddSingleton<IIdentityService>(new IdentityService(new HttpContextAccessor()));

                services.AddDistributedMemoryCache();

                services.AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromMinutes(60);
                });
            });

            _ = builder.ConfigureAppConfiguration((hostContext, config) =>
            {
                foreach (var s in config.Sources)
                {
                    if (s is FileConfigurationSource source)
                        source.ReloadOnChange = false;
                }
            });
            return builder;
        }

        [ExcludeFromCodeCoverage]
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && !disposed)
            {
                BrowserFactory?.Dispose();
                host?.Dispose();
                sqlConnection?.Dispose();
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

            SetEnvironmentVariable("RegressionBuyerUsername", "suesmith@email.com");

            SetEnvironmentVariable("RegressionBuyerPassword", "Pass123$");

            SetEnvironmentVariable("RegressionAdminUsername", "bobsmith@email.com");

            SetEnvironmentVariable("RegressionAdminPassword", "Pass123$");
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
