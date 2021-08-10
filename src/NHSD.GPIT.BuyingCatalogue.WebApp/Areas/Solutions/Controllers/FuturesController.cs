using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Document;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers
{
    [Area("Solutions")]
    public sealed class FuturesController : Controller
    {
        private readonly ISolutionsService solutionsService;
        private readonly IDocumentService documentService;

        public FuturesController(ISolutionsService solutionsService, IDocumentService documentService)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
        }

        [HttpGet("Solutions/Futures")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Solutions/Futures/Foundation")]
        public async Task<IActionResult> Foundation()
        {
            var solutions = await solutionsService.GetFuturesFoundationSolutions();

            return View(new SolutionsModel { CatalogueItems = solutions });
        }

        [HttpGet("Solutions/Futures/CapabilitiesSelector")]
        public async Task<IActionResult> CapabilitiesSelector()
        {
            var model = new CapabilitiesModel();

            var capabilities = await solutionsService.GetFuturesCapabilities();

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
        [Route("Solutions/Futures/CapabilitiesSelector")]
        public IActionResult CapabilitiesSelector(CapabilitiesModel model)
        {
            var selected = model.LeftCapabilities.Where(c => c.Checked).Union(model.RightCapabilities.Where(c => c.Checked));

            var queryString = string.Join("-", selected.Select(c => c.CapabilityRef));

            return RedirectToAction("SearchResults", new { capabilities = queryString });
        }

        [HttpGet("Solutions/Futures/Compare")]
        public IActionResult Compare()
        {
            return View();
        }

        [HttpGet("Solutions/Compare/Document")]
        public async Task<IActionResult> Document()
        {
            return await documentService.DownloadDocumentAsync("compare-solutions.xlsx");
        }

        [HttpGet("Solutions/Futures/SearchResults")]
        public async Task<IActionResult> SearchResults(string capabilities)
        {
            string[] splitCapabilities = Array.Empty<string>();

            if (!string.IsNullOrWhiteSpace(capabilities))
                splitCapabilities = capabilities.Split('-', StringSplitOptions.RemoveEmptyEntries);

            var foundationSolutions = await solutionsService.GetFuturesSolutionsByCapabilities(splitCapabilities);

            return View(new SolutionsModel { CatalogueItems = foundationSolutions });
        }
    }
}
