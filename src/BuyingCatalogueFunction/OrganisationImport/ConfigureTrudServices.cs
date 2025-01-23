using System;
using System.Diagnostics.CodeAnalysis;
using BuyingCatalogueFunction.OrganisationImport.Interfaces;
using BuyingCatalogueFunction.OrganisationImport.Models;
using BuyingCatalogueFunction.OrganisationImport.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BuyingCatalogueFunction.OrganisationImport;

[ExcludeFromCodeCoverage(Justification = "Registers dependencies in IoC container.")]
public sealed class ConfigureTrudServices : IConfigureServices
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TrudApiOptions>(configuration.GetSection("trudApi"));
        services.Configure<TrudBatchOptions>(configuration.GetSection("batchOptions"));
        services.AddTransient<IHttpService, HttpService>();
        services.AddTransient<ITrudApiService, TrudApiService>();
        services.AddTransient<ITrudService, TrudService>();
        services.AddTransient<IZipService, ZipService>();

        services
            .AddHttpClient<TrudApiService>((provider, client) =>
            {
                var options = provider.GetRequiredService<IOptions<TrudApiOptions>>();

                client.BaseAddress = new Uri(options.Value.ApiUrl);
            });
    }
}
