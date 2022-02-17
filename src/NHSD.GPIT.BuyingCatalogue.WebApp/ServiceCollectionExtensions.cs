using System;
using System.IO.Compression;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.Services.Email;
using NHSD.GPIT.BuyingCatalogue.Services.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;
using Notify.Client;
using Notify.Interfaces;

namespace NHSD.GPIT.BuyingCatalogue.WebApp
{
    public static class ServiceCollectionExtensions
    {
        private const string BuyingCatalogueDbConnectionEnvironmentVariable = "BC_DB_CONNECTION";
        private const string BuyingCatalogueDomainNameEnvironmentVariable = "DOMAIN_NAME";
        private const string BuyingCataloguePdfEnvironmentVariable = "USE_SSL_FOR_PDF";

        public static void ConfigureAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "AdminOnly",
                    policy => policy.RequireClaim(
                        "organisationFunction",
                        new[] { OrganisationFunction.Authority.DisplayName }));
            });
        }

        public static void ConfigureCookies(this IServiceCollection services, IConfiguration configuration)
        {
            var cookieExpiration = configuration.GetSection("cookieExpiration").Get<CookieExpirationSettings>();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "user-session";
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.ExpireTimeSpan = cookieExpiration.ExpireTimeSpan;
                options.SlidingExpiration = cookieExpiration.SlidingExpiration;
                options.AccessDeniedPath = "/404";

                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = ctx =>
                    {
                        var relativeRedirectUri = new Uri(ctx.RedirectUri).PathAndQuery;
                        ctx.Response.Headers[HeaderNames.Location] = relativeRedirectUri;
                        ctx.Response.StatusCode = StatusCodes.Status302Found;
                        return Task.CompletedTask;
                    },
                };
            });

            services.AddAntiforgery(options =>
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.Name = "antiforgery";
            });
        }

        public static void ConfigureOds(this IServiceCollection services, IConfiguration configuration)
        {
            var odsSettings = configuration.GetSection("Ods").Get<OdsSettings>();

            services.AddSingleton(odsSettings);
        }

        public static void ConfigureDbContexts(this IServiceCollection services)
        {
            var buyingCatalogueConnectionString = Environment.GetEnvironmentVariable(BuyingCatalogueDbConnectionEnvironmentVariable);

            if (string.IsNullOrWhiteSpace(buyingCatalogueConnectionString))
                throw new InvalidOperationException($"Environment variable '{BuyingCatalogueDbConnectionEnvironmentVariable}' must be set for the database connection string");

            services.AddDbContext<BuyingCatalogueDbContext>(options => options.UseSqlServer(buyingCatalogueConnectionString));
        }

        public static void ConfigureSession(this IServiceCollection services)
        {
            var buyingCatalogueConnectionString = Environment.GetEnvironmentVariable(BuyingCatalogueDbConnectionEnvironmentVariable);

            if (string.IsNullOrWhiteSpace(buyingCatalogueConnectionString))
                throw new InvalidOperationException($"Environment variable '{BuyingCatalogueDbConnectionEnvironmentVariable}' must be set for the database connection string");

            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = buyingCatalogueConnectionString;
                options.SchemaName = "cache";
                options.TableName = "SessionData";
            });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });
        }

        public static void ConfigureDisabledErrorMessage(this IServiceCollection services, IConfiguration configuration)
        {
            var disabledErrorMessage = configuration.GetSection("disabledErrorMessage").Get<DisabledErrorMessageSettings>();
            services.AddSingleton(disabledErrorMessage);
        }

        public static void ConfigureDomainName(this IServiceCollection services)
        {
            var domain = Environment.GetEnvironmentVariable(BuyingCatalogueDomainNameEnvironmentVariable);

            if (string.IsNullOrWhiteSpace(domain))
                throw new InvalidOperationException($"Environment variable '{BuyingCatalogueDomainNameEnvironmentVariable}' must be set for the domain name");

            var domainNameSettings = new DomainNameSettings { DomainName = domain };
            services.AddSingleton(domainNameSettings);
        }

        public static void ConfigurePdf(this IServiceCollection services)
        {
            var useSsl = Environment.GetEnvironmentVariable(BuyingCataloguePdfEnvironmentVariable);

            if (string.IsNullOrWhiteSpace(useSsl))
                useSsl = "false";

            var pdfSettings = new PdfSettings { UseSslForPdf = useSsl.EqualsIgnoreCase("true") };
            services.AddSingleton(pdfSettings);
        }

        public static IServiceCollection ConfigureConsentCookieSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var cookieExpiration = configuration.GetSection("cookieExpiration").Get<CookieExpirationSettings>();
            cookieExpiration.ConsentExpiration = configuration.GetValue<TimeSpan>(CatalogueCookies.BuyingCatalogueConsentExpiration);

            services.AddSingleton(cookieExpiration);

            return services;
        }

        public static IServiceCollection ConfigureCacheKeySettings(this IServiceCollection services, IConfiguration configuration)
        {
            var filterCacheKeySettings = configuration.GetSection(FilterCacheKeysSettings.SectionName).Get<FilterCacheKeysSettings>();
            services.AddSingleton(filterCacheKeySettings);

            return services;
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddTransient<IUserClaimsPrincipalFactory<AspNetUser>, CatalogueUserClaimsPrincipalFactory>();

            services.AddIdentity<AspNetUser, AspNetRole>(o =>
                {
                    o.Password.RequireDigit = false;
                    o.Password.RequireLowercase = false;
                    o.Password.RequireNonAlphanumeric = false;
                    o.Password.RequireUppercase = false;
                    o.Password.RequiredLength = 10;
                    o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    o.Lockout.MaxFailedAccessAttempts = 6;
                })
                .AddEntityFrameworkStores<BuyingCatalogueDbContext>()
                .AddTokenProvider<DataProtectorTokenProvider<AspNetUser>>(TokenOptions.DefaultProvider)
                .AddPasswordValidator<PasswordValidator>();
        }

        public static void ConfigurePasswordReset(this IServiceCollection services, IConfiguration configuration)
        {
            var passwordResetSettings = configuration.GetSection("passwordReset").Get<PasswordResetSettings>();
            services.AddSingleton(passwordResetSettings);
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IPasswordResetCallback, PasswordResetCallback>();
        }

        public static IServiceCollection ConfigureGovNotify(this IServiceCollection services, IConfiguration configuration)
        {
            var notifyApiKey = Environment.GetEnvironmentVariable("NOTIFY_API_KEY");
            if (!string.IsNullOrWhiteSpace(notifyApiKey))
            {
                services.AddScoped<IAsyncNotificationClient, NotificationClient>(sp => new NotificationClient(notifyApiKey));
                services.AddScoped<IGovNotifyEmailService, GovNotifyEmailService>();
            }
            else
            {
                services.AddScoped<IGovNotifyEmailService, FakeGovNotifyEmailService>();
            }

            return services;
        }

        public static IServiceCollection ConfigureAnalyticsSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var analyticsSettings = configuration.GetSection(AnalyticsSettings.Key).Get<AnalyticsSettings>();
            services.AddSingleton(analyticsSettings);

            return services;
        }

        public static IServiceCollection ConfigureNominateOrganisationMessageSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection("nominateOrganisationMessage").Get<NominateOrganisationMessageSettings>();
            services.AddSingleton(settings);

            return services;
        }

        public static IServiceCollection ConfigureOrderMessageSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var orderMessageSettings = configuration.GetSection("orderMessage").Get<OrderMessageSettings>();
            services.AddSingleton(orderMessageSettings);

            return services;
        }

        public static IServiceCollection ConfigureRequestAccountMessageSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection("requestAccountMessage").Get<RequestAccountMessageSettings>();
            services.AddSingleton(settings);

            return services;
        }

        public static void ConfigureRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            var registrationSettings = configuration.GetSection("Registration").Get<RegistrationSettings>();
            services.AddSingleton(registrationSettings);
        }

        public static void ConfigureDataProtection(this IServiceCollection services, IConfiguration configuration)
        {
            var dataProtectionAppName = configuration.GetValue<string>("dataProtection:applicationName");

            services.AddDataProtection()
                .SetApplicationName(dataProtectionAppName)
                .PersistKeysToDbContext<BuyingCatalogueDbContext>();
        }

        public static void ConfigureResponseCompression(this IServiceCollection services)
        {
            services.AddResponseCompression(opt =>
            {
                opt.Providers.Add<GzipCompressionProvider>();
                opt.Providers.Add<BrotliCompressionProvider>();
                opt.EnableForHttps = true;
            });

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
        }

        public static IServiceCollection AddFluentValidation(this IServiceCollection services)
        {
            return services.AddFluentValidation(
                    options =>
                    {
                        options.RegisterValidatorsFromAssemblyContaining<SolutionModelValidator>();
                    }).AddSingleton<IValidatorInterceptor, FluentValidatorInterceptor>();
        }
    }
}
