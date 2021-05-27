using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.Solution;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Route("marketing/supplier/solution/{id}")]
    public class SolutionController : Controller
    {
        private readonly IMapper mapper;
        private readonly ISolutionsService solutionsService;

        public SolutionController(
            IMapper mapper,
            ISolutionsService solutionsService)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet]
        public async Task<IActionResult> Index(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"index-{nameof(id)}");

            var solution = await solutionsService.GetSolution(id);
            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            var model = mapper.Map<CatalogueItem, SolutionStatusModel>(solution);

            return View(model);
        }

        [HttpGet("preview")]
        public IActionResult Preview(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"preview-{nameof(id)}");

            return RedirectToAction("preview", "solutions", new { id });
        }
    }
}
