﻿using System;
using System.IO.Compression;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.Services.Email;
using NHSD.GPIT.BuyingCatalogue.Services.Identity;
using NHSD.GPIT.BuyingCatalogue.Services.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;
using Notify.Client;
using Notify.Interfaces;

namespace NHSD.GPIT.BuyingCatalogue.WebApp
{
    public static class ServiceCollectionExtensions
    {
        private const int DefaultSessionTimeout = 60;
        private const string BuyingCatalogueDbConnectionEnvironmentVariable = "BC_DB_CONNECTION";
        private const string BuyingCatalogueDomainNameEnvironmentVariable = "DOMAIN_NAME";
        private const string BuyingCataloguePdfEnvironmentVariable = "USE_SSL_FOR_PDF";
        private const string SessionIdleTimeoutEnvironmentVariable = "SESSION_IDLE_TIMEOUT";

        public static void ConfigureAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "AdminOnly",
                    policy => policy.RequireClaim(
                        "organisationFunction",
                        new[] { OrganisationFunction.Authority.Name }));

                options.AddPolicy(
                    "Buyer",
                    policy => policy.RequireClaim(
                        "organisationFunction",
                        new[] { OrganisationFunction.Buyer.Name, OrganisationFunction.AccountManager.Name }));

                options.AddPolicy(
                    "AccountManager",
                    policy => policy.RequireClaim(
                        "organisationFunction",
                        new[] { OrganisationFunction.AccountManager.Name }));
            });
        }

        public static void ConfigureCookies(this IServiceCollection services, IConfiguration configuration)
        {
            var cookieExpiration = configuration.GetSection("cookieExpiration").Get<CookieExpirationSettings>();

            var sessionIdleTimeout = configuration.GetValue<int?>(SessionIdleTimeoutEnvironmentVariable) ?? DefaultSessionTimeout;

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "user-session";
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(sessionIdleTimeout);
                options.SlidingExpiration = cookieExpiration.SlidingExpiration;
                options.AccessDeniedPath = "/unauthorized";

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

            if (odsSettings.UseTrud)
                services.AddScoped<IOdsService, TrudOdsService>();
            else
                services.AddScoped<IOdsService, OdsService>();
        }

        public static void ConfigureDbContexts(this IServiceCollection services, IConfiguration configuration)
        {
            var buyingCatalogueConnectionString = configuration.GetValue<string>(BuyingCatalogueDbConnectionEnvironmentVariable);

            if (string.IsNullOrWhiteSpace(buyingCatalogueConnectionString))
                throw new InvalidOperationException($"Environment variable '{BuyingCatalogueDbConnectionEnvironmentVariable}' must be set for the database connection string");

            services.AddDbContext<BuyingCatalogueDbContext>(options => options.UseSqlServer(buyingCatalogueConnectionString));
        }

        public static void ConfigureSession(this IServiceCollection services, IConfiguration configuration)
        {
            var buyingCatalogueConnectionString = configuration.GetValue<string>(BuyingCatalogueDbConnectionEnvironmentVariable);

            if (string.IsNullOrWhiteSpace(buyingCatalogueConnectionString))
                throw new InvalidOperationException($"Environment variable '{BuyingCatalogueDbConnectionEnvironmentVariable}' must be set for the database connection string");

            var sessionIdleTimeout = configuration.GetValue<int?>(SessionIdleTimeoutEnvironmentVariable) ?? DefaultSessionTimeout;

            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = buyingCatalogueConnectionString;
                options.SchemaName = "cache";
                options.TableName = "SessionData";
            });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(sessionIdleTimeout);
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });
        }

        public static void ConfigureDisabledErrorMessage(this IServiceCollection services, IConfiguration configuration)
        {
            var disabledErrorMessage = configuration.GetSection("disabledErrorMessage").Get<DisabledErrorMessageSettings>();
            services.AddSingleton(disabledErrorMessage);
        }

        public static void ConfigureDomainName(this IServiceCollection services, IConfiguration configuration)
        {
            var domain = configuration.GetValue<string>(BuyingCatalogueDomainNameEnvironmentVariable);

            if (string.IsNullOrWhiteSpace(domain))
                throw new InvalidOperationException($"Environment variable '{BuyingCatalogueDomainNameEnvironmentVariable}' must be set for the domain name");

            var domainNameSettings = new DomainNameSettings { DomainName = domain };
            services.AddSingleton(domainNameSettings);
        }

        public static void ConfigurePdf(this IServiceCollection services, IConfiguration configuration)
        {
            var useSsl = configuration.GetValue<bool?>(BuyingCataloguePdfEnvironmentVariable) ?? false;

            var pdfSettings = new PdfSettings { UseSslForPdf = useSsl };
            services.AddSingleton(pdfSettings);
        }

        public static IServiceCollection ConfigurePriceTiersCap(this IServiceCollection services, IConfiguration configuration)
        {
            var priceTiersCapSettings = configuration.GetSection("priceTiersCap").Get<PriceTiersCapSettings>();
            services.AddSingleton(priceTiersCapSettings);

            return services;
        }

        public static IServiceCollection ConfigureAccountManagement(this IServiceCollection services, IConfiguration configuration)
        {
            var accountManagementSettings = configuration.GetSection("accountManagement").Get<AccountManagementSettings>();
            services.AddSingleton(accountManagementSettings);

            return services;
        }

        public static IServiceCollection ConfigureTermsOfUseSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var termsOfUseSettings = configuration.GetSection("termsOfUse").Get<TermsOfUseSettings>();

            services.AddSingleton(termsOfUseSettings);

            return services;
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

            var gpPracticeCacheKeySettings = configuration.GetSection(GpPracticeCacheKeysSettings.SectionName).Get<GpPracticeCacheKeysSettings>();
            services.AddSingleton(gpPracticeCacheKeySettings);

            return services;
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddTransient<IUserClaimsPrincipalFactory<AspNetUser>, CatalogueUserClaimsPrincipalFactory>();

            services.AddIdentity<AspNetUser, AspNetRole>(o =>
                {
                    PasswordValidator.ConfigurePasswordOptions(o.Password);

                    o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    o.Lockout.MaxFailedAccessAttempts = 3;
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
            var notifyApiKey = configuration.GetValue<string>("NOTIFY_API_KEY");
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

        public static IServiceCollection ConfigureImportPracticeListMessageSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection("importPracticeListMessage").Get<ImportPracticeListMessageSettings>();
            services.AddSingleton(settings);

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

        public static IServiceCollection ConfigureProcurementHubMessageSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection("procurementHubMessage").Get<ProcurementHubMessageSettings>();
            services.AddSingleton(settings);

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

        public static void ConfigureContactUs(this IServiceCollection services, IConfiguration configuration)
        {
            var contactUsSettings = configuration.GetSection("contactUs").Get<ContactUsSettings>();
            services.AddSingleton(contactUsSettings);
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
                        options.ValidatorOptions.DefaultClassLevelCascadeMode = FluentValidation.CascadeMode.Continue;
                        options.ValidatorOptions.DefaultRuleLevelCascadeMode = FluentValidation.CascadeMode.Stop;
                    }).AddSingleton<IValidatorInterceptor, FluentValidatorInterceptor>();
        }

        public static void ConfigureFormOptions(this IServiceCollection services)
        {
            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = 16384;
            });
        }
    }
}
