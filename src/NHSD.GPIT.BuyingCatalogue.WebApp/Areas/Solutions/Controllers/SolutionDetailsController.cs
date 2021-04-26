using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers
{
    [Area("Solutions")]
    public class SolutionDetailsController : Controller
    {
        private readonly ILogWrapper<SolutionDetailsController> _logger;
        private readonly ISolutionsService _solutionsService;

        public SolutionDetailsController(ILogWrapper<SolutionDetailsController> logger, ISolutionsService solutionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }
    
        [Route("solutions/futures/{id}")]
        public async Task <IActionResult> SolutionDetail(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new SolutionDetailModel(solution);

            return View(model);
        }

        [Route("solutions/futures/foundation/{id}")]
        public async Task<IActionResult> FoundationSolutionDetail(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new SolutionDetailModel(solution);

            return View("SolutionDetail", model);
        }

        [Route("solutions/dfocvc/{id}")]
        public async Task<IActionResult> DVOCVCSolutionDetail(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new SolutionDetailModel(solution);

            return View("SolutionDetail", model);
        }

        [Route("solutions/vaccinations/{id}")]
        public async Task<IActionResult> VaccinationsSolutionDetail(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new SolutionDetailModel(solution);

            return View("SolutionDetail", model);
        }

        [Route("solutions/preview/{id}")]
        public async Task<IActionResult> PreviewSolutionDetail(string id)
        {
            var solution = await _solutionsService.GetSolution(id);

            var model = new SolutionDetailModel(solution);
            model.BackLink = $"/marketing/supplier/solution/{id}";
            
            return View("SolutionDetail", model);
        }
    }
}
