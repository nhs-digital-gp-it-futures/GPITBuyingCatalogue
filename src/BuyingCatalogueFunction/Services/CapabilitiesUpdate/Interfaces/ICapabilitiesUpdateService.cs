using System.Threading.Tasks;
using BuyingCatalogueFunction.Models.CapabilitiesUpdate;

namespace BuyingCatalogueFunction.Services.CapabilitiesUpdate.Interfaces;

public interface ICapabilitiesUpdateService
{
    Task UpdateAsync(CapabilitiesImportModel capabilitiesAndEpics);
}
