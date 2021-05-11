using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.Solution;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Route("marketing/supplier/solution/{id}")]
    public class SolutionController : Controller
    {
        private readonly ILogWrapper<SolutionController> _logger;
        private readonly IMapper _mapper;
        private readonly ISolutionsService _solutionsService;

        public SolutionController(ILogWrapper<SolutionController> logger, IMapper mapper,
            ISolutionsService solutionsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet]
        public async Task<IActionResult> Index(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var solution = await _solutionsService.GetSolution(id);
            if(solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            var model = _mapper.Map<CatalogueItem, SolutionStatusModel>(solution);

            return View(model);
        }

        [HttpGet("preview")]
        public IActionResult Preview(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            return RedirectToAction("preview", "solutions", new { id = id });
        }                    
    }
}
