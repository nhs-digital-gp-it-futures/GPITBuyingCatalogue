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
        private readonly ILogWrapper<DFOCVCController> _logger;
        private readonly ISolutionsService _solutionsService;

        public DFOCVCController(ILogWrapper<DFOCVCController> logger, ISolutionsService solutionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(_solutionsService));
        }

        public async Task<IActionResult> Index()
        {
            var solutions = await _solutionsService.GetDFOCVCSolutions();

            var model = new SolutionsModel { CatalogueItems = solutions };

            return View(model);            
        }        
    }
}
