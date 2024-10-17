using System.IO;
using System.Threading.Tasks;

namespace BuyingCatalogueFunction.OrganisationImport.Interfaces;

public interface IZipService
{
    Task<Stream> GetTrudDataFileAsync(Stream zipStream);
}
