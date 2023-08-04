using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Filters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SelectSolutionsModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;

[Authorize("Development")]
[Authorize("Buyer")]
[Area("Competitions")]
[Route("organisation/{internalOrgId}/competitions/{competitionId:int}")]
[CompetitionSolutionSelectionFilter]
public class CompetitionSelectSolutionsController : Controller
{
    private readonly IOrganisationsService organisationsService;
    private readonly ICompetitionsService competitionsService;

    public CompetitionSelectSolutionsController(
        IOrganisationsService organisationsService,
        ICompetitionsService competitionsService)
    {
        this.organisationsService =
            organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
        this.competitionsService = competitionsService ?? throw new ArgumentNullException(nameof(competitionsService));
    }

    [HttpGet("select-solutions")]
    public async Task<IActionResult> SelectSolutions(string internalOrgId, int competitionId)
    {
        var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
        var competition = await competitionsService.GetCompetitionWithServices(internalOrgId, competitionId);

        if (competition == null)
        {
            return RedirectToAction(
                nameof(CompetitionsDashboardController.Index),
                typeof(CompetitionsDashboardController).ControllerName(),
                new { internalOrgId });
        }

        var model = new SelectSolutionsModel(competition.Name, competition.CompetitionSolutions)
        {
            BackLinkText = "Go back to manage competitions",
            BackLink = Url.Action(nameof(CompetitionsDashboardController.Index), typeof(CompetitionsDashboardController).ControllerName(), new { internalOrgId }),
        };

        if (model.HasNoSolutions())
            await competitionsService.DeleteCompetition(organisation.Id, competition.Id);

        return View(model);
    }

    [HttpPost("select-solutions")]
    public async Task<IActionResult> SelectSolutions(
        string internalOrgId,
        int competitionId,
        SelectSolutionsModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);

        if (model.HasSingleSolution())
        {
            return model.IsDirectAward.GetValueOrDefault()
                ? await HandleDirectAward(organisation, competitionId)
                : await HandleDeleteOrder(organisation, competitionId);
        }

        await competitionsService.SetShortlistedSolutions(
            organisation.Id,
            competitionId,
            model.Solutions.Where(x => x.Selected).Select(x => x.SolutionId));

        return RedirectToAction(nameof(JustifySolutions), new { internalOrgId, competitionId });
    }

    [HttpGet("justify-solutions")]
    public async Task<IActionResult> JustifySolutions(string internalOrgId, int competitionId)
    {
        var competition = await competitionsService.GetCompetitionWithServices(internalOrgId, competitionId);

        if (competition.CompetitionSolutions.All(x => x.IsShortlisted))
            return RedirectToAction(nameof(ConfirmSolutions), new { internalOrgId, competitionId });

        var nonShortlistedSolutions = competition.CompetitionSolutions.Where(x => !x.IsShortlisted);

        var model = new JustifySolutionsModel(competition.Name, nonShortlistedSolutions)
        {
            BackLink = Url.Action(nameof(SelectSolutions), new { internalOrgId, competitionId }),
        };

        return View(model);
    }

    [HttpPost("justify-solutions")]
    public async Task<IActionResult> JustifySolutions(string internalOrgId, int competitionId, JustifySolutionsModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
        var solutionsJustification = model.Solutions.ToDictionary(x => x.SolutionId, x => x.Justification);

        await competitionsService.SetSolutionJustifications(organisation.Id, competitionId, solutionsJustification);

        return RedirectToAction(nameof(ConfirmSolutions), new { internalOrgId, competitionId });
    }

    [HttpGet("confirm-solutions")]
    public async Task<IActionResult> ConfirmSolutions(string internalOrgId, int competitionId)
    {
        var competition = await competitionsService.GetCompetitionWithServices(internalOrgId, competitionId);

        var backlink = competition.CompetitionSolutions.All(x => x.IsShortlisted)
            ? nameof(SelectSolutions)
            : nameof(JustifySolutions);

        var model = new ConfirmSolutionsModel(competition.Name, competition.CompetitionSolutions.ToList())
        {
            BackLink = Url.Action(backlink, new { internalOrgId, competitionId }),
        };

        return View(model);
    }

    [HttpPost("confirm-solutions")]
    public async Task<IActionResult> ConfirmSolutions(string internalOrgId, int competitionId, ConfirmSolutionsModel model)
    {
        _ = model;

        var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);

        await competitionsService.AcceptShortlist(organisation.Id, competitionId);

        return RedirectToAction(nameof(CompetitionTaskListController.Index), typeof(CompetitionTaskListController).ControllerName(), new { internalOrgId, competitionId });
    }

    private async Task<IActionResult> HandleDirectAward(Organisation organisation, int competitionId)
    {
        await competitionsService.CompleteCompetition(organisation.Id, competitionId);

        return RedirectToAction(
            nameof(OrderDescriptionController.NewOrderDescription),
            typeof(OrderDescriptionController).ControllerName(),
            new
            {
                Area = typeof(OrderDescriptionController).AreaName(),
                internalOrgId = organisation.InternalIdentifier,
                option = OrderTriageValue.Under40K,
                orderType = CatalogueItemType.Solution,
            });
    }

    private async Task<IActionResult> HandleDeleteOrder(Organisation organisation, int competitionId)
    {
        await competitionsService.DeleteCompetition(organisation.Id, competitionId);

        return RedirectToAction(nameof(CompetitionsDashboardController.Index), typeof(CompetitionsDashboardController).ControllerName(), new { internalOrgId = organisation.InternalIdentifier });
    }
}
