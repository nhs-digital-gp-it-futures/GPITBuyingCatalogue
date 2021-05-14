using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Document
{
    public interface IDocumentService
    {
        Task<FileStreamResult> DownloadDocumentAsync(string name);

        Task<FileStreamResult> DownloadSolutionDocumentAsync(string id, string name);

        IAsyncEnumerable<string> GetDocumentsBySolutionId(string id);
    }
}
