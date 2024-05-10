using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Services.Framework;
using NHSD.GPIT.BuyingCatalogue.Services.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;

[Authorize("Buyer")]
[Area("Competitions")]
[Route("organisation/{internalOrgId}/competitions")]
public class CompetitionsDashboardController : Controller
{
    private readonly IOrganisationsService organisationsService;
    private readonly ICompetitionsService competitionsService;
    private readonly IManageFiltersService filterService;
    private readonly IManageFiltersService manageFiltersService;
    private readonly IFrameworkService frameworkService;
    private readonly ISolutionsFilterService solutionsFilterService;

    public CompetitionsDashboardController(
        IOrganisationsService organisationsService,
        ICompetitionsService competitionsService,
        IManageFiltersService filterService,
        IManageFiltersService manageFiltersService,
        ISolutionsFilterService solutionsFilterService,
        IFrameworkService frameworkService)
    {
        this.organisationsService =
            organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
        this.competitionsService = competitionsService ?? throw new ArgumentNullException(nameof(competitionsService));
        this.filterService = filterService ?? throw new ArgumentNullException(nameof(filterService));
        this.manageFiltersService = manageFiltersService ?? throw new ArgumentNullException(nameof(manageFiltersService));
        this.solutionsFilterService =
            solutionsFilterService ?? throw new ArgumentNullException(nameof(solutionsFilterService));
        this.frameworkService = frameworkService ?? throw new ArgumentNullException(nameof(frameworkService));
    }

    [HttpGet]
    public async Task<IActionResult> Index(string internalOrgId, [FromQuery] string page = "")
    {
        var options = new PageOptions(page, pageSize: 10);
        var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);

        var competitions = await competitionsService.GetPagedCompetitions(internalOrgId, options);

        var model = new CompetitionDashboardModel(internalOrgId, organisation.Name, competitions.Items)
        {
            Options = competitions.Options,
        };

