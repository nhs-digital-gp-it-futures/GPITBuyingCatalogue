using System;
using System.Linq;
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
    internal const string AddOrEditSubProcessorView = "AddOrEditSubProcessor";

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

    [HttpGet("sub-processors/add")]
    public async Task<IActionResult> AddSubProcessor(
        CatalogueItemId solutionId)
    {
        var solution = await dataProcessingInformationService.GetSolutionWithDataProcessingInformation(solutionId);

        var model = new AddEditSubProcessorModel(solution) { BackLink = Url.Action(nameof(Index), new { solutionId }) };

        return View(AddOrEditSubProcessorView, model);
    }

    [HttpPost("sub-processors/add")]
    public async Task<IActionResult> AddSubProcessor(
        CatalogueItemId solutionId,
        AddEditSubProcessorModel model)
    {
        if (!ModelState.IsValid)
            return View(AddOrEditSubProcessorView, model);

        await dataProcessingInformationService.AddSubProcessor(
            solutionId,
            new SetSubProcessorModel(
                model.OrganisationName,
                model.Subject,
                model.Duration,
                model.ProcessingNature,
                model.PersonalDataTypes,
                model.DataSubjectCategories,
                model.PostProcessingPlan));

        return RedirectToAction(nameof(Index), new { solutionId });
    }

    [HttpGet("sub-processors/{subProcessorId:int}")]
    public async Task<IActionResult> EditSubProcessor(
        CatalogueItemId solutionId,
        int subProcessorId)
    {
        var solution = await dataProcessingInformationService.GetSolutionWithDataProcessingInformation(solutionId);
        var subProcessor =
            solution?.DataProcessingInformation?.SubProcessors.FirstOrDefault(x => x.Id == subProcessorId);

        if (subProcessor is null)
            return RedirectToAction(nameof(Index), new { solutionId });

        var model = new AddEditSubProcessorModel(solution, subProcessor) { BackLink = Url.Action(nameof(Index), new { solutionId }) };

        return View(AddOrEditSubProcessorView, model);
    }

    [HttpPost("sub-processors/{subProcessorId:int}")]
    public async Task<IActionResult> EditSubProcessor(
        CatalogueItemId solutionId,
        int subProcessorId,
        AddEditSubProcessorModel model)
    {
        if (!ModelState.IsValid)
            return View(AddOrEditSubProcessorView, model);

        await dataProcessingInformationService.EditSubProcessor(
            solutionId,
            new SetSubProcessorModel(
                model.OrganisationName,
                model.Subject,
                model.Duration,
                model.ProcessingNature,
                model.PersonalDataTypes,
                model.DataSubjectCategories,
                model.PostProcessingPlan,
                subProcessorId));

        return RedirectToAction(nameof(Index), new { solutionId });
    }

    [HttpGet("sub-processors/{subProcessorId:int}/delete")]
    public async Task<IActionResult> DeleteSubProcessor(
        CatalogueItemId solutionId,
        int subProcessorId)
    {
        var solution = await dataProcessingInformationService.GetSolutionWithDataProcessingInformation(solutionId);
        var subProcessor =
            solution?.DataProcessingInformation?.SubProcessors.FirstOrDefault(x => x.Id == subProcessorId);

        if (subProcessor is null)
            return RedirectToAction(nameof(Index), new { solutionId });

        var model = new DeleteSubProcessorModel(subProcessor)
        {
            BackLink = Url.Action(nameof(EditSubProcessor), new { solutionId, subProcessorId }),
        };

        return View(model);
    }

    [HttpPost("sub-processors/{subProcessorId:int}/delete")]
    public async Task<IActionResult> DeleteSubProcessor(
        CatalogueItemId solutionId,
        int subProcessorId,
        DeleteSubProcessorModel model)
    {
        _ = model;

        await dataProcessingInformationService.DeleteSubProcessor(solutionId, subProcessorId);

        return RedirectToAction(nameof(Index), new { solutionId });
    }
}
