using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Security.Claims;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
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
using NHSD.GPIT.BuyingCatalogue.Framework.Environments;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Security;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Security.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Storage;
using NHSD.GPIT.BuyingCatalogue.Services.Email;
using NHSD.GPIT.BuyingCatalogue.Services.Identity;
using NHSD.GPIT.BuyingCatalogue.Services.Organisations;
using NHSD.GPIT.BuyingCatalogue.Services.Security;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using NHSD.GPIT.BuyingCatalogue.WebApp.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;
using Notify.Client;
using Notify.Interfaces;

namespace NHSD.GPIT.BuyingCatalogue.WebApp
{
    [ExcludeFromCodeCoverage]
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
                        ClaimTypes.Role,
                        new[] { OrganisationFunction.Authority.Name }));

                options.AddPolicy(
                    "Buyer",
                    policy => policy.RequireClaim(
                        ClaimTypes.Role,
                        new[] { OrganisationFunction.Buyer.Name, OrganisationFunction.AccountManager.Name }));

                options.AddPolicy(
                    "AccountManager",
                    policy => policy.RequireClaim(
                        ClaimTypes.Role,
                        new[] { OrganisationFunction.AccountManager.Name }));

                options.AddPolicy(
                    "Development",
                    policy => policy.Requirements.Add(new DevelopmentRequirement()));
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
                    OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync,
                };
            });

            services.AddAntiforgery(options =>
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.Name = "antiforgery";
            });

            services.Configure<SecurityStampValidatorOptions>(o => o.ValidationInterval = TimeSpan.FromMinutes(0));
        }

        public static void ConfigureOds(this IServiceCollection services, IConfiguration configuration)
        {
            var odsSettings = configuration.GetSection("Ods").Get<OdsSettings>();

            services.AddSingleton(odsSettings);

            services.AddScoped<IOdsService, TrudOdsService>();
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

        public static void ConfigureIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            var passwordSettings = configuration.GetSection("password").Get<PasswordSettings>();

            services.AddTransient<IUserClaimsPrincipalFactory<AspNetUser>, CatalogueUserClaimsPrincipalFactory>();

            services.AddIdentity<AspNetUser, AspNetRole>(o =>
                {
                    PasswordValidator.ConfigurePasswordOptions(o.Password);

                    o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(passwordSettings.LockOutTimeInMinutes);
                    o.Lockout.MaxFailedAccessAttempts = passwordSettings.MaxAccessFailedAttempts;
                })
                .AddEntityFrameworkStores<BuyingCatalogueDbContext>()
                .AddTokenProvider<DataProtectorTokenProvider<AspNetUser>>(TokenOptions.DefaultProvider)
                .AddPasswordValidator<PasswordValidator>();
        }

        public static void ConfigurePassword(this IServiceCollection services, IConfiguration configuration)
        {
            var passwordSettings = configuration.GetSection("password").Get<PasswordSettings>();
            services.AddSingleton(passwordSettings);
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

        public static void ConfigureBlobStorage(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection("AzureBlobSettings").Get<AzureBlobSettings>();

            services.AddSingleton(settings);
            services.AddScoped<BlobServiceClient>(_ => new(settings.ConnectionString));
        }

        public static void ConfigureQueueStorage(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection("AzureBlobSettings").Get<AzureBlobSettings>();

            services.AddSingleton(settings);
            services.AddScoped<QueueServiceClient>(_ => new(settings.ConnectionString));
        }

        public static IServiceCollection ConfigureRecaptcha(this IServiceCollection services, IConfiguration configuration)
        {
            const string validationErrorMessage = "reCAPTCHA configuration: Site or Secret key not specified";

            services.AddOptions<RecaptchaSettings>()
                .Bind(configuration.GetSection(nameof(RecaptchaSettings)))
                .Validate(x => !string.IsNullOrWhiteSpace(x.SiteKey) && !string.IsNullOrWhiteSpace(x.SecretKey), validationErrorMessage)
                .ValidateOnStart();

            services.AddHttpClient<IRecaptchaVerificationService, GoogleRecaptchaVerificationService>(
                x =>
                {
                    x.BaseAddress = RecaptchaSettings.GoogleRecaptchaApiUri;
                });

            return services;
        }

        public static IServiceCollection ConfigureHsts(this IServiceCollection services, IConfiguration configuration)
        {
            if (CurrentEnvironment.IsDevelopment) return services;

            const int defaultDays = 365;

            var maxAgeDays = configuration.GetValue<int?>("hstsMaxAgeDays") ?? defaultDays;

            services.AddHsts(
                x =>
                {
                    x.Preload = true;
                    x.IncludeSubDomains = true;
                    x.MaxAge = TimeSpan.FromDays(maxAgeDays);
                });

            return services;
        }
    }
}
