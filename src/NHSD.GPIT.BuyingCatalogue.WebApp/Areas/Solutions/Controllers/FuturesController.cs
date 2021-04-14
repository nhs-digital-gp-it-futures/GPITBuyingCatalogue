using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers
{
    [Area("Solutions")]
    public class FuturesController : Controller
    {
        private readonly ILogger<FuturesController> _logger;
        private readonly ISolutionsService _solutionsService;

        public FuturesController(ILogger<FuturesController> logger, ISolutionsService solutionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(_solutionsService));
        }

        public IActionResult Index()
        {                                    
            return View();
        }

        public async Task<IActionResult> Foundation()
        {
            var foundationSolutions = await _solutionsService.GetFoundationSolutions();

            var model = new SolutionsModel { Solutions = foundationSolutions };

            return View(model);            
        }

        public IActionResult CapabilitiesSelector()
        {
            return View();
        }

        public IActionResult Compare()
        {
            return View();
        }
    }
}
