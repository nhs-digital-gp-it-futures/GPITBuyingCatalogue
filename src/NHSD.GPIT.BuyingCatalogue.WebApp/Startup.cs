using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.Framework.Middleware;
using NHSD.GPIT.BuyingCatalogue.Services;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using Serilog;

namespace NHSD.GPIT.BuyingCatalogue.WebApp
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private const string OperatingModeEnvironmentVariable = "OPERATING_MODE";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(typeof(ActionArgumentNullFilter));
                options.Filters.Add(typeof(OrdersActionFilter));
                options.Filters.Add<SerilogMvcLoggingAttribute>();
            });

            services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            services.AddApplicationInsightsTelemetry();

            services.ConfigureDbContexts();

            services.ConfigureSession();

            services.ConfigureIdentity();

            services.ConfigureValidationSettings(Configuration);

            services.ConfigureConsentCookieSettings(Configuration);

            services.ConfigureCookies(Configuration);

            services.ConfigurePasswordReset(Configuration);

            services.ConfigureRegistration(Configuration);

            services.ConfigureAzureBlobStorage(Configuration);

            services.ConfigureOds(Configuration);

            services.ConfigureEmail(Configuration);

            services.ConfigureDisabledErrorMessage(Configuration);

            services.ConfigureAuthorization();

            ServicesStartup.Configure(services);

            services.ConfigureAutoMapper();

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper, ILogger<Startup> logger)
        {
            app.UseSerilogRequestLogging(opts =>
            {
                opts.GetLevel = SerilogRequestLoggingOptions.GetLevel;
                opts.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms. Action {ActionName}";
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(
                        context =>
                        {
                            var error = context.Features.Get<IExceptionHandlerFeature>();
                            if (error != null)
                            {
                                var errorMessage = error.Error.FullErrorMessage();

                                // TODO - AppInsights isn't picking up LogError for some reason
                                logger.LogInformation(error.Error, errorMessage);
                            }

                            context.Response.Redirect("/Home/Error");
                            return Task.CompletedTask;
                        });
                });

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            mapper.ConfigurationProvider.AssertConfigurationIsValid();

            app.UseSession();

            app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

            app.UseHttpsRedirection();

            var operatingMode = Environment.GetEnvironmentVariable(OperatingModeEnvironmentVariable);

            // Disable the marketing pages when deployed publicly
            if (string.IsNullOrWhiteSpace(operatingMode) || !operatingMode.Equals("Private", StringComparison.InvariantCultureIgnoreCase))
                app.UseMiddleware<DisableMarketingMiddleware>();

            app.UseMiddleware<Framework.Middleware.CookieConsent.CookieConsentMiddleware>();

            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = (context) =>
                {
                    context.Context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                },
            });

            app.UseCookiePolicy();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto,
            });

            app.Use(async (context, next) =>
            {
                context.Response.Headers["X-Frame-Options"] = "DENY";
                context.Response.Headers["X-Xss-Protection"] = "1; mode=block";
                context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                context.Response.Headers["Pragma"] = "no-cache";
                await next().ConfigureAwait(false);
            });

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
