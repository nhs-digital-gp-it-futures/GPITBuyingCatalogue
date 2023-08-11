using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BuyingCatalogueFunction.EpicsAndCapabilities.Models;

namespace BuyingCatalogueFunction.EpicsAndCapabilities.Interfaces
{
    public interface ICapabilityService
    {
        Task<List<string>> Process(List<CapabilityCsv> capabilties);
        Task<List<CapabilityCsv>> Read(Stream stream);
    }
}
