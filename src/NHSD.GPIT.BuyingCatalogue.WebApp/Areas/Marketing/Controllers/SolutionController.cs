using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.Solution;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Route("marketing/supplier/solution/{solutionId}")]
    public sealed class SolutionController : Controller
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
        public async Task<IActionResult> Index(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            if (solution is null)
                return BadRequest($"No Catalogue Item found for Id: {solutionId}");

            var model = mapper.Map<CatalogueItem, SolutionStatusModel>(solution);

            return View(model);
        }

        [HttpGet("preview")]
        public IActionResult Preview(CatalogueItemId solutionId)
        {
            return RedirectToAction(
                nameof(SolutionDetailsController.PreviewSolutionDetail),
                typeof(SolutionDetailsController).ControllerName(),
                new { solutionId });
        }
    }
}
