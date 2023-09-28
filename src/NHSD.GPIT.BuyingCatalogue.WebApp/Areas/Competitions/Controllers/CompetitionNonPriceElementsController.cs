using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels.FeaturesModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;

[Authorize("Development")]
[Authorize("Buyer")]
[Area("Competitions")]
[Route("organisation/{internalOrgId}/competitions/{competitionId:int}/non-price-elements")]
public class CompetitionNonPriceElementsController : Controller
{
    private readonly ICompetitionsService competitionsService;
    private readonly ICompetitionNonPriceElementsService competitionNonPriceElementsService;

    public CompetitionNonPriceElementsController(
        ICompetitionsService competitionsService,
        ICompetitionNonPriceElementsService competitionNonPriceElementsService)
    {
        this.competitionsService = competitionsService ?? throw new ArgumentNullException(nameof(competitionsService));
        this.competitionNonPriceElementsService = competitionNonPriceElementsService
            ?? throw new ArgumentNullException(nameof(competitionNonPriceElementsService));
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

        var selectedNonPriceElements =
            model.AvailableNonPriceElements.Where(x => x.Selected).Select(x => x.Value).ToList();

        (NonPriceElement? nextNonPriceElement, IEnumerable<NonPriceElement> remainingNonPriceElements) =
            GetNextNonPriceElement(selectedNonPriceElements);

        return RedirectToAction(
            nextNonPriceElement.ToString(),
            new
            {
                internalOrgId,
                competitionId,
                selectedNonPriceElements = string.Join(
                    ',',
                    remainingNonPriceElements),
            });
    }

    [HttpGet("add/interoperability")]
    public async Task<IActionResult> Interoperability(
        string internalOrgId,
        int competitionId,
        string returnUrl = null,
        string selectedNonPriceElements = null)
    {
        _ = returnUrl;
        _ = selectedNonPriceElements;

        var competition = await competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competitionId);

        var model = new SelectInteroperabilityCriteriaModel(competition)
        {
            BackLink = returnUrl ?? Url.Action(
                nameof(Index),
                new { internalOrgId, competitionId }),
            InternalOrgId = internalOrgId,
            CompetitionId = competitionId,
        };

