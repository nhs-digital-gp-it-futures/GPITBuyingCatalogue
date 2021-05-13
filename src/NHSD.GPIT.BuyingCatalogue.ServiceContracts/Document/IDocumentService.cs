using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email
{
    public interface IDocumentService
    {
        // MJRTODO - From DocumentController
        Task<FileStreamResult> DownloadDocumentAsync(string name);

        // MJRTODO - Is there any difference??
        // From SolutionsController
        Task<FileStreamResult> DownloadSolutionDocumentAsync(string id, string name);

        IAsyncEnumerable<string> GetDocumentsBySolutionId(string id);
    }
}
