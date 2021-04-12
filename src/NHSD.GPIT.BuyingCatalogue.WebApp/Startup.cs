using System;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.Services.Email;
using NHSD.GPIT.BuyingCatalogue.Services.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions.DependencyInjection;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp
{
    public class Startup
    {
        private const string IdentityDbConnectionEnvironmentVariable = "ID_DB_CONNECTION";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            ConfigureDbContexts(services);

            ConfigureIdentity(services);

            ConfigureCookies(services);
            
            var issuerUrl = Configuration.GetValue<string>("issuerUrl");
            var issuerSettings = new IssuerSettings { IssuerUrl = new Uri(issuerUrl) };
            services.AddSingleton(issuerSettings);

            var passwordResetSettings = Configuration.GetSection("passwordReset").Get<PasswordResetSettings>();
            services.AddSingleton(passwordResetSettings);            
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IPasswordResetCallback, PasswordResetCallback>();

            var allowInvalidCertificate = Configuration.GetValue<bool>("AllowInvalidCertificate");
            var smtpSettings = Configuration.GetSection("SmtpServer").Get<SmtpSettings>();
            smtpSettings.AllowInvalidCertificate ??= allowInvalidCertificate;
            services.AddSingleton(smtpSettings);
            services.AddScoped<IMailTransport, SmtpClient>();
            services.AddTransient<IEmailService, MailKitEmailService>();
            
            var identityConnectionString = Environment.GetEnvironmentVariable(IdentityDbConnectionEnvironmentVariable);
            services.AddHealthChecks(identityConnectionString).AddSmtpHealthCheck(smtpSettings);

            var disabledErrorMessage = Configuration.GetSection("disabledErrorMessage").Get<DisabledErrorMessageSettings>();
            services.AddSingleton(disabledErrorMessage);

            services.AddAuthorization(options =>
            { 
                options.AddPolicy("AdminOnly", policy => policy.RequireClaim("IsAdmin"));                
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
                {
                    Predicate = healthCheckRegistration => healthCheckRegistration.Tags.Contains(HealthCheckTags.Live),
                });

                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
                {
                    Predicate = healthCheckRegistration => healthCheckRegistration.Tags.Contains(HealthCheckTags.Ready),
                });
            });
        }

        private void ConfigureDbContexts(IServiceCollection services)
        {
            var identityConnectionString = Environment.GetEnvironmentVariable(IdentityDbConnectionEnvironmentVariable);

            if (string.IsNullOrWhiteSpace(identityConnectionString))
            {
                throw new InvalidOperationException($"Environment variable '{IdentityDbConnectionEnvironmentVariable}' must be set for the database connection string");
            }
            
            services.AddDbContext<UsersDbContext>(options => options.UseSqlServer(identityConnectionString));
        }

        private static void ConfigureIdentity(IServiceCollection services)
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
            .AddEntityFrameworkStores<UsersDbContext>()                               
            .AddTokenProvider<DataProtectorTokenProvider<AspNetUser>>(TokenOptions.DefaultProvider)
            .AddPasswordValidator<PasswordValidator>();
        }

        private void ConfigureCookies(IServiceCollection services)
        {
            var cookieExpiration = Configuration.GetSection("cookieExpiration").Get<CookieExpirationSettings>();

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
    }
}
