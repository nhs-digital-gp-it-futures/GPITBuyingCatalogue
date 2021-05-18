using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers
{
    [Area("Solutions")]
    public class DFOCVCController : Controller
    {
        private readonly ILogWrapper<DFOCVCController> logger;
        private readonly ISolutionsService solutionsService;

        public DFOCVCController(ILogWrapper<DFOCVCController> logger, ISolutionsService solutionsService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        public async Task<IActionResult> Index()
        {
            var solutions = await solutionsService.GetDFOCVCSolutions();

            return View(new SolutionsModel { CatalogueItems = solutions });
        }
    }
}
