using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers
{
    [Area("Solutions")]
    public sealed class DFOCVCController : Controller
    {
        private readonly ISolutionsService solutionsService;

        public DFOCVCController(ISolutionsService solutionsService)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [Route("Solutions/DFOCVC")]
        public async Task<IActionResult> Index()
        {
            var solutions = await solutionsService.GetDFOCVCSolutions();

            return View(new SolutionsModel { CatalogueItems = solutions });
        }
    }
}
