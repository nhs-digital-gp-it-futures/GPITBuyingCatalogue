using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.DataProcessingInformationModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DataProcessingInformation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;

[Authorize(Policy = "AdminOnly")]
[Area("Admin")]
[Route("admin/catalogue-solutions/{solutionId}/data-processing")]
public class DataProcessingInformationController(IDataProcessingInformationService dataProcessingInformationService)
    : Controller
{
    private readonly IDataProcessingInformationService dataProcessingInformationService =
        dataProcessingInformationService ?? throw new ArgumentNullException(nameof(dataProcessingInformationService));

    [HttpGet]
    public async Task<IActionResult> Index(CatalogueItemId solutionId)
    {
        var solution = await dataProcessingInformationService.GetSolutionWithDataProcessingInformation(solutionId);

        var model = new DataProcessingInformationModel(solution)
        {
            BackLink = Url.Action(
                nameof(CatalogueSolutionsController.ManageCatalogueSolution),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { solutionId }),
        };

        return View(model);
    }

    [HttpGet("information")]
    public async Task<IActionResult> AddOrEditDataProcessingInformation(
        CatalogueItemId solutionId)
    {
        var solution = await dataProcessingInformationService.GetSolutionWithDataProcessingInformation(solutionId);

        var model = new AddEditDataProcessingInformationModel(solution)
        {
            BackLink = Url.Action(nameof(Index), new { solutionId }),
        };

        return View(model);
    }

    [HttpPost("information")]
    public async Task<IActionResult> AddOrEditDataProcessingInformation(
        CatalogueItemId solutionId,
        AddEditDataProcessingInformationModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        await dataProcessingInformationService.SetDataProcessingInformation(
            solutionId,
            new SetDataProcessingInformationModel(
                model.Subject,
                model.Duration,
                model.ProcessingNature,
                model.PersonalDataTypes,
                model.DataSubjectCategories,
                model.ProcessingLocation,
                model.AdditionalJurisdiction));

        return RedirectToAction(nameof(Index), new { solutionId });
    }

    [HttpGet("data-protection-officer")]
    public async Task<IActionResult> AddOrEditDataProtectionOfficer(
        CatalogueItemId solutionId)
    {
        var solution = await dataProcessingInformationService.GetSolutionWithDataProcessingInformation(solutionId);

        var model = new AddEditDataProtectionOfficerModel(solution)
        {
            BackLink = Url.Action(nameof(Index), new { solutionId }),
        };

        return View(model);
    }

    [HttpPost("data-protection-officer")]
    public async Task<IActionResult> AddOrEditDataProtectionOfficer(
        CatalogueItemId solutionId,
        AddEditDataProtectionOfficerModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        await dataProcessingInformationService.SetDataProtectionOfficer(
            solutionId,
            new SetDataProtectionOfficerModel(
                model.Name,
                model.EmailAddress,
                model.PhoneNumber));

        return RedirectToAction(nameof(Index), new { solutionId });
    }
}
