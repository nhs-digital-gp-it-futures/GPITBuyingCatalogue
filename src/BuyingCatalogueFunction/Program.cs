using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuyingCatalogueFunction.Adapters;
using BuyingCatalogueFunction.Models.Ods;
using BuyingCatalogueFunction.Services;
using BuyingCatalogueFunction.Services.CapabilitiesUpdate;
using BuyingCatalogueFunction.Services.CapabilitiesUpdate.Interfaces;
using BuyingCatalogueFunction.Services.IncrementalUpdate;
using BuyingCatalogueFunction.Services.IncrementalUpdate.Interfaces;
using BuyingCatalogueFunction.Settings;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace BuyingCatalogueFunction;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureFunctionsWorkerDefaults(builder =>
            {
                builder
                    .AddApplicationInsights()
                    .AddApplicationInsightsLogger();
            })
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;

                services.AddSingleton(new OdsSettings(
                    configuration.GetValue<Uri>("OrganisationUri"),
                    configuration.GetValue<Uri>("RelationshipsUri"),
                    configuration.GetValue<Uri>("RolesUri"),
                    configuration.GetValue<Uri>("SearchUri")));

                services.AddSingleton<IIdentityService, FunctionsIdentityService>();

                services.AddTransient<IIncrementalUpdateService, IncrementalUpdateService>();
                services.AddTransient<IOdsService, OdsService>();
                services.AddTransient<IOdsOrganisationService, OdsOrganisationService>();
                services.AddTransient<IOrganisationUpdateService, OrganisationUpdateService>();
                services.AddTransient<ICapabilitiesUpdateService, CapabilitiesUpdateService>();
                services.AddTransient<ICapabilitiesService, CapabilitiesService>();

                services.AddTransient<IAdapter<Org, OdsOrganisation>, OdsOrganisationAdapter>();
                services.AddTransient<IAdapter<Org, IEnumerable<OrganisationRelationship>>, OrganisationRelationshipsAdapter>();
                services.AddTransient<IAdapter<Org, IEnumerable<OrganisationRole>>, OrganisationRolesAdapter>();

                services.AddDbContext<BuyingCatalogueDbContext>((serviceProvider, options) =>
                {
                    options.UseSqlServer(configuration.GetValue<string>("BUYINGCATALOGUECONNECTIONSTRING"));
                    options.EnableSensitiveDataLogging();
                });
            })
            .Build();

        await host.RunAsync();
    }
}
