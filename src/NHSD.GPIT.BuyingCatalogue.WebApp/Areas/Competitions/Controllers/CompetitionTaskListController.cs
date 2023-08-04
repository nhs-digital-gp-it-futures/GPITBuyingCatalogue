using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;

[Authorize("Development")]
[Authorize("Buyer")]
[Area("Competitions")]
[Route("organisation/{internalOrgId}/competitions/{competitionId:int}")]
public class CompetitionTaskListController : Controller
{
    private readonly IOrganisationsService organisationsService;
    private readonly ICompetitionsService competitionsService;

    public CompetitionTaskListController(
        IOrganisationsService organisationsService,
        ICompetitionsService competitionsService)
    {
        this.organisationsService =
            organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
        this.competitionsService =
            competitionsService ?? throw new ArgumentNullException(nameof(competitionsService));
    }

    [HttpGet]
    public async Task<IActionResult> Index(string internalOrgId, int competitionId)
    {
        var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
        var competition = await competitionsService.GetCompetitionTaskList(internalOrgId, competitionId);

        var model = new CompetitionTaskListViewModel(organisation, competition)
        {
            BackLink = Url.Action(
                nameof(CompetitionsDashboardController.Index),
                typeof(CompetitionsDashboardController).ControllerName(),
                new { internalOrgId }),
        };

        return View(model);
    }

    [HttpGet("shortlisted-solutions")]
    public async Task<IActionResult> ShortlistedSolutions(string internalOrgId, int competitionId)
    {
        var competition = await competitionsService.GetCompetitionWithServices(internalOrgId, competitionId);

        var model = new CompetitionShortlistedSolutionsModel(competition)
        {
            BackLink = Url.Action(
                nameof(Index),
                new { internalOrgId, competitionId }),
        };

        return View(model);
    }

    [HttpGet("contract-length")]
    public async Task<IActionResult> ContractLength(string internalOrgId, int competitionId)
    {
        var competition = await GetCompetition(internalOrgId, competitionId);

        var model = new CompetitionContractModel(competition)
        {
            BackLink = Url.Action(nameof(Index), new { internalOrgId, competitionId }),
        };

        return View(model);
    }

    [HttpPost("contract-length")]
    public async Task<IActionResult> ContractLength(
        string internalOrgId,
        int competitionId,
        CompetitionContractModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        await competitionsService.SetContractLength(internalOrgId, competitionId, model.ContractLength.GetValueOrDefault());

        return RedirectToAction(nameof(Index), new { internalOrgId, competitionId });
    }

    [HttpGet("award-criteria")]
    public async Task<IActionResult> AwardCriteria(string internalOrgId, int competitionId)
    {
        var competition = await GetCompetition(internalOrgId, competitionId);

        var model = new CompetitionAwardCriteriaModel(competition)
        {
            BackLink = Url.Action(nameof(Index), new { internalOrgId, competitionId }),
        };

        return View(model);
    }

    [HttpPost("award-criteria")]
    public async Task<IActionResult> AwardCriteria(string internalOrgId, int competitionId, CompetitionAwardCriteriaModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var includesNonPrice = model.IncludesNonPrice.GetValueOrDefault();

        var competition = await GetCompetition(internalOrgId, competitionId);

        if (competition.IncludesNonPrice.GetValueOrDefault() && !includesNonPrice)
            return RedirectToAction(nameof(ConfirmAwardCriteria), new { internalOrgId, competitionId });

        await competitionsService.SetCompetitionCriteria(
            internalOrgId,
            competitionId,
            model.IncludesNonPrice.GetValueOrDefault());

        return RedirectToAction(nameof(Index), new { internalOrgId, competitionId });
    }

    [HttpGet("award-criteria/confirm")]
    public async Task<IActionResult> ConfirmAwardCriteria(string internalOrgId, int competitionId)
    {
        var competition = await GetCompetition(internalOrgId, competitionId);

        var model = new CompetitionAwardCriteriaModel(competition)
        {
            BackLink = Url.Action(nameof(AwardCriteria), new { internalOrgId, competitionId }),
        };

        return View(model);
    }

    [HttpPost("award-criteria/confirm")]
    public async Task<IActionResult> ConfirmAwardCriteria(
        string internalOrgId,
        int competitionId,
        CompetitionAwardCriteriaModel model)
    {
        _ = model;

        await competitionsService.SetCompetitionCriteria(internalOrgId, competitionId, false);
        await competitionsService.RemoveNonPriceElements(internalOrgId, competitionId);

        return RedirectToAction(nameof(Index), new { internalOrgId, competitionId });
    }

    [HttpGet("weightings")]
    public async Task<IActionResult> Weightings(
        string internalOrgId,
        int competitionId,
        string returnUrl = null)
    {
        var competition = await competitionsService.GetCompetitionWithWeightings(internalOrgId, competitionId);

        var model = new CompetitionWeightingsModel(competition)
        {
            BackLink = returnUrl ?? Url.Action(nameof(Index), new { internalOrgId, competitionId }),
        };

        return View(model);
    }

    [HttpPost("weightings")]
    public async Task<IActionResult> Weightings(
        string internalOrgId,
        int competitionId,
        CompetitionWeightingsModel model,
        string returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        await competitionsService.SetCompetitionWeightings(
            internalOrgId,
            competitionId,
            model.Price.GetValueOrDefault(),
            model.NonPrice.GetValueOrDefault());

        return string.IsNullOrWhiteSpace(returnUrl)
            ? RedirectToAction(nameof(Index), new { internalOrgId, competitionId })
            : Redirect(returnUrl);
    }

    [HttpGet("review-criteria")]
    public async Task<IActionResult> ReviewCriteria(string internalOrgId, int competitionId)
    {
        var competition = await competitionsService.GetCompetitionCriteriaReview(internalOrgId, competitionId);

        var model = new CompetitionReviewCriteriaModel(competition)
        {
            BackLink = Url.Action(nameof(Index), new { internalOrgId, competitionId }),
        };

        return View(model);
    }

    [HttpPost("review-criteria")]
    public async Task<IActionResult> ReviewCriteria(
        string internalOrgId,
        int competitionId,
        CompetitionReviewCriteriaModel model)
    {
        _ = model;

        await competitionsService.SetCriteriaReviewed(internalOrgId, competitionId);

        return RedirectToAction(nameof(Index), new { internalOrgId, competitionId });
    }

    private async Task<Competition> GetCompetition(string internalOrgId, int competitionId)
    {
        var competition = await competitionsService.GetCompetition(internalOrgId, competitionId);

        return competition;
    }
}
