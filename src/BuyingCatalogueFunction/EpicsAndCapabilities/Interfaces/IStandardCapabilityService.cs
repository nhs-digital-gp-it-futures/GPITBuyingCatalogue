using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BuyingCatalogueFunction.EpicsAndCapabilities.Models;

namespace BuyingCatalogueFunction.EpicsAndCapabilities.Interfaces
{
    public interface IStandardCapabilityService
    {
        Task<List<string>> Process(List<StandardCapabilityCsv> standardCapabilities);
        Task<List<StandardCapabilityCsv>> Read(Stream stream);
    }
}