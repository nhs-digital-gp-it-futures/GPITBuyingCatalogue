using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers
{
    [Area("Solutions")]
    public class SolutionDetailsController : Controller
    {
        private readonly ILogger<SolutionDetailsController> _logger;
        private readonly ISolutionsService _solutionsService;

        public SolutionDetailsController(ILogger<SolutionDetailsController> logger, ISolutionsService solutionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(_solutionsService));
        }
    
        [Route("Solutions/Futures/{id}")]
        public async Task <IActionResult> SolutionDetail(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new SolutionDetailModel(solution);

            return View(model);
        }

        [Route("Solutions/Futures/Foundation/{id}")]
        public async Task<IActionResult> FoundationSolutionDetail(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new SolutionDetailModel(solution);

            return View("SolutionDetail", model);
        }

        [Route("Solutions/DFOCVC/{id}")]
        public async Task<IActionResult> DVOCVCSolutionDetail(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new SolutionDetailModel(solution);

            return View("SolutionDetail", model);
        }
    }
}
