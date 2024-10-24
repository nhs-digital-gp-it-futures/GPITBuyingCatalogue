﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Filters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SelectSolutionsModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;

[Authorize("Buyer")]
[Area("Competitions")]
[Route("organisation/{internalOrgId}/competitions/{competitionId:int}")]
[CompetitionSolutionSelectionFilter]
public class CompetitionSelectSolutionsController : Controller
{
    private readonly PublicationStatus[] allowedPublicationStatuses = [PublicationStatus.Published, PublicationStatus.InRemediation];
    private readonly ICompetitionsService competitionsService;
    private readonly ICompetitionOrderService competitionOrderService;
    private readonly IFrameworkService frameworkService;
    private readonly IManageFiltersService filtersService;

    public CompetitionSelectSolutionsController(
        ICompetitionsService competitionsService,
        ICompetitionOrderService competitionOrderService,
        IFrameworkService frameworkService,
        IManageFiltersService filtersService)
    {
        this.competitionsService = competitionsService ?? throw new ArgumentNullException(nameof(competitionsService));
        this.competitionOrderService = competitionOrderService ?? throw new ArgumentNullException(nameof(competitionOrderService));
        this.frameworkService = frameworkService ?? throw new ArgumentNullException(nameof(frameworkService));
        this.filtersService = filtersService ?? throw new ArgumentNullException(nameof(filtersService));
    }

    [HttpGet("select-solutions")]
    public async Task<IActionResult> SelectSolutions(string internalOrgId, int competitionId)
    {
        var competition = await competitionsService.GetCompetitionWithServicesAndFramework(internalOrgId, competitionId);

        if (competition == null)
        {
            return RedirectToAction(
                nameof(CompetitionsDashboardController.Index),
                typeof(CompetitionsDashboardController).ControllerName(),
                new { internalOrgId });
        }

        var framework = await frameworkService.GetFramework(competition.FrameworkId);
        var availableSolutions = framework.IsExpired
            ? []
            : competition.CompetitionSolutions.Where(x =>
                allowedPublicationStatuses.Contains(x.Solution.CatalogueItem.PublishedStatus)
                && x.Solution.FrameworkSolutions.Any(y => y.FrameworkId == competition.FrameworkId));
        var filterDetails = await filtersService.GetFilterDetails(competition.OrganisationId, competition.FilterId);

        var model = new SelectSolutionsModel(competition.Name, availableSolutions, framework.ShortName, filterDetails)
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
        {
            var competition = await competitionsService.GetCompetitionWithServicesAndFramework(internalOrgId, competitionId);
            var filterDetails = await filtersService.GetFilterDetails(competition.OrganisationId, competition.FilterId);
            model.ReviewFilter = new ReviewFilterModel(filterDetails) { InExpander = true };
            return View(model);
        }

        if (model.HasSingleSolution())
        {
            return model.IsDirectAward.GetValueOrDefault()
                ? await HandleDirectAward(internalOrgId, competitionId, model.Solutions.First().SolutionId)
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

        var availableSolutions = competition.CompetitionSolutions
            .Where(x => allowedPublicationStatuses.Contains(x.Solution.CatalogueItem.PublishedStatus));

        if (availableSolutions.All(x => x.IsShortlisted))
            return RedirectToAction(nameof(ConfirmSolutions), new { internalOrgId, competitionId });

        var nonShortlistedSolutions = availableSolutions.Where(x => !x.IsShortlisted);

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
            CompetitionId = competitionId,
            InternalOrgId = internalOrgId,
        };

        return View(model);
    }

    [HttpPost("confirm-solutions")]
    public async Task<IActionResult> ConfirmSolutions(string internalOrgId, int competitionId, ConfirmSolutionsModel model)
    {
        if (!ModelState.IsValid)
        {
            var competition = await competitionsService.GetCompetitionWithServices(internalOrgId, competitionId);
            model.CompetitionSolutions = competition.CompetitionSolutions.ToList();
            return View(model);
        }

        await competitionsService.AcceptShortlist(internalOrgId, competitionId);

        return RedirectToAction(nameof(CompetitionTaskListController.Index), typeof(CompetitionTaskListController).ControllerName(), new { internalOrgId, competitionId });
    }

    private async Task<IActionResult> HandleDirectAward(string internalOrgId, int competitionId, CatalogueItemId solutionId)
    {
        await competitionsService.CompleteCompetition(internalOrgId, competitionId, true);
        var callOffId = await competitionOrderService.CreateDirectAwardOrder(internalOrgId, competitionId, solutionId);

        return RedirectToAction(
            nameof(OrderController.Order),
            typeof(OrderController).ControllerName(),
            new { internalOrgId, callOffId, area = typeof(OrderController).AreaName() });
    }

    private async Task<IActionResult> HandleDeleteOrder(string internalOrgId, int competitionId)
    {
        await competitionsService.DeleteCompetition(internalOrgId, competitionId);

        return RedirectToAction(nameof(CompetitionsDashboardController.Index), typeof(CompetitionsDashboardController).ControllerName(), new { internalOrgId = internalOrgId });
    }
}
