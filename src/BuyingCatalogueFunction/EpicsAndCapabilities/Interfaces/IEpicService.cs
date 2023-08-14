using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BuyingCatalogueFunction.EpicsAndCapabilities.Models;

namespace BuyingCatalogueFunction.EpicsAndCapabilities.Interfaces
{
    public interface IEpicService
    {
        Task<List<string>> Process(List<EpicCsv> epics);
        Task<List<EpicCsv>> Read(Stream stream);
    }
}
