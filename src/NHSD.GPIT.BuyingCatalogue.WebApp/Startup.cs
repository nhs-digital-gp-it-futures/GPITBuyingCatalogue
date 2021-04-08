using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;

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

            ConfigureCookie(services);
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
                o.Password.RequiredLength = 12;
                o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                o.Lockout.MaxFailedAccessAttempts = 6;
            })
               .AddEntityFrameworkStores<UsersDbContext>()
               .AddDefaultTokenProviders();
        }

        private void ConfigureCookie(IServiceCollection services)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "user-session";
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always; // MJRTODO - Confirm
                options.ExpireTimeSpan = TimeSpan.FromSeconds(1000); // MJRTODO - Config
                options.AccessDeniedPath = "/404";
            });

            services.AddAntiforgery(options => options.Cookie.Name = "antiforgery");
        }
    }
}
