using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.Solution;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    public class SolutionController : Controller
    {
        private readonly ILogWrapper<SolutionController> _logger;
        private readonly ISolutionsService _solutionsService;

        public SolutionController(ILogWrapper<SolutionController> logger, ISolutionsService solutionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(_solutionsService));
        }

        [Route("marketing/supplier/solution/{id}")]
        public async Task<IActionResult> Index(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);

            var model = new SolutionStatusModel(solution);

            return View(model);
        }

        [HttpGet("marketing/supplier/solution/{id}/preview")]
        public IActionResult Preview(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            return RedirectToAction("preview", "solutions", new { id = id });
        }                    
    }
}
