using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Browsers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils
{
    public sealed class LocalWebApplicationFactory : WebApplicationFactory<Startup>, IDisposable
    {
        private const string LocalhostBaseAddress = "https://127.0.0.1";
        private const string BrowserName = "chrome";

        private readonly IWebHost host;

        public LocalWebApplicationFactory()
        {
            ClientOptions.BaseAddress = new Uri(LocalhostBaseAddress);

            host = CreateWebHostBuilder().Build();
            host.Start();

            RootUri = host.ServerFeatures.Get<IServerAddressesFeature>()!.Addresses.LastOrDefault();

            Driver = new BrowserFactory(BrowserName).Driver;
        }

        public string? RootUri { get; }

        public IWebDriver Driver { get; }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            var builder = WebHost.CreateDefaultBuilder(Array.Empty<string>());
            builder.UseWebRoot(Path.GetFullPath("../../../../../src/NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp/wwwroot"));
            builder.UseStartup<Startup>();

            builder.UseUrls($"{LocalhostBaseAddress}:0");

            return builder;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                host?.Dispose();
                Driver?.Dispose();
            }
        }
    }
}
