using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions/manage/{solutionId}")]
    public sealed class SolutionSectionsController : Controller
    {
        private readonly ISolutionsService solutionsService;

        public SolutionSectionsController(
            ISolutionsService solutionsService)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet("edit-description")]
        public async Task<IActionResult> EditDescription(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            var solutionDescription = new EditDescriptionModel(solution);

            return View(solutionDescription);
        }

        [HttpPost("edit-description")]
        public async Task<IActionResult> EditDescription(CatalogueItemId solutionId, EditDescriptionModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await solutionsService.SaveSolutionDescription(
                solutionId,
                model.Summary,
                model.Description,
                model.Link);

            return RedirectToAction(
                nameof(CatalogueSolutionsController.ManageCatalogueSolution),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { solutionId });
        }
    }
}
