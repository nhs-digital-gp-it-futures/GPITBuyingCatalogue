using System;
using System.Collections.Generic;
using System.IO.Compression;
using AutoMapper;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
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
        private const string BuyingCatalogueBlobConnectionEnvironmentVariable = "BC_BLOB_CONNECTION";
        private const string BuyingCatalogueBlobContainerEnvironmentVariable = "BC_BLOB_CONTAINER";
        private const string BuyingCatalogueSmtpHostEnvironmentVariable = "BC_SMTP_HOST";
        private const string BuyingCatalogueSmtpPortEnvironmentVariable = "BC_SMTP_PORT";

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

                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.ExpireTimeSpan = cookieExpiration.ExpireTimeSpan;
                options.SlidingExpiration = cookieExpiration.SlidingExpiration;
                options.AccessDeniedPath = "/404";
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
            });
        }

        public static void ConfigureDisabledErrorMessage(this IServiceCollection services, IConfiguration configuration)
        {
            var disabledErrorMessage = configuration.GetSection("disabledErrorMessage").Get<DisabledErrorMessageSettings>();
            services.AddSingleton(disabledErrorMessage);
        }

        public static void ConfigureEmail(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var host = Environment.GetEnvironmentVariable(BuyingCatalogueSmtpHostEnvironmentVariable);

            if (string.IsNullOrWhiteSpace(host))
                throw new InvalidOperationException($"Environment variable '{BuyingCatalogueSmtpHostEnvironmentVariable}' must be set for the smtp host");

            var port = Environment.GetEnvironmentVariable(BuyingCatalogueSmtpPortEnvironmentVariable);

            if (string.IsNullOrWhiteSpace(port))
                throw new InvalidOperationException($"Environment variable '{BuyingCatalogueSmtpPortEnvironmentVariable}' must be set for the smtp port");

            if (!int.TryParse(port, out var portNumber))
                throw new InvalidOperationException($"Environment variable '{BuyingCatalogueSmtpPortEnvironmentVariable}' must be a valid smtp port number");

            var allowInvalidCertificate = configuration.GetValue<bool>("AllowInvalidCertificate");
            var smtpSettings = configuration.GetSection("SmtpServer").Get<SmtpSettings>();
            smtpSettings.AllowInvalidCertificate ??= allowInvalidCertificate;
            smtpSettings.Host = host;
            smtpSettings.Port = portNumber;
            services.AddSingleton(smtpSettings);
            services.AddScoped<IMailTransport, SmtpClient>();
            services.AddTransient<IEmailService, MailKitEmailService>();
        }

        public static void ConfigureConsentCookieSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var cookieExpiration = configuration.GetSection("cookieExpiration").Get<CookieExpirationSettings>();
            cookieExpiration.ConsentExpiration = configuration.GetValue<TimeSpan>(Cookies.BuyingCatalogueConsentExpiration);

            services.AddSingleton(cookieExpiration);
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

        public static void ConfigureOrderMessageSettings(this IServiceCollection servies, IConfiguration configuration)
        {
            var orderMessageSettings = configuration.GetSection("orderMessage").Get<OrderMessageSettings>();
            servies.AddSingleton(orderMessageSettings);
        }

        public static void ConfigureRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            var registrationSettings = configuration.GetSection("Registration").Get<RegistrationSettings>();
            services.AddSingleton(registrationSettings);
        }

        public static void ConfigureAzureBlobStorage(this IServiceCollection services, IConfiguration configuration)
        {
            var buyingCatalogueConnectionString = Environment.GetEnvironmentVariable(BuyingCatalogueBlobConnectionEnvironmentVariable);

            if (string.IsNullOrWhiteSpace(buyingCatalogueConnectionString))
                throw new InvalidOperationException($"Environment variable '{BuyingCatalogueBlobConnectionEnvironmentVariable}' must be set for the blob connection string");

            var buyingCatalogueContainerString = Environment.GetEnvironmentVariable(BuyingCatalogueBlobContainerEnvironmentVariable);

            if (string.IsNullOrWhiteSpace(buyingCatalogueContainerString))
                throw new InvalidOperationException($"Environment variable '{BuyingCatalogueBlobContainerEnvironmentVariable}' must be set for the blob container");

            var settings = configuration.GetSection("AzureBlobStorage").Get<AzureBlobStorageSettings>();
            settings.ConnectionString = buyingCatalogueConnectionString;
            settings.ContainerName = buyingCatalogueContainerString;
            services.AddSingleton(settings);

            services.AddTransient(_ => AzureBlobContainerClientFactory.Create(settings));
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
    }
}
