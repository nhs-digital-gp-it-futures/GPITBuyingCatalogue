using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Integrations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Filters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels.FeaturesModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;

[Authorize("Buyer")]
[Area("Competitions")]
[Route("organisation/{internalOrgId}/competitions/{competitionId:int}/non-price-elements")]
public class CompetitionNonPriceElementsController(
    ICompetitionsService competitionsService,
    ICompetitionNonPriceElementsService competitionNonPriceElementsService,
    IIntegrationsService integrationsService)
    : Controller
{
    private readonly ICompetitionsService competitionsService =
        competitionsService ?? throw new ArgumentNullException(nameof(competitionsService));

    private readonly ICompetitionNonPriceElementsService competitionNonPriceElementsService =
        competitionNonPriceElementsService
        ?? throw new ArgumentNullException(nameof(competitionNonPriceElementsService));

    private readonly IIntegrationsService integrationsService =
        integrationsService ?? throw new ArgumentNullException(nameof(integrationsService));

    [HttpGet("dashboard")]
    public async Task<IActionResult> Index(
        string internalOrgId,
        int competitionId)
    {
        var competition = await competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competitionId);
        var integrations = await integrationsService.GetIntegrations();

        var model = new NonPriceElementsModel(competition, integrations)
        {
            BackLink = Url.Action(
                nameof(CompetitionTaskListController.Index),
                typeof(CompetitionTaskListController).ControllerName(),
                new { internalOrgId, competitionId }),
        };

        return View(model);
    }

    [CriteriaReviewedGuardFilter]
    [HttpGet("add/interoperability")]
    public async Task<IActionResult> Interoperability(
        string internalOrgId,
        int competitionId,
        string returnUrl = null)
    {
        _ = returnUrl;

        var competition = await competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competitionId);
        var integrations = await integrationsService.GetIntegrationsWithTypes();

        var model = new SelectInteroperabilityCriteriaModel(competition, integrations)
        {
            BackLink = GetBackLink(internalOrgId, competitionId, returnUrl),
            InternalOrgId = internalOrgId,
            CompetitionId = competitionId,
        };

        model.CanDelete &= returnUrl is null;

        return View(model);
    }

    [CriteriaReviewedGuardFilter]
    [HttpPost("add/interoperability")]
    public async Task<IActionResult> Interoperability(
        string internalOrgId,
        int competitionId,
        SelectInteroperabilityCriteriaModel model,
        string returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        var integrations = model.Integrations.SelectMany(x => x.Value).Where(x => x.Selected).Select(x => x.Value);

        await competitionsService.SetInteroperabilityCriteria(
            internalOrgId,
            competitionId,
            integrations);

        return GetRedirect(internalOrgId, competitionId, returnUrl);
    }

    [CriteriaReviewedGuardFilter]
    [HttpGet("add/implementation")]
    public async Task<IActionResult> Implementation(
        string internalOrgId,
        int competitionId,
        string returnUrl = null)
    {
        _ = returnUrl;

        var competition = await competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competitionId);

        var model = new AddImplementationCriteriaModel(competition)
        {
            BackLink = GetBackLink(internalOrgId, competitionId, returnUrl),
            InternalOrgId = internalOrgId,
            CompetitionId = competitionId,
        };

        model.CanDelete &= returnUrl is null;

        return View(model);
    }

    [CriteriaReviewedGuardFilter]
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

    [CriteriaReviewedGuardFilter]
    [HttpGet("add/service-level")]
    public async Task<IActionResult> ServiceLevel(
        string internalOrgId,
        int competitionId,
        string returnUrl = null)
    {
        _ = returnUrl;

        var competition = await competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competitionId);

        var model = new AddServiceLevelCriteriaModel(competition)
        {
            BackLink = GetBackLink(internalOrgId, competitionId, returnUrl),
            InternalOrgId = internalOrgId,
            CompetitionId = competitionId,
        };

        model.CanDelete &= returnUrl is null;

        return View(model);
    }

    [CriteriaReviewedGuardFilter]
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
            model.ApplicableDays.Where(x => x.Selected).Select(x => x.Value),
            model.IncludesBankHolidays!.Value);

        return GetRedirect(internalOrgId, competitionId, returnUrl);
    }

    [CriteriaReviewedGuardFilter]
    [HttpGet("add/feature")]
    public async Task<IActionResult> Feature(
        string internalOrgId,
        int competitionId)
    {
        var competition = await competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competitionId);

        var model = new FeatureModel(competition)
        {
            BackLink = Url.Action(nameof(Index), new { internalOrgId, competitionId }),
        };

        return View(model);
    }

    [CriteriaReviewedGuardFilter]
    [HttpPost("add/feature")]
    public async Task<IActionResult> Feature(
        string internalOrgId,
        int competitionId,
        FeatureModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        await competitionNonPriceElementsService.AddFeatureRequirement(
            internalOrgId,
            competitionId,
            model.Requirements,
            model.SelectedCompliance!.Value);

        return RedirectToAction(nameof(Index), new { internalOrgId, competitionId });
    }

    [CriteriaReviewedGuardFilter]
    [HttpGet("edit/feature/{requirementId:int}")]
    public async Task<IActionResult> EditFeature(
        string internalOrgId,
        int competitionId,
        int requirementId,
        string returnUrl = null)
    {
        _ = returnUrl;

        var competition = await competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competitionId);
        var requirement = competition.NonPriceElements.Features.FirstOrDefault(x => x.Id == requirementId);

        if (requirement is null)
            return RedirectToAction(nameof(Index), new { internalOrgId, competitionId });

        var model = new FeatureModel(competition, requirement)
        {
            BackLink = GetBackLink(internalOrgId, competitionId, returnUrl),
            InternalOrgId = internalOrgId,
        };

        return View("Feature", model);
    }

    [CriteriaReviewedGuardFilter]
    [HttpPost("edit/feature/{requirementId:int}")]
    public async Task<IActionResult> EditFeature(
        string internalOrgId,
        int competitionId,
        int requirementId,
        FeatureModel model,
        string returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View("Feature", model);

        await competitionNonPriceElementsService.EditFeatureRequirement(
            internalOrgId,
            competitionId,
            requirementId,
            model.Requirements,
            model.SelectedCompliance!.Value);

        return GetRedirect(internalOrgId, competitionId, returnUrl);
    }

    [CriteriaReviewedGuardFilter]
    [HttpGet("add/feature/{requirementId:int}/delete")]
    public async Task<IActionResult> DeleteFeature(
        string internalOrgId,
        int competitionId,
        int requirementId,
        string returnUrl = null)
    {
        _ = returnUrl;

        var competition = await competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competitionId);

        var model = new DeleteNonPriceElementModel(NonPriceElement.Features, competition, featureId: requirementId)
        {
            BackLink = GetBackLink(internalOrgId, competitionId, returnUrl),
        };

        return View("Delete", model);
    }

    [CriteriaReviewedGuardFilter]
    [HttpPost("add/feature/{requirementId:int}/delete")]
    public async Task<IActionResult> DeleteFeature(
        string internalOrgId,
        int competitionId,
        int requirementId,
        DeleteNonPriceElementModel model,
        string returnUrl = null)
    {
        _ = model;

        var competition = await competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competitionId);
        var features = competition.NonPriceElements.Features;
        var numberOfFeatures = features.Count;

        if (numberOfFeatures > 1)
        {
            await competitionNonPriceElementsService.DeleteFeatureRequirement(
                internalOrgId,
                competitionId,
                requirementId);
        }
        else
        {
            await competitionNonPriceElementsService.DeleteNonPriceElement(
                internalOrgId,
                competitionId,
                NonPriceElement.Features);
        }

        return GetRedirect(internalOrgId, competitionId, returnUrl);
    }

    [CriteriaReviewedGuardFilter]
    [HttpGet("weights")]
    public async Task<IActionResult> Weights(
        string internalOrgId,
        int competitionId,
        string returnUrl = null)
    {
        var competition = await competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competitionId);

        var model = new NonPriceElementWeightsModel(competition)
        {
            BackLink = returnUrl ?? Url.Action(
                nameof(CompetitionTaskListController.Index),
                typeof(CompetitionTaskListController).ControllerName(),
                new { internalOrgId, competitionId }),
        };

        return View(model);
    }

    [CriteriaReviewedGuardFilter]
    [HttpPost("weights")]
    public async Task<IActionResult> Weights(
        string internalOrgId,
        int competitionId,
        NonPriceElementWeightsModel model,
        string returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        await competitionsService.SetNonPriceWeights(
            internalOrgId,
            competitionId,
            model.Implementation,
            model.Interoperability,
            model.ServiceLevel,
            model.Features);

        return !string.IsNullOrWhiteSpace(returnUrl)
            ? Redirect(returnUrl)
            : RedirectToAction(
                nameof(CompetitionTaskListController.Index),
                typeof(CompetitionTaskListController).ControllerName(),
                new { internalOrgId, competitionId });
    }

    [CriteriaReviewedGuardFilter]
    [HttpGet("delete/{nonPriceElement}")]
    public async Task<IActionResult> Delete(
        string internalOrgId,
        int competitionId,
        NonPriceElement nonPriceElement)
    {
        _ = internalOrgId;
        var competition = await competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competitionId);

        IEnumerable<Integration> integrations = new List<Integration>();
        if (nonPriceElement == NonPriceElement.Interoperability)
        {
            integrations = await integrationsService.GetIntegrationsWithTypes();
        }

        var model = new DeleteNonPriceElementModel(nonPriceElement, competition, integrations)
        {
            BackLink = Url.Action(nonPriceElement.ToString(), new { internalOrgId, competitionId }),
        };

        return View(model);
    }

    [CriteriaReviewedGuardFilter]
    [HttpPost("delete/{nonPriceElement}")]
    public async Task<IActionResult> Delete(
        string internalOrgId,
        int competitionId,
        NonPriceElement nonPriceElement,
        DeleteNonPriceElementModel model)
    {
        _ = model;

        await competitionNonPriceElementsService.DeleteNonPriceElement(internalOrgId, competitionId, nonPriceElement);

        return RedirectToAction(nameof(Index), new { internalOrgId, competitionId });
    }

    internal string GetBackLink(
        string internalOrgId,
        int competitionId,
        string returnUrl)
    {
        return returnUrl ?? Url.Action(nameof(Index), new { internalOrgId, competitionId });
    }

    internal IActionResult GetRedirect(
        string internalOrgId,
        int competitionId,
        string returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction(nameof(Index), new { internalOrgId, competitionId });
    }
}
