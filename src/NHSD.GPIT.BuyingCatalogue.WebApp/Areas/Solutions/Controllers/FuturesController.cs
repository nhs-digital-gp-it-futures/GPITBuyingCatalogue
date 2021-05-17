using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Document;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers
{
    [Area("Solutions")]
    public class FuturesController : Controller
    {
        private readonly ILogWrapper<FuturesController> _logger;
        private readonly ISolutionsService _solutionsService;
        private readonly IDocumentService _documentService;

        public FuturesController(ILogWrapper<FuturesController> logger, ISolutionsService solutionsService, IDocumentService documentService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("Solutions/Futures/Foundation")]
        public async Task<IActionResult> Foundation()
        {
            var solutions = await _solutionsService.GetFuturesFoundationSolutions();
            
            return View(new SolutionsModel { CatalogueItems = solutions });            
        }

        [Route("Solutions/Futures/CapabilitiesSelector")]
        public async Task<IActionResult> CapabilitiesSelector()
        {
            var model = new CapabilitiesModel();

            var capabilities = await _solutionsService.GetFuturesCapabilities();

            int half = capabilities.Count / 2;
            
            model.LeftCapabilities = capabilities.Take(half)
                .Select(o => new CapabilityModel { CapabilityName = o.Name, CapabilityRef = o.CapabilityRef, Checked = false })
                .ToArray();

            model.RightCapabilities = capabilities.Skip(half)
                .Select(o => new CapabilityModel { CapabilityName = o.Name, CapabilityRef = o.CapabilityRef, Checked = false })
                .ToArray();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Solutions/Futures/CapabilitiesSelector")]
        public IActionResult CapabilitiesSelector(CapabilitiesModel model)
        {
            var selected = model.LeftCapabilities.Where(x => x.Checked).Union(model.RightCapabilities.Where(x => x.Checked));

            var queryString = string.Join("-", selected.Select(x => x.CapabilityRef));

            return RedirectToAction("SearchResults", new { capabilities = queryString });
        }

        [Route("Solutions/Futures/Compare")]
        public async Task<IActionResult> Compare()
        {
            return await _documentService.DownloadDocumentAsync("compare-futures-solutions.xlsx");            
        }

        [Route("Solutions/Futures/Compare/Document")]
        public IActionResult Document()
        {
            return View();
        }

        [Route("Solutions/Futures/SearchResults")]
        public async Task <IActionResult> SearchResults(string capabilities)
        {
            string[] splitCapabilities = new string[0];

            if(!string.IsNullOrWhiteSpace(capabilities))
                splitCapabilities = capabilities.Split('-', StringSplitOptions.RemoveEmptyEntries);

            var foundationSolutions = await _solutionsService.GetFuturesSolutionsByCapabilities(splitCapabilities);
            
            return View(new SolutionsModel { CatalogueItems = foundationSolutions });
        }
    }
}
