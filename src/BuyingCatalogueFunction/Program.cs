using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuyingCatalogueFunction.EpicsAndCapabilities.Interfaces;
using BuyingCatalogueFunction.EpicsAndCapabilities.Services;
using BuyingCatalogueFunction.IncrementalUpdate.Adapters;
using BuyingCatalogueFunction.IncrementalUpdate.Interfaces;
using BuyingCatalogueFunction.IncrementalUpdate.Models.Ods;
using BuyingCatalogueFunction.IncrementalUpdate.Services;
using BuyingCatalogueFunction.Services;
using BuyingCatalogueFunction.Settings;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
                services.AddTransient<ICapabilityService, CapabilityService>();
                services.AddTransient<IEpicService, EpicService>();
                services.AddTransient<IStandardService, StandardService>();
                services.AddTransient<IStandardCapabilityService, StandardCapabilityService>();

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