        return View(model);
    }

    [HttpGet("before-you-start")]
    public IActionResult BeforeYouStart(string internalOrgId)
    {
        var model = new NavBaseModel { BackLink = Url.Action(nameof(Index), new { internalOrgId }) };

        return View(model);
    }

    [HttpPost("before-you-start")]
    public IActionResult BeforeYouStart(string internalOrgId, NavBaseModel model)
    {
        _ = model;

        return RedirectToAction(nameof(SelectFilter), new { internalOrgId });
    }

    [HttpGet("select-filter")]
    public async Task<IActionResult> SelectFilter(string internalOrgId)
    {
        var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
        var filters = await filterService.GetFilters(organisation.Id);

        var model = new SelectFilterModel(organisation.Name, filters)
        {
            BackLink = Url.Action(nameof(BeforeYouStart), new { internalOrgId }),
        };

        return View(model);
    }

    [HttpPost("select-filter")]
    public async Task<IActionResult> SelectFilter(string internalOrgId, SelectFilterModel model)
    {
        if (ModelState.IsValid)
            return RedirectToAction(nameof(ReviewFilter), new { internalOrgId, filterId = model.SelectedFilterId });

        var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
        var filters = await filterService.GetFilters(organisation.Id);

        model.WithFilters(filters);
        return View(model);
    }

    [HttpGet("select-filter/{filterId:int}/review")]
    public async Task<IActionResult> ReviewFilter(string internalOrgId, int filterId)
    {
        var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
        var filterDetails = await filterService.GetFilterDetails(organisation.Id, filterId);
        var filterIds = await manageFiltersService.GetFilterIds(organisation.Id, filterId);
        var solutions = await solutionsFilterService.GetAllSolutionsFilteredFromFilterIds(filterIds);

        if (filterDetails == null)
            return RedirectToAction(nameof(SelectFilter), new { internalOrgId });

        var model = new ReviewFilterModel(filterDetails, organisation.InternalIdentifier, solutions.ToList(), true, filterIds)
        {
            BackLink = Url.Action(nameof(Index), typeof(ManageFiltersController).ControllerName()),
            Caption = organisation.Name,
            OrganisationName = organisation.Name,
            InExpander = true,
        };

        return View("Shortlists/FilterDetails", model);
    }

    [HttpPost("select-filter/{filterId:int}/review")]
    public IActionResult ReviewFilter(string internalOrgId, int filterId, ReviewFilterModel model)
    {
        _ = model;

        return RedirectToAction(nameof(SaveCompetition), new { internalOrgId, filterId });
    }

    [HttpGet("select-filter/{filterId:int}/save")]
    public async Task<IActionResult> SaveCompetition(string internalOrgId, int filterId, string frameworkId = null, bool fromFilter = false)
    {
        _ = filterId;
        var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);

        var backlink = fromFilter == true ?
                Url.Action(
                    nameof(ManageFiltersController.FilterDetails),
                    typeof(ManageFiltersController).ControllerName(),
                    new { filterId, Area = typeof(ManageFiltersController).AreaName() }) :
                Url.Action(nameof(ReviewFilter), new { internalOrgId, filterId });

        if (string.IsNullOrWhiteSpace(frameworkId) || (await frameworkService.GetFramework(frameworkId)) == null)
        {
            return Redirect(backlink);
        }

        var filterIds = await filterService.GetFilterIds(organisation.Id, filterId);
        var results = await solutionsFilterService.GetAllSolutionsFilteredFromFilterIds(filterIds);
        var availableSolutions = results.Where(x => x.Solution.FrameworkSolutions.Any(y => y.FrameworkId == frameworkId));
        if (!availableSolutions.Any())
        {
            return Redirect(backlink);
        }

        var model = new SaveCompetitionModel(internalOrgId, organisation.Name, frameworkId)
        {
            BackLink = backlink,
        };

        return View(model);
    }

    [HttpPost("select-filter/{filterId:int}/save")]
    public async Task<IActionResult> SaveCompetition(string internalOrgId, int filterId, SaveCompetitionModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);

        var competitionId = await competitionsService.AddCompetition(
            organisation.Id,
            filterId,
            model.FrameworkId,
            model.Name,
            model.Description);

        await AssignCompetitionSolutions(organisation.Id, internalOrgId, competitionId, filterId);

        return RedirectToAction(
            nameof(CompetitionSelectSolutionsController.SelectSolutions),
            typeof(CompetitionSelectSolutionsController).ControllerName(),
            new { internalOrgId, competitionId });
    }

    private async Task AssignCompetitionSolutions(int organisationId, string internalOrgId, int competitionId, int filterId)
    {
        var competition = await competitionsService.GetCompetitionWithServices(internalOrgId, competitionId, true);
        var filter = await filterService.GetFilterIds(organisationId, filterId);

        var pageOptions = new PageOptions { PageSize = 100 };
        var (solutionsAndServices, _, _) =
            await solutionsFilterService.GetAllSolutionsFiltered(
                pageOptions,
                capabilitiesAndEpics: filter.CapabilityAndEpicIds,
                selectedFrameworkId: filter.FrameworkId,
                selectedApplicationTypeIds: filter.ApplicationTypeIds.ToFilterString(),
                selectedHostingTypeIds: filter.HostingTypeIds.ToFilterString(),
                selectedIm1Integrations: filter.IM1Integrations.ToFilterString(),
                selectedGpConnectIntegrations: filter.GPConnectIntegrations.ToFilterString(),
                selectedInteroperabilityOptions: filter.InteroperabilityOptions.ToFilterString());

        var competitionSolutions = solutionsAndServices.Select(
            x => new CompetitionSolution(competition.Id, x.Solution.CatalogueItemId)
            {
                SolutionServices = x.Solution.AdditionalServices.Select(
                        y => new SolutionService(competition.Id, x.Solution.CatalogueItemId, y.CatalogueItemId, true))
                    .ToList(),
            });

        await competitionsService.AddCompetitionSolutions(internalOrgId, competition.Id, competitionSolutions);
    }
}
