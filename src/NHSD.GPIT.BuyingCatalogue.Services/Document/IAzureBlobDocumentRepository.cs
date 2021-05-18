using System.Collections.Generic;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.Services.Document
{
    public interface IAzureBlobDocumentRepository
    {
        Task<IDocument> DownloadAsync(string documentName);

        Task<IDocument> DownloadAsync(string directoryName, string documentName);

        IAsyncEnumerable<string> GetFileNamesAsync(string directory);
    }
}
