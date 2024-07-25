using System;
using System.IO;
using NHSD.GPIT.BuyingCatalogue.Identity.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using OpenIddict.Client;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace NHSD.GPIT.BuyingCatalogue.Identity;

public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(
            options =>
            {
                options.UseSqlite($"Filename={Path.Combine(Path.GetTempPath(), "openiddict-identity.sqlite3")}");

                options.UseOpenIddict();
            });

        services.AddDbContext<BuyingCatalogueDbContext>(
            options =>
            {
                var connectionString = Configuration.GetConnectionString("BuyingCatalogue");
                options.UseSqlServer(connectionString);
            });

        services.AddIdentity<AspNetUser, AspNetRole>()
            .AddEntityFrameworkStores<BuyingCatalogueDbContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI();

        services.AddOpenIddict()
            .AddCore(
                options =>
                {
                    options.UseEntityFrameworkCore()
                        .UseDbContext<ApplicationDbContext>();
                })
            .AddClient(
                options =>
                {
                    options.AllowAuthorizationCodeFlow()
                        .AllowRefreshTokenFlow();

                    options.AddDevelopmentEncryptionCertificate()
                        .AddDevelopmentSigningCertificate();

                    options.UseAspNetCore()
                        .EnableStatusCodePagesIntegration()
                        .EnableRedirectionEndpointPassthrough();

                    options.UseSystemNetHttp()
                        .SetProductInformation(typeof(Startup).Assembly);

                    options.AddRegistration(
                        new OpenIddictClientRegistration
                        {
                            Issuer = new Uri("http://localhost:44310/"),
                            ClientId = "BuyingCatalogueTest",
                            ClientSecret = "test-data-secret",
                            GrantTypes = { GrantTypes.AuthorizationCode, GrantTypes.RefreshToken },
                            PostLogoutRedirectUri = new Uri("https://localhost:5001/signout-callback-oidc"),
                            RedirectUri = new Uri("https://localhost:5001/signin-oidc"),
                            Scopes = { Scopes.Email, Scopes.Profile, Scopes.OpenId },
                        });
                })
            .AddServer(
                options =>
                {
                    options.SetAuthorizationEndpointUris("connect/authorize")
                        .SetLogoutEndpointUris("connect/logout")
                        .SetTokenEndpointUris("connect/token")
                        .SetUserinfoEndpointUris("connect/userinfo");

                    options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.OpenId);

                    options.AllowAuthorizationCodeFlow()
                        .AllowRefreshTokenFlow();

                    options.AddDevelopmentEncryptionCertificate()
                        .AddDevelopmentSigningCertificate();

                    options.UseAspNetCore()
                        .DisableTransportSecurityRequirement()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableLogoutEndpointPassthrough()
                        .EnableStatusCodePagesIntegration()
                        .EnableTokenEndpointPassthrough();
                })
            .AddValidation(
                options =>
                {
                    options.UseLocalServer();
                    options.UseAspNetCore();
                });

        services.AddControllersWithViews();
        services.AddRazorPages();

        services.AddHostedService<Worker>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseDeveloperExceptionPage();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(
            endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
    }
}
