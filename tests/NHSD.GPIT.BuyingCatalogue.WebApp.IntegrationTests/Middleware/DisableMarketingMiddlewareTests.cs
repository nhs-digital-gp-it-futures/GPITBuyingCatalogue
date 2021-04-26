using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using NHSD.GPIT.BuyingCatalogue.Framework.Middleware;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.IntegrationTests.Middleware
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public static class DisableMarketingMiddlewareTests
    {
        [Test]
        public static async Task MiddlewareTest_ReturnsNotFoundForRequest()
        {
            using var host = await new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .ConfigureServices(services =>
                        {
                            // services.AddMyServices();
                        })
                        .Configure(app =>
                        {
                            app.UseMiddleware<DisableMarketingMiddleware>();
                        });
                })
                .StartAsync();

            var response = await host.GetTestClient().GetAsync("/");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }


        [Test]
        public static async Task ShouldReturnHelloWorld()
        {
            Environment.SetEnvironmentVariable("BC_DB_CONNECTION", "Some Connection String");
            Environment.SetEnvironmentVariable("ID_DB_CONNECTION", "Some Connection String");


            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    // Add TestServer
                    webHost.UseTestServer();
                    webHost.UseStartup<NHSD.GPIT.BuyingCatalogue.WebApp.Startup>();
                    //webHost.Configure(app =>
                    //{
                    //    app.UseMiddleware<DisableMarketingMiddleware>();
                    //});
                    //webHost.Configure(app => app.Run(async ctx =>
                    //{

                    //await ctx.Response.WriteAsync("Hello World!");
                    //}
                    //));
                });

            // Build and start the IHost
            var host = await hostBuilder.StartAsync();

            // Create an HttpClient to send requests to the TestServer
            var client = host.GetTestClient();

            var response = await client.GetAsync("/marketing");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("Hello World!", responseString);
        }
    }
}
