using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;

[Authorize("Development")]
[Authorize("Buyer")]
[Area("Competitions")]
[Route("organisation/{internalOrgId}/competitions")]
public class CompetitionsDashboardController : Controller
{
    private readonly IOrganisationsService organisationsService;
    private readonly ICompetitionsService competitionsService;
    private readonly IManageFiltersService filterService;
    private readonly ISolutionsFilterService solutionsFilterService;

    public CompetitionsDashboardController(
        IOrganisationsService organisationsService,
        ICompetitionsService competitionsService,
        IManageFiltersService filterService,
        ISolutionsFilterService solutionsFilterService)
    {
        this.organisationsService =
            organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
        this.competitionsService = competitionsService ?? throw new ArgumentNullException(nameof(competitionsService));
        this.filterService = filterService ?? throw new ArgumentNullException(nameof(filterService));
        this.solutionsFilterService =
            solutionsFilterService ?? throw new ArgumentNullException(nameof(solutionsFilterService));
    }

    [HttpGet]
    public async Task<IActionResult> Index(string internalOrgId)
    {
        var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);

        var competitions = await competitionsService.GetCompetitions(organisation.Id);

        var model = new CompetitionDashboardModel(internalOrgId, organisation.Name, competitions.ToList());

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

    [HttpGet("select-filter/{filterId}/review")]
    public async Task<IActionResult> ReviewFilter(string internalOrgId, int filterId)
    {
        var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
        var filterDetails = await filterService.GetFilterDetails(organisation.Id, filterId);

        if (filterDetails == null)
            return RedirectToAction(nameof(SelectFilter), new { internalOrgId });

        var model = new ReviewFilterModel(filterDetails)
        {
            BackLink = Url.Action(nameof(SelectFilter), new { internalOrgId }),
            Caption = filterDetails.Name,
        };

        return View(model);
    }

    [HttpPost("select-filter/{filterId}/review")]
    public IActionResult ReviewFilter(string internalOrgId, int filterId, ReviewFilterModel model)
    {
        _ = model;

        return RedirectToAction(nameof(SaveCompetition), new { internalOrgId, filterId });
    }

    [HttpGet("select-filter/{filterId}/save")]
    public async Task<IActionResult> SaveCompetition(string internalOrgId, int filterId)
    {
        _ = filterId;
        var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);

        var model = new SaveCompetitionModel(organisation.Id, organisation.Name)
        {
            BackLink = Url.Action(nameof(ReviewFilter), new { internalOrgId, filterId }),
        };

        return View(model);
    }

    [HttpPost("select-filter/{filterId}/save")]
    public async Task<IActionResult> SaveCompetition(string internalOrgId, int filterId, SaveCompetitionModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);

        var competitionId = await competitionsService.AddCompetition(
            organisation.Id,
            filterId,
            model.Name,
            model.Description);

        await AssignCompetitionSolutions(organisation.Id, competitionId, filterId);

        return RedirectToAction(nameof(SelectSolutions), new { internalOrgId, competitionId });
    }

    [HttpGet("{competitionId:int}/select-solutions")]
    public async Task<IActionResult> SelectSolutions(string internalOrgId, int competitionId)
    {
        var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
        var competition = await competitionsService.GetCompetition(organisation.Id, competitionId);

        var model = new SelectSolutionsModel(competition.Name, competition.CompetitionSolutions)
        {
            BackLinkText = "Go back to manage competitions",
            BackLink = Url.Action(nameof(Index), new { internalOrgId }),
        };

        return View(model);
    }

    [HttpPost("{competitionId:int}/select-solutions")]
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

        return RedirectToAction(nameof(Index), new { internalOrgId });
    }

    private async Task<IActionResult> HandleDirectAward(Organisation organisation, int competitionId)
    {
        await competitionsService.CompleteCompetition(organisation.Id, competitionId);

        return RedirectToAction(
            nameof(OrderController.NewOrder),
            typeof(OrderController).ControllerName(),
            new
            {
                Area = typeof(OrderController).AreaName(),
                internalOrgId = organisation.InternalIdentifier,
                option = OrderTriageValue.Under40K,
                orderType = CatalogueItemType.Solution,
            });
    }

    private async Task<IActionResult> HandleDeleteOrder(Organisation organisation, int competitionId)
    {
        await competitionsService.DeleteCompetition(organisation.Id, competitionId);

        return RedirectToAction(nameof(Index), new { internalOrgId = organisation.InternalIdentifier });
    }

    private async Task AssignCompetitionSolutions(int organisationId, int competitionId, int filterId)
    {
        var competition = await competitionsService.GetCompetition(organisationId, competitionId);
        var filter = await filterService.GetFilter(organisationId, filterId);

        var pageOptions = new PageOptions { PageSize = 100 };
        var solutionsAndServices =
            await solutionsFilterService.GetAllSolutionsFiltered(
                pageOptions,
                selectedCapabilityIds: string.Join(
                    FilterConstants.Delimiter,
                    filter.FilterCapabilities.Select(x => x.CapabilityId)),
                selectedEpicIds: string.Join(FilterConstants.Delimiter, filter.FilterEpics.Select(x => x.EpicId)),
                selectedFrameworkId: filter.FrameworkId,
                selectedClientApplicationTypeIds: string.Join(
                    FilterConstants.Delimiter,
                    filter.FilterClientApplicationTypes.Select(x => (int)x.ClientApplicationType)),
                selectedHostingTypeIds: string.Join(
                    FilterConstants.Delimiter,
                    filter.FilterHostingTypes.Select(x => (int)x.HostingType)));

        var competitionSolutions = solutionsAndServices.CatalogueItems.Select(
            x => new CompetitionSolution(competition.Id, x.Solution.CatalogueItemId)
            {
                RequiredServices = x.Solution.AdditionalServices.Select(
                        y => new RequiredService(competition.Id, x.Solution.CatalogueItemId, y.CatalogueItemId))
                    .ToList(),
            });

        await competitionsService.AddCompetitionSolutions(organisationId, competitionId, competitionSolutions);
    }
}
