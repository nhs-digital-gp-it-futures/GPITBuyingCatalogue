using System.Diagnostics.CodeAnalysis;
using BuyingCatalogueFunction.DatabaseMaintenance.Interfaces;
using BuyingCatalogueFunction.DatabaseMaintenance.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuyingCatalogueFunction.DatabaseMaintenance;

[ExcludeFromCodeCoverage(Justification = "Registers dependencies in IoC container.")]
public class ConfigureDatabaseMaintenanceServices : IConfigureServices
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDatabaseIndexMaintenanceService, DatabaseIndexMaintenanceService>();
    }
}
