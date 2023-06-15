using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DeleteApplicationTypeModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions")]
    public sealed class DeleteApplicationTypeController : Controller
    {
        private readonly ISolutionsService solutionsService;

        public DeleteApplicationTypeController(
            ISolutionsService solutionsService)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet("manage/{solutionId}/client-application-type/delete/{applicationType}")]
        public async Task<IActionResult> DeleteApplicationTypeConfirmation(CatalogueItemId solutionId, ApplicationType applicationType)
        {
            var solution = await solutionsService.GetSolutionThin(solutionId);

            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new DeleteApplicationTypeConfirmationModel(solution, applicationType)
            {
                BackLink = applicationType switch
                {
                    ApplicationType.BrowserBased => Url.Action(
                        nameof(BrowserBasedController.BrowserBased),
                        typeof(BrowserBasedController).ControllerName(),
                        new { solutionId }),
                    ApplicationType.Desktop => Url.Action(
                        nameof(DesktopBasedController.Desktop),
                        typeof(DesktopBasedController).ControllerName(),
                        new { solutionId }),
                    ApplicationType.MobileTablet => Url.Action(
                        nameof(MobileTabletBasedController.MobileTablet),
                        typeof(MobileTabletBasedController).ControllerName(),
                        new { solutionId }),
                    _ => throw new ArgumentOutOfRangeException(nameof(applicationType)),
                },
            };

            return View(model);
        }

        [HttpPost("manage/{solutionId}/client-application-type/delete/{applicationType}")]
        public async Task<IActionResult> DeleteApplicationTypeConfirmation(
            CatalogueItemId solutionId,
            ApplicationType applicationType,
            DeleteApplicationTypeConfirmationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await solutionsService.DeleteApplicationType(solutionId, applicationType);

            return RedirectToAction(
                nameof(CatalogueSolutionsController.ApplicationType),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { solutionId });
        }
    }
}
