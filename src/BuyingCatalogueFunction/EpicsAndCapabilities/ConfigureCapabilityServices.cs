using System.Diagnostics.CodeAnalysis;
using BuyingCatalogueFunction.EpicsAndCapabilities.Interfaces;
using BuyingCatalogueFunction.EpicsAndCapabilities.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuyingCatalogueFunction.EpicsAndCapabilities;

[ExcludeFromCodeCoverage(Justification = "Registers dependencies in IoC container.")]
public sealed class ConfigureCapabilityServices : IConfigureServices
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ICapabilityService, CapabilityService>();
        services.AddTransient<IEpicService, EpicService>();
        services.AddTransient<IStandardService, StandardService>();
        services.AddTransient<IStandardCapabilityService, StandardCapabilityService>();
    }
}
