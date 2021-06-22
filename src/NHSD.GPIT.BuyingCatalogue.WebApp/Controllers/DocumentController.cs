using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Document;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IDocumentService documentService;

        public DocumentController(IDocumentService documentService)
        {
            this.documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
        }

        [Route("Document/{documentName}")]
        public async Task<IActionResult> GetDocument(string documentName)
        {
            var document = await documentService.DownloadDocumentAsync(documentName);

            if (document == null)
                return View("NotFound");

            return document;
        }
    }
}
