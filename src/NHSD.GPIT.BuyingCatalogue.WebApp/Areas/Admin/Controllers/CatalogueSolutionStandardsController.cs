using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionStandards;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;

[Authorize(Policy = "AdminOnly")]
[Area("Admin")]
[Route("admin/catalogue-solutions/manage/{solutionId}/standards")]
public class CatalogueSolutionStandardsController(
    ISolutionsService solutionsService,
    ISolutionStandardsService solutionStandardsService) : Controller
{
    private readonly ISolutionsService solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
    private readonly ISolutionStandardsService solutionStandardsService =
        solutionStandardsService ?? throw new ArgumentNullException(nameof(solutionStandardsService));

    [HttpGet]
    public async Task<IActionResult> Index(CatalogueItemId solutionId)
    {
        var solutionName = await solutionsService.GetSolutionName(solutionId);
        var solutionStandards = await solutionStandardsService.GetSolutionStandards(solutionId);

        var model = new CatalogueSolutionStandardsModel(
            solutionId,
            solutionName,
            solutionStandards)
        {
            BackLink = Url.Action(
                nameof(CatalogueSolutionsController.ManageCatalogueSolution),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { area = typeof(CatalogueSolutionsController).AreaName(), solutionId }),
        };

        return View(model);
    }

    [HttpGet("{standardId}")]
    public async Task<IActionResult> Edit(CatalogueItemId solutionId, string standardId)
    {
        var solutionName = await solutionsService.GetSolutionName(solutionId);
        var standard = await solutionStandardsService.GetSolutionStandard(solutionId, standardId);

        var model = new CatalogueSolutionStandardModel(solutionId, solutionName, standard)
        {
            BackLink = Url.Action(
                nameof(Index),
                new { area = typeof(CatalogueSolutionStandardsController).AreaName(), solutionId }),
        };

        return View(model);
    }

    [HttpPost("{standardId}")]
    public async Task<IActionResult> Edit(
        CatalogueItemId solutionId,
        string standardId,
        CatalogueSolutionStandardModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (model.Compliance.HasValue)
            await solutionStandardsService.SetSolutionStandardStatus(solutionId, standardId, model.Compliance.Value);

        return RedirectToAction(
            nameof(Index),
            new { area = typeof(CatalogueSolutionStandardsController).AreaName(), solutionId });
    }
}
