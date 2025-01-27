using System.Threading.Tasks;

namespace BuyingCatalogueFunction.DatabaseMaintenance.Interfaces;

public interface IDatabaseIndexMaintenanceService
{
    Task RebuildIndexes();
}
