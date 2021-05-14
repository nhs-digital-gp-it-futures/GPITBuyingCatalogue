using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Document;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    public class DocumentController : Controller
    {
        private readonly ILogWrapper<DocumentController> _logger;
        private readonly IDocumentService _documentService;

        public DocumentController(ILogWrapper<DocumentController> logger, IDocumentService documentService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
        }

        [Route("Document/{documentName}")]
        public async Task<IActionResult> GetDocument(string documentName)
        {
            return await _documentService.DownloadDocumentAsync(documentName);            
        }
    }
}