        return View(model);
    }

    [HttpPost("add/interoperability")]
    public async Task<IActionResult> Interoperability(
        string internalOrgId,
        int competitionId,
        SelectInteroperabilityCriteriaModel model,
        string returnUrl = null,
        string selectedNonPriceElements = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        var im1Integrations = model.Im1Integrations.Where(x => x.Selected).Select(x => x.Value);
        var gpConnectIntegrations = model.GpConnectIntegrations.Where(x => x.Selected).Select(x => x.Value);

        await competitionsService.SetInteroperabilityCriteria(
            internalOrgId,
            competitionId,
            im1Integrations,
            gpConnectIntegrations);

        return GetRedirect(internalOrgId, competitionId, returnUrl, selectedNonPriceElements);
    }

    [HttpGet("add/implementation")]
    public async Task<IActionResult> Implementation(
        string internalOrgId,
        int competitionId,
        string returnUrl = null,
        string selectedNonPriceElements = null)
    {
        _ = returnUrl;
        _ = selectedNonPriceElements;

        var competition = await competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competitionId);

        var model = new AddImplementationCriteriaModel(competition)
        {
            BackLink = returnUrl ?? Url.Action(
                nameof(Index),
                new { internalOrgId, competitionId }),
            InternalOrgId = internalOrgId,
            CompetitionId = competitionId,
        };

        return View(model);
    }

    [HttpPost("add/implementation")]
    public async Task<IActionResult> Implementation(
        string internalOrgId,
        int competitionId,
        AddImplementationCriteriaModel model,
        string returnUrl = null,
        string selectedNonPriceElements = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        await competitionsService.SetImplementationCriteria(internalOrgId, competitionId, model.Requirements);

        return GetRedirect(internalOrgId, competitionId, returnUrl, selectedNonPriceElements);
    }

    [HttpGet("add/service-level")]
    public async Task<IActionResult> ServiceLevel(
        string internalOrgId,
        int competitionId,
        string returnUrl = null,
        string selectedNonPriceElements = null)
    {
        _ = returnUrl;
        _ = selectedNonPriceElements;

        var competition = await competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competitionId);

        var model = new AddServiceLevelCriteriaModel(competition)
        {
            BackLink = returnUrl ?? Url.Action(
                nameof(Index),
                new { internalOrgId, competitionId }),
            InternalOrgId = internalOrgId,
            CompetitionId = competitionId,
        };

        return View(model);
    }

    [HttpPost("add/service-level")]
    public async Task<IActionResult> ServiceLevel(
        string internalOrgId,
        int competitionId,
        AddServiceLevelCriteriaModel model,
        string returnUrl = null,
        string selectedNonPriceElements = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        await competitionsService.SetServiceLevelCriteria(
            internalOrgId,
            competitionId,
            model.TimeFrom.GetValueOrDefault(),
            model.TimeUntil.GetValueOrDefault(),
            model.ApplicableDays);

        return GetRedirect(internalOrgId, competitionId, returnUrl, selectedNonPriceElements);
    }

    [HttpGet("add/features")]
    public async Task<IActionResult> Features(
        string internalOrgId,
        int competitionId,
        string returnUrl = null,
        string selectedNonPriceElements = null)
    {
        _ = returnUrl;

        var competition = await competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competitionId);

        if (!competition.NonPriceElements?.Features?.Any() ?? true)
        {
            return RedirectToAction(
                nameof(FeatureRequirement),
                new { internalOrgId, competitionId, selectedNonPriceElements, });
        }

        var model = new FeaturesRequirementsModel(competition)
        {
            BackLink = Url.Action(nameof(Index), new { internalOrgId, competitionId }),
            InternalOrgId = internalOrgId,
            SelectedNonPriceElements = selectedNonPriceElements,
        };

        return View(model);
    }

    [HttpPost("add/features")]
    public IActionResult Features(
        string internalOrgId,
        int competitionId,
        FeaturesRequirementsModel model,
        string returnUrl = null,
        string selectedNonPriceElements = null)
    {
        _ = model;

        return GetRedirect(internalOrgId, competitionId, returnUrl, selectedNonPriceElements);
    }

    [HttpGet("add/features/requirement")]
    public async Task<IActionResult> FeatureRequirement(
        string internalOrgId,
        int competitionId,
        string returnUrl = null,
        string selectedNonPriceElements = null)
    {
        _ = selectedNonPriceElements;

        var competition = await competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competitionId);

        var model = new FeaturesRequirementModel(competition)
        {
            BackLink = returnUrl ?? (competition.NonPriceElements?.Features?.Any() ?? false
                ? Url.Action(
                    nameof(Features),
                    new { internalOrgId, competitionId, returnUrl, selectedNonPriceElements })
                : Url.Action(nameof(Index), new { internalOrgId, competitionId })),
        };

        return View(model);
    }

    [HttpPost("add/features/requirement")]
    public async Task<IActionResult> FeatureRequirement(
        string internalOrgId,
        int competitionId,
        FeaturesRequirementModel model,
        string returnUrl = null,
        string selectedNonPriceElements = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        await competitionNonPriceElementsService.AddFeatureRequirement(
            internalOrgId,
            competitionId,
            model.Requirements,
            model.SelectedCompliance!.Value);

        return returnUrl is not null
            ? Redirect(returnUrl)
            : RedirectToAction(
                nameof(Features),
                new { internalOrgId, competitionId, selectedNonPriceElements });
    }

    [HttpGet("add/features/requirement/{requirementId:int}")]
    public async Task<IActionResult> EditFeatureRequirement(
        string internalOrgId,
        int competitionId,
        int requirementId,
        string returnUrl = null,
        string selectedNonPriceElements = null)
    {
        _ = selectedNonPriceElements;

        var competition = await competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competitionId);
        var requirement = competition.NonPriceElements.Features.FirstOrDefault(x => x.Id == requirementId);

        if (requirement is null)
            return GetRedirect(internalOrgId, competitionId, returnUrl, selectedNonPriceElements);

        var model = new FeaturesRequirementModel(competition, requirement)
        {
            BackLink = !string.IsNullOrWhiteSpace(returnUrl)
                ? returnUrl
                : Url.Action(
                    nameof(Features),
                    new { internalOrgId, competitionId, returnUrl, selectedNonPriceElements }),
        };

        return View("FeatureRequirement", model);
    }

    [HttpPost("add/features/requirement/{requirementId:int}")]
    public async Task<IActionResult> EditFeatureRequirement(
        string internalOrgId,
        int competitionId,
        int requirementId,
        FeaturesRequirementModel model,
        string returnUrl = null,
        string selectedNonPriceElements = null)
    {
        if (!ModelState.IsValid)
            return View("FeatureRequirement", model);

        await competitionNonPriceElementsService.EditFeatureRequirement(
            internalOrgId,
            competitionId,
            requirementId,
            model.Requirements,
            model.SelectedCompliance!.Value);

        return returnUrl is not null
            ? Redirect(returnUrl)
            : RedirectToAction(
                nameof(Features),
                new { internalOrgId, competitionId, selectedNonPriceElements });
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
            model.ServiceLevel,
            model.Features);

        return RedirectToAction(
            nameof(CompetitionTaskListController.Index),
            typeof(CompetitionTaskListController).ControllerName(),
            new { internalOrgId, competitionId });
    }

    [HttpGet("delete/{nonPriceElement}")]
    public IActionResult Delete(
        string internalOrgId,
        int competitionId,
        NonPriceElement nonPriceElement)
    {
        _ = internalOrgId;
        _ = competitionId;

        var model = new DeleteNonPriceElementModel(nonPriceElement)
        {
            BackLink = Url.Action(nonPriceElement.ToString(), new { internalOrgId, competitionId }),
        };

        return View(model);
    }

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

    internal IActionResult GetRedirect(
        string internalOrgId,
        int competitionId,
        string returnUrl,
        string selectedNonPriceElements)
    {
        if (string.IsNullOrWhiteSpace(selectedNonPriceElements))
        {
            if (!string.IsNullOrWhiteSpace(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(Index), new { internalOrgId, competitionId });
        }

        var parsedNonPriceElements = ParseNonPriceElements(selectedNonPriceElements);
        (NonPriceElement? nextNonPriceElement, IEnumerable<NonPriceElement> remainingNonPriceElements) =
            GetNextNonPriceElement(parsedNonPriceElements.ToList());

        return RedirectToAction(
            nextNonPriceElement.ToString(),
            new
            {
                internalOrgId,
                competitionId,
                selectedNonPriceElements = string.Join(',', remainingNonPriceElements),
            });
    }

    private static (NonPriceElement? NextNonPriceElement, IEnumerable<NonPriceElement> RemainingNonPriceElements)
        GetNextNonPriceElement(
            ICollection<NonPriceElement> nonPriceElements)
    {
        if (!nonPriceElements.Any()) return (null, nonPriceElements);

        var nextNonPriceElement = nonPriceElements.First();

        return (nextNonPriceElement, nonPriceElements.Where(x => x != nextNonPriceElement));
    }

    private static IEnumerable<NonPriceElement> ParseNonPriceElements(string nonPriceElementsQuery) =>
        nonPriceElementsQuery.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Where(x => Enum.TryParse<NonPriceElement>(x, out var parsedEnum) && Enum.IsDefined(parsedEnum))
            .Select(Enum.Parse<NonPriceElement>);
}
