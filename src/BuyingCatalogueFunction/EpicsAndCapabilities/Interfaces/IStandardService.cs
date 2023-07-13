using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BuyingCatalogueFunction.EpicsAndCapabilities.Models;

namespace BuyingCatalogueFunction.EpicsAndCapabilities.Interfaces
{
    public interface IStandardService
    {
        Task<List<string>> Process(List<StandardCsv> standards);
        Task<List<StandardCsv>> Read(Stream stream);
    }
}