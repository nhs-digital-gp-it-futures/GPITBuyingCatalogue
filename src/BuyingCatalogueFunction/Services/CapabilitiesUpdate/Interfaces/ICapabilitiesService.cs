using System.IO;
using System.Threading.Tasks;
using BuyingCatalogueFunction.Models.CapabilitiesUpdate;

namespace BuyingCatalogueFunction.Services.CapabilitiesUpdate.Interfaces;

public interface ICapabilitiesService
{
    Task<CapabilitiesImportModel> GetCapabilitiesAndEpics(Stream stream);
}
