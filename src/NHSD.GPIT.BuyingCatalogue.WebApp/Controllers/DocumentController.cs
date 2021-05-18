using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Document;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    public class DocumentController : Controller
    {
        private readonly ILogWrapper<DocumentController> logger;
        private readonly IDocumentService documentService;

        public DocumentController(ILogWrapper<DocumentController> logger, IDocumentService documentService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
