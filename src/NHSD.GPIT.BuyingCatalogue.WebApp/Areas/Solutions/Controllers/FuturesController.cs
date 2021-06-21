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
        private readonly ILogWrapper<FuturesController> logger;
        private readonly ISolutionsService solutionsService;
        private readonly IDocumentService documentService;

        public FuturesController(ILogWrapper<FuturesController> logger, ISolutionsService solutionsService, IDocumentService documentService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
        }

        [Route("Solutions/Futures")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("Solutions/Futures/Foundation")]
        public async Task<IActionResult> Foundation()
        {
            var solutions = await solutionsService.GetFuturesFoundationSolutions();

            return View(new SolutionsModel { CatalogueItems = solutions });
        }

        [Route("Solutions/Futures/CapabilitiesSelector")]
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
            var selected = model.LeftCapabilities.Where(x => x.Checked).Union(model.RightCapabilities.Where(x => x.Checked));

            var queryString = string.Join("-", selected.Select(x => x.CapabilityRef));

            return RedirectToAction("SearchResults", new { capabilities = queryString });
        }

        [Route("Solutions/Futures/Compare")]
        public IActionResult Compare()
        {
            return View();
        }

        [Route("Solutions/Compare/Document")]
        public async Task<IActionResult> Document()
        {
            return await documentService.DownloadDocumentAsync("compare-solutions.xlsx");
        }

        [Route("Solutions/Futures/SearchResults")]
        public async Task<IActionResult> SearchResults(string capabilities)
        {
            string[] splitCapabilities = new string[0];

            if (!string.IsNullOrWhiteSpace(capabilities))
                splitCapabilities = capabilities.Split('-', StringSplitOptions.RemoveEmptyEntries);

            var foundationSolutions = await solutionsService.GetFuturesSolutionsByCapabilities(splitCapabilities);

            return View(new SolutionsModel { CatalogueItems = foundationSolutions });
        }
    }
}
