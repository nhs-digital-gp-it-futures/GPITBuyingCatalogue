using System;
using System.Collections.Generic;
using BuyingCatalogueFunction;
using BuyingCatalogueFunction.Adapters;
using BuyingCatalogueFunction.Models.Ods;
using BuyingCatalogueFunction.Services;
using BuyingCatalogueFunction.Services.IncrementalUpdate;
using BuyingCatalogueFunction.Services.IncrementalUpdate.Interfaces;
using BuyingCatalogueFunction.Settings;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

[assembly: FunctionsStartup(typeof(Startup))]
namespace BuyingCatalogueFunction
{
    public class Startup : FunctionsStartup
    {
        private readonly ILogger<Startup> _logger;

        public Startup()
        {
            using var loggerFactory = new LoggerFactory();

            _logger = loggerFactory.CreateLogger<Startup>();
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            builder.ConfigurationBuilder.AddEnvironmentVariables();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            services.AddSingleton(GetOdsSettings());
            services.AddSingleton<IIdentityService, FunctionsIdentityService>();

            services.AddTransient<IIncrementalUpdateService, IncrementalUpdateService>();
            services.AddTransient<IOdsService, OdsService>();
            services.AddTransient<IOdsOrganisationService, OdsOrganisationService>();
            services.AddTransient<IOrganisationUpdateService, OrganisationUpdateService>();

            services.AddTransient<IAdapter<Org, OdsOrganisation>, OdsOrganisationAdapter>();
            services.AddTransient<IAdapter<Org, IEnumerable<OrganisationRelationship>>, OrganisationRelationshipsAdapter>();
            services.AddTransient<IAdapter<Org, IEnumerable<OrganisationRole>>, OrganisationRolesAdapter>();

            services.AddDbContext<BuyingCatalogueDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetOrThrow("BUYINGCATALOGUECONNECTIONSTRING", _logger));
                options.EnableSensitiveDataLogging();
            });
        }

        private OdsSettings GetOdsSettings()
        {
            var organisationUri = Configuration.GetOrThrow("OrganisationUri", _logger);
            var relationshipsUri = Configuration.GetOrThrow("RelationshipsUri", _logger);
            var rolesUri = Configuration.GetOrThrow("RolesUri", _logger);
            var searchUri = Configuration.GetOrThrow("SearchUri", _logger);

            return new OdsSettings(
                new Uri(organisationUri),
                new Uri(relationshipsUri),
                new Uri(rolesUri),
                new Uri(searchUri));
        }
    }
}
