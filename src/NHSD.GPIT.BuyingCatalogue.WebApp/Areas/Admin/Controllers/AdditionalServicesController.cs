using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AdditionalServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions/manage/{solutionId}/additional-services")]
    public sealed class AdditionalServicesController : Controller
    {
        private readonly ISolutionsService solutionsService;
        private readonly IAdditionalServicesService additionalServicesService;

        public AdditionalServicesController(
            ISolutionsService solutionsService,
            IAdditionalServicesService additionalServicesService)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.additionalServicesService = additionalServicesService ?? throw new ArgumentNullException(nameof(additionalServicesService));
        }

        [HttpGet]
        public async Task<IActionResult> Index(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var additionalServices = await additionalServicesService.GetAdditionalServicesBySolutionId(solutionId);

            return View(new AdditionalServicesModel(solution, additionalServices));
        }

        [HttpGet("{additionalServiceId}/edit-additional-service")]
        public async Task<IActionResult> EditAdditionalService(CatalogueItemId solutionId, CatalogueItemId additionalServiceId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var additionalService = await additionalServicesService.GetAdditionalService(solutionId, additionalServiceId);
            if (additionalService is null)
                return BadRequest($"No Additional Service with Id {additionalServiceId} found for Solution {solutionId}");

            var model = new EditAdditionalServiceModel(solution, additionalService)
            {
                BackLink = Url.Action(nameof(Index), new { solutionId }),
            };

            return View(model);
        }
    }
}
