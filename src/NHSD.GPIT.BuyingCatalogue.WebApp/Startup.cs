using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.Framework.Environments;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.Services;
using NHSD.GPIT.BuyingCatalogue.Services.Pdf;
using NHSD.GPIT.BuyingCatalogue.Services.Routing;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.ModelBinders;
using Serilog;

namespace NHSD.GPIT.BuyingCatalogue.WebApp
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private readonly IHostEnvironment hostEnvironment;

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            this.hostEnvironment = hostEnvironment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            CurrentEnvironment.IsDevelopment = hostEnvironment.IsDevelopment() || IsE2ETestEnvironment();

            if (!IsE2ETestEnvironment())
            {
                services.ConfigureDataProtection(Configuration);
            }

            services.ConfigureResponseCompression();

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(typeof(ActionArgumentNullFilter));
                options.Filters.Add(typeof(OrdersActionFilter));
                options.Filters.Add(typeof(CookieConsentActionFilter));
                options.Filters.Add(typeof(TermsOfUseActionFilter));
                options.Filters.Add(typeof(UpdatePasswordActionFilter));
                options.Filters.Add<SerilogMvcLoggingAttribute>();
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                options.Filters.Add(new BadRequestActionFilter());

                options.ModelBinderProviders.Insert(0, new NewlinesNormalizingModelBinderProvider());
            }).AddControllersAsServices();

            services.AddFluentValidation();

            services.AddApplicationInsightsTelemetry();

            services.ConfigureDbContexts(Configuration);

            if (!IsE2ETestEnvironment())
            {
                services.ConfigureSession(Configuration);
            }

            services.ConfigureIdentity(Configuration);

            services.ConfigureCacheKeySettings(Configuration)
                .ConfigureGovNotify(Configuration)
                .ConfigureImportPracticeListMessageSettings(Configuration)
                .ConfigureNominateOrganisationMessageSettings(Configuration)
                .ConfigureOrderMessageSettings(Configuration)
                .ConfigureProcurementHubMessageSettings(Configuration)
                .ConfigureRequestAccountMessageSettings(Configuration)
                .ConfigureConsentCookieSettings(Configuration)
                .ConfigureTermsOfUseSettings(Configuration)
                .ConfigureAnalyticsSettings(Configuration)
                .ConfigurePriceTiersCap(Configuration)
                .ConfigureAccountManagement(Configuration)
                .ConfigureRecaptcha(Configuration);

            services.ConfigureCookies(Configuration);

            services.ConfigurePassword(Configuration);

            services.ConfigureRegistration(Configuration);

            services.ConfigureContactUs(Configuration);

            services.ConfigureOds(Configuration);

            services.ConfigureDomainName(Configuration);

            services.ConfigurePdf(Configuration);

            services.ConfigureFormOptions();

            services.ConfigureDisabledErrorMessage(Configuration);

            services.ConfigureAuthorization();

            services.ConfigureBlobStorage(Configuration);

            services.ConfigureQueueStorage(Configuration);

            services.AddSingleton<IRoutingService, RoutingService>();
            services.AddSingleton<IOrderTaskProgressProviderService, OrderTaskProgressProviderService>();
            services.AddSingleton<OrganisationRestrictActionFilterAttribute>();
            services.AddScoped<IOrderPdfService, OrderPdfService>();

            if (!IsE2ETestEnvironment())
            {
                services.Decorate<IOrderPdfService, CachedOrderPdfService>();
            }

            ServicesStartup.Configure(services);

            services.AddRazorPages();

            services.AddHttpClient();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseResponseCompression();

            var forwardingOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
            };

            forwardingOptions.KnownNetworks.Clear();
            forwardingOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(forwardingOptions);

            app.UseSerilogRequestLogging(opts =>
            {
                opts.GetLevel = SerilogRequestLoggingOptions.GetLevel;
                opts.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms. Action {ActionName}";
            });

            if (env.IsDevelopment() || env.IsEnvironment("E2ETest"))
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
                            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

                            if (exceptionHandlerFeature != null)
                            {
                                logger.LogError("Exception occured {Error}:", exceptionHandlerFeature.Error);
                            }

                            context.Response.Redirect("/Home/Error");
                            return Task.CompletedTask;
                        });
                });

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSession();

            app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

            app.UseHttpsRedirection();

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

        private bool IsE2ETestEnvironment() => hostEnvironment.IsEnvironment("E2ETest");
    }
}
