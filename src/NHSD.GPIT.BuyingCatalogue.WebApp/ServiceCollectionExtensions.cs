using System;
using System.Collections.Generic;
using AutoMapper;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.DependencyInjection;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.Services.Document;
using NHSD.GPIT.BuyingCatalogue.Services.Email;
using NHSD.GPIT.BuyingCatalogue.Services.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.MappingProfiles;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.Solution;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.MappingProfiles;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp
{
    public static class ServiceCollectionExtensions
    {
        private const string BuyingCatalogueDbConnectionEnvironmentVariable = "BC_DB_CONNECTION";
        private const string CatalogueOrderingDbConnectionEnvironmentVariable = "CO_DB_CONNECTION";

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

        public static void ConfigureAutoMapper(this IServiceCollection services)
        {
            services.AddTransient<IMemberValueResolver<object, object, string, string>,
                ConfigSettingResolver>();
            services
                .AddTransient<IMemberValueResolver<object, object, string, bool?>,
                    StringToNullableBoolResolver>();
            services
                .AddTransient<ITypeConverter<CatalogueItem, SolutionStatusModel>,
                    CatalogueItemToSolutionStatusModelConverter>();
            services.AddTransient<ITypeConverter<string, bool?>, StringToNullableBoolResolver>();
            services
                .AddTransient<IMemberValueResolver<CatalogueItem, InteroperabilityModel, string, IList<IntegrationModel>>,
                    IntegrationModelsResolver>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        public static void ConfigureCookies(this IServiceCollection services, IConfiguration configuration)
        {
            var cookieExpiration = configuration.GetSection("cookieExpiration").Get<CookieExpirationSettings>();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "user-session";
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
                options.ExpireTimeSpan = cookieExpiration.ExpireTimeSpan;
                options.SlidingExpiration = cookieExpiration.SlidingExpiration;
                options.AccessDeniedPath = "/404"; // MJRTODO - don't like this
            });

            services.AddAntiforgery(options => options.Cookie.Name = "antiforgery");
        }

        public static void ConfigureOds(this IServiceCollection services, IConfiguration configuration)
        {
            var odsSettings = configuration.GetSection("Ods").Get<OdsSettings>();

            services.AddSingleton(odsSettings);
        }

        public static void ConfigureDbContexts(this IServiceCollection services, IHealthChecksBuilder healthCheckBuilder)
        {
            var buyingCatalogueConnectionString = Environment.GetEnvironmentVariable(BuyingCatalogueDbConnectionEnvironmentVariable);

            if (string.IsNullOrWhiteSpace(buyingCatalogueConnectionString))
                throw new InvalidOperationException($"Environment variable '{BuyingCatalogueDbConnectionEnvironmentVariable}' must be set for the database connection string");

            var catalogueOrderingConnectionString = Environment.GetEnvironmentVariable(CatalogueOrderingDbConnectionEnvironmentVariable);

            if (string.IsNullOrWhiteSpace(catalogueOrderingConnectionString))
                throw new InvalidOperationException($"Environment variable '{CatalogueOrderingDbConnectionEnvironmentVariable}' must be set for the database connection string");

            services.AddDbContext<GPITBuyingCatalogueDbContext>(options => options.UseSqlServer(buyingCatalogueConnectionString));
            services.AddDbContext<OrderingDbContext>(options => options.UseSqlServer(catalogueOrderingConnectionString));

            healthCheckBuilder.AddDatabaseHealthCheck(buyingCatalogueConnectionString);
        }

        public static void ConfigureSession(this IServiceCollection services)
        {
            var buyingCatalogueConnectionString = Environment.GetEnvironmentVariable(BuyingCatalogueDbConnectionEnvironmentVariable);

            if (string.IsNullOrWhiteSpace(buyingCatalogueConnectionString))
                throw new InvalidOperationException($"Environment variable '{BuyingCatalogueDbConnectionEnvironmentVariable}' must be set for the database connection string");

            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = buyingCatalogueConnectionString;
                options.SchemaName = "dbo";
                options.TableName = "SQLSessions";
            });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
            });
        }

        public static void ConfigureDisabledErrorMessage(this IServiceCollection services, IConfiguration configuration)
        {
            var disabledErrorMessage = configuration.GetSection("disabledErrorMessage").Get<DisabledErrorMessageSettings>();
            services.AddSingleton(disabledErrorMessage);
        }

        public static void ConfigureEmail(
            this IServiceCollection services,
            IConfiguration configuration,
            IHealthChecksBuilder healthCheckBuilder)
        {
            var allowInvalidCertificate = configuration.GetValue<bool>("AllowInvalidCertificate");
            var smtpSettings = configuration.GetSection("SmtpServer").Get<SmtpSettings>();
            smtpSettings.AllowInvalidCertificate ??= allowInvalidCertificate;
            services.AddSingleton(smtpSettings);
            services.AddScoped<IMailTransport, SmtpClient>();
            services.AddTransient<IEmailService, MailKitEmailService>();
            healthCheckBuilder.AddSmtpHealthCheck(smtpSettings);
        }

        public static void ConfigureCookiePolicy(this IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.ConsentCookie.Name = "buyingcatalogue-cookie-consent";
            });
        }

        public static void ConfigureValidationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var validationSettings = new ValidationSettings
            {
                MaxDeliveryDateWeekOffset = configuration.GetValue<int>("MaxDeliveryDateWeekOffset"),
            };
            services.AddSingleton(validationSettings);
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddTransient<IUserClaimsPrincipalFactory<AspNetUser>, UserClaimsPrincipalFactoryEx<AspNetUser>>();

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
                .AddEntityFrameworkStores<GPITBuyingCatalogueDbContext>()
                .AddTokenProvider<DataProtectorTokenProvider<AspNetUser>>(TokenOptions.DefaultProvider)
                .AddPasswordValidator<PasswordValidator>();
        }

        public static void ConfigureIssuer(this IServiceCollection services, IConfiguration configuration)
        {
            var issuerUrl = configuration.GetValue<string>("issuerUrl");
            var issuerSettings = new IssuerSettings { IssuerUrl = new Uri(issuerUrl) };
            services.AddSingleton(issuerSettings);
        }

        public static void ConfigurePasswordReset(this IServiceCollection services, IConfiguration configuration)
        {
            var passwordResetSettings = configuration.GetSection("passwordReset").Get<PasswordResetSettings>();
            services.AddSingleton(passwordResetSettings);
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IPasswordResetCallback, PasswordResetCallback>();
        }

        public static void ConfigureRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            var registrationSettings = configuration.GetSection("Registration").Get<RegistrationSettings>();
            services.AddSingleton(registrationSettings);
        }

        public static void ConfigureAzureBlobStorage(this IServiceCollection services, IConfiguration configuration, IHealthChecksBuilder healthCheckBuilder)
        {
            var settings = configuration.GetSection("AzureBlobStorage").Get<AzureBlobStorageSettings>();
            services.AddSingleton(settings);

            services.AddTransient(_ => AzureBlobContainerClientFactory.Create(settings));

            healthCheckBuilder.AddAzureStorageHealthChecks(settings);
        }
    }
}
