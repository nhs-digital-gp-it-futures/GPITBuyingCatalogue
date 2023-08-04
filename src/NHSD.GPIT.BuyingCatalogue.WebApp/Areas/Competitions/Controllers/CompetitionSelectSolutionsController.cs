using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
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
    private readonly ICompetitionsService competitionsService;

    public CompetitionSelectSolutionsController(
        ICompetitionsService competitionsService)
    {
        this.competitionsService = competitionsService ?? throw new ArgumentNullException(nameof(competitionsService));
    }

    [HttpGet("select-solutions")]
    public async Task<IActionResult> SelectSolutions(string internalOrgId, int competitionId)
    {
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
            await competitionsService.DeleteCompetition(internalOrgId, competition.Id);

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

        if (model.HasSingleSolution())
        {
            return model.IsDirectAward.GetValueOrDefault()
                ? await HandleDirectAward(internalOrgId, competitionId)
                : await HandleDeleteOrder(internalOrgId, competitionId);
        }

        await competitionsService.SetShortlistedSolutions(
            internalOrgId,
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

        var solutionsJustification = model.Solutions.ToDictionary(x => x.SolutionId, x => x.Justification);

        await competitionsService.SetSolutionJustifications(internalOrgId, competitionId, solutionsJustification);

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

        await competitionsService.AcceptShortlist(internalOrgId, competitionId);

        return RedirectToAction(nameof(CompetitionTaskListController.Index), typeof(CompetitionTaskListController).ControllerName(), new { internalOrgId, competitionId });
    }

    private async Task<IActionResult> HandleDirectAward(string internalOrgId, int competitionId)
    {
        await competitionsService.CompleteCompetition(internalOrgId, competitionId);

        return RedirectToAction(
            nameof(OrderDescriptionController.NewOrderDescription),
            typeof(OrderDescriptionController).ControllerName(),
            new
            {
                Area = typeof(OrderDescriptionController).AreaName(),
                internalOrgId = internalOrgId,
                option = OrderTriageValue.Under40K,
                orderType = CatalogueItemType.Solution,
            });
    }

    private async Task<IActionResult> HandleDeleteOrder(string internalOrgId, int competitionId)
    {
        await competitionsService.DeleteCompetition(internalOrgId, competitionId);

        return RedirectToAction(nameof(CompetitionsDashboardController.Index), typeof(CompetitionsDashboardController).ControllerName(), new { internalOrgId = internalOrgId });
    }
}
