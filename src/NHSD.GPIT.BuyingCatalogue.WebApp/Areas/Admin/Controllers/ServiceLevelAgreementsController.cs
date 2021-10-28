using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions/manage/{solutionId}/service-level-agreements")]
    public class ServiceLevelAgreementsController : Controller
    {
        private readonly IServiceLevelAgreementsService serviceLevelAgreementsService;
        private readonly ISolutionsService solutionsService;

        public ServiceLevelAgreementsController(
            IServiceLevelAgreementsService serviceLevelAgreementsService,
            ISolutionsService solutionsService)
        {
            this.serviceLevelAgreementsService = serviceLevelAgreementsService ?? throw new ArgumentNullException(nameof(serviceLevelAgreementsService));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet]
        public async Task<IActionResult> Index(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var sla = await serviceLevelAgreementsService.GetAllServiceLevelAgreementsForSolution(solutionId);

            if (sla is null)
            {
                return RedirectToAction(nameof(AddSlaLevel), new { solutionId });
            }

            return RedirectToAction(nameof(EditServiceLevelAgreement), new { solutionId });
        }

        [HttpGet("add-sla-level")]
        public async Task<IActionResult> AddSlaLevel(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new AddSlaTypeModel(solution)
            {
                BackLink = Url.Action(
                    nameof(CatalogueSolutionsController.ManageCatalogueSolution),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { solutionId }),
                Title = "Type of Catalogue Solution",
            };

            return View("AddEditSlaLevel", model);
        }

        [HttpPost("add-sla-level")]
        public async Task<IActionResult> AddSlaLevel(CatalogueItemId solutionId, AddSlaTypeModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("AddEditSlaLevel", model);
            }

            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var addSlaModel = new AddSlaModel
            {
                Solution = solution,
                SlaLevel = model.SlaLevel.Value,
            };

            await serviceLevelAgreementsService.AddServiceLevelAsync(addSlaModel);

            return RedirectToAction(nameof(EditServiceLevelAgreement), new { solutionId });
        }

        [HttpGet("edit-service-level-agreement")]
        public async Task<IActionResult> EditServiceLevelAgreement(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var serviceLevelAgreements = await serviceLevelAgreementsService.GetAllServiceLevelAgreementsForSolution(solutionId);

            var model = new EditServiceLevelAgreementModel(solution, serviceLevelAgreements)
            {
                BackLink = Url.Action(
                    nameof(CatalogueSolutionsController.ManageCatalogueSolution),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { solutionId }),
            };

            return View(model);
        }

        [HttpGet("edit-service-level-agreement/edit-sla-level")]
        public async Task<IActionResult> EditSlaLevel(CatalogueItemId solutionId)
        {
            var catalogueItem = await solutionsService.GetSolution(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var serviceLevelAgreements = await serviceLevelAgreementsService.GetAllServiceLevelAgreementsForSolution(solutionId);

            var model = new AddSlaTypeModel(catalogueItem)
            {
                SlaLevel = serviceLevelAgreements.SlaType,
                BackLink = Url.Action(nameof(EditServiceLevelAgreement), new { solutionId }),
                Title = "Type of Catalogue Solution",
            };

            return View("AddEditSlaLevel", model);
        }

        [HttpPost("edit-service-level-agreement/edit-sla-level")]
        public async Task<IActionResult> EditSlaLevel(CatalogueItemId solutionId, AddSlaTypeModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("AddEditSlaLevel", model);
            }

            var solution = await solutionsService.GetSolution(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            await serviceLevelAgreementsService.UpdateServiceLevelTypeAsync(solution, model.SlaLevel!.Value);

            return RedirectToAction(nameof(EditServiceLevelAgreement), new { solutionId });
        }
    }
}
