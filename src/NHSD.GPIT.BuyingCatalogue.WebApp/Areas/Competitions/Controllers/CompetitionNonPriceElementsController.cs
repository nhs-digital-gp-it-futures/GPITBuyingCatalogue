﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;

[Authorize("Development")]
[Authorize("Buyer")]
[Area("Competitions")]
[Route("organisation/{internalOrgId}/competitions/{competitionId:int}/non-price-elements")]
public class CompetitionNonPriceElementsController : Controller
{
    private readonly ICompetitionsService competitionsService;

    public CompetitionNonPriceElementsController(
        ICompetitionsService competitionsService)
    {
        this.competitionsService = competitionsService ?? throw new ArgumentNullException(nameof(competitionsService));
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> Index(
        string internalOrgId,
        int competitionId)
    {
        var competition = await competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competitionId);

        var model = new NonPriceElementsModel(competition)
        {
            BackLink = Url.Action(
                nameof(CompetitionTaskListController.Index),
                typeof(CompetitionTaskListController).ControllerName(),
                new { internalOrgId, competitionId }),
        };

        return View(model);
    }

    [HttpGet("add")]
    public async Task<IActionResult> AddNonPriceElement(
        string internalOrgId,
        int competitionId)
    {
        var competition = await competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competitionId);

        var model = new AddNonPriceElementModel(competition)
        {
            BackLink = Url.Action(nameof(Index), new { internalOrgId, competitionId }),
        };

        if (!model.AvailableNonPriceElements.Any())
            return RedirectToAction(nameof(Index), new { internalOrgId, competitionId });

        return View(model);
    }

    [HttpPost("add")]
    public IActionResult AddNonPriceElement(
        string internalOrgId,
        int competitionId,
        AddNonPriceElementModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var selectedNonPriceElement = model.AvailableNonPriceElements.Count == 1
            ? model.AvailableNonPriceElements.FirstOrDefault(x => x.Selected)?.Value
            : model.SelectedNonPriceElement;

        return RedirectToAction(
            selectedNonPriceElement is null ? nameof(Index) : selectedNonPriceElement.GetValueOrDefault().ToString(),
            new { internalOrgId, competitionId });
    }

    [HttpGet("add/interoperability")]
    public async Task<IActionResult> Interoperability(
        string internalOrgId,
        int competitionId,
        bool? isEdit = false,
        string returnUrl = null)
    {
        _ = returnUrl;

        var competition = await competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competitionId);

        var model = new SelectInteroperabilityCriteriaModel(competition)
        {
            BackLink = returnUrl ?? Url.Action(isEdit.GetValueOrDefault() ? nameof(Index) : nameof(AddNonPriceElement), new { internalOrgId, competitionId }),
        };

        return View(model);
    }

    [HttpPost("add/interoperability")]
    public async Task<IActionResult> Interoperability(
        string internalOrgId,
        int competitionId,
        SelectInteroperabilityCriteriaModel model,
        string returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        var im1Integrations = model.Im1Integrations.Where(x => x.Selected)?.Select(x => x.Value);
        var gpConnectIntegrations = model.GpConnectIntegrations.Where(x => x.Selected)?.Select(x => x.Value);

        await competitionsService.SetInteroperabilityCriteria(
            internalOrgId,
            competitionId,
            im1Integrations,
            gpConnectIntegrations);

        return GetRedirect(internalOrgId, competitionId, returnUrl);
    }

    [HttpGet("add/implementation")]
    public async Task<IActionResult> Implementation(
        string internalOrgId,
        int competitionId,
        bool? isEdit = false,
        string returnUrl = null)
    {
        _ = returnUrl;

        var competition = await competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competitionId);

        var model = new AddImplementationCriteriaModel(competition)
        {
            BackLink = returnUrl ?? Url.Action(isEdit.GetValueOrDefault() ? nameof(Index) : nameof(AddNonPriceElement), new { internalOrgId, competitionId }),
        };

        return View(model);
    }

    [HttpPost("add/implementation")]
    public async Task<IActionResult> Implementation(
        string internalOrgId,
        int competitionId,
        AddImplementationCriteriaModel model,
        string returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        await competitionsService.SetImplementationCriteria(internalOrgId, competitionId, model.Requirements);

        return GetRedirect(internalOrgId, competitionId, returnUrl);
    }

    [HttpGet("add/service-level")]
    public async Task<IActionResult> ServiceLevel(
        string internalOrgId,
        int competitionId,
        bool? isEdit = false,
        string returnUrl = null)
    {
        _ = returnUrl;

        var competition = await competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competitionId);

        var model = new AddServiceLevelCriteriaModel(competition)
        {
            BackLink = returnUrl ?? Url.Action(isEdit.GetValueOrDefault() ? nameof(Index) : nameof(AddNonPriceElement), new { internalOrgId, competitionId }),
        };

        return View(model);
    }

    [HttpPost("add/service-level")]
    public async Task<IActionResult> ServiceLevel(
        string internalOrgId,
        int competitionId,
        AddServiceLevelCriteriaModel model,
        string returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        await competitionsService.SetServiceLevelCriteria(
            internalOrgId,
            competitionId,
            model.TimeFrom.GetValueOrDefault(),
            model.TimeUntil.GetValueOrDefault(),
            model.ApplicableDays);

        return GetRedirect(internalOrgId, competitionId, returnUrl);
    }

    [HttpGet("weights")]
    public async Task<IActionResult> Weights(
        string internalOrgId,
        int competitionId)
    {
        var competition = await competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competitionId);

        var model = new NonPriceElementWeightsModel(competition)
        {
            BackLink = Url.Action(
                nameof(CompetitionTaskListController.Index),
                typeof(CompetitionTaskListController).ControllerName(),
                new { internalOrgId, competitionId }),
        };

        return View(model);
    }

    [HttpPost("weights")]
    public async Task<IActionResult> Weights(
        string internalOrgId,
        int competitionId,
        NonPriceElementWeightsModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        await competitionsService.SetNonPriceWeights(
            internalOrgId,
            competitionId,
            model.Implementation,
            model.Interoperability,
            model.ServiceLevel);

        return RedirectToAction(
            nameof(CompetitionTaskListController.Index),
            typeof(CompetitionTaskListController).ControllerName(),
            new { internalOrgId, competitionId });
    }

    private IActionResult GetRedirect(string internalOrgId, int competitionId, string returnUrl) =>
        string.IsNullOrWhiteSpace(returnUrl)
            ? RedirectToAction(nameof(Index), new { internalOrgId, competitionId })
            : Redirect(returnUrl);
}
