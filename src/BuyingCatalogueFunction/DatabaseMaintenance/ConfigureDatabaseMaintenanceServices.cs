using BuyingCatalogueFunction.DatabaseMaintenance.Interfaces;
using BuyingCatalogueFunction.DatabaseMaintenance.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BuyingCatalogueFunction.DatabaseMaintenance;

public class ConfigureDatabaseMaintenanceServices : IConfigureServices
{
    public void ConfigureServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IDatabaseIndexMaintenanceService, DatabaseIndexMaintenanceService>();
    }
}
