using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;

public interface IGen2MappingService
{
    Task<bool> MapToSolutions(Gen2MappingModel gen2Mapping);
}
