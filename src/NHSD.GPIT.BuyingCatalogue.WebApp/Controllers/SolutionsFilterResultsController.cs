using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Shorlists;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    [Route("solutions-filter-results/{internalOrgId}/{filterId}")]
    [RestrictToLocalhostActionFilter]
    public class SolutionsFilterResultsController : Controller
    {
        private readonly ISolutionsFilterService solutionsFilterService;
        private readonly IFrameworkService frameworkService;
        private readonly IManageFiltersService filterService;
        private readonly IOrganisationsService organisationsService;
        private readonly IManageFiltersService manageFiltersService;

        public SolutionsFilterResultsController(
            ISolutionsFilterService solutionsFilterService,
            IFrameworkService frameworkService,
            IManageFiltersService filterService,
            IOrganisationsService organisationsService,
            IManageFiltersService manageFiltersService)
        {
            this.solutionsFilterService = solutionsFilterService ?? throw new ArgumentNullException(nameof(solutionsFilterService));
            this.frameworkService = frameworkService ?? throw new ArgumentNullException(nameof(frameworkService));
            this.filterService = filterService ?? throw new ArgumentNullException(nameof(filterService));
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            this.manageFiltersService = manageFiltersService ?? throw new ArgumentNullException(nameof(manageFiltersService));
        }

        public async Task<IActionResult> Index(
            string internalOrgId,
            int filterId)
        {
            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
            var filterDetails = await filterService.GetFilterDetails(organisation.Id, filterId);
            var filterIds = await manageFiltersService.GetFilterIds(organisation.Id, filterId);
            var filterResults = (await solutionsFilterService.GetAllSolutionsFilteredFromFilterIds(filterIds)).ToList();
            var filter = await manageFiltersService.GetFilterDetails(organisation.Id, filterId);

            if (filter == null || filterIds == null)
                return NotFound();

            var selectedFrameworks = filterResults
                .SelectMany(x => x.Solution.FrameworkSolutions)
                .GroupBy(x => (x.Framework.ShortName, x.Framework.Id, x.Framework.IsExpired));

            if (!string.IsNullOrWhiteSpace(filterIds.FrameworkId))
                selectedFrameworks = selectedFrameworks.Where(x => string.Equals(x.Key.Id, filterIds.FrameworkId));

            var resultsForFrameworks = selectedFrameworks.Select(
                x => new ResultsForFrameworkModel(
                    internalOrgId,
                    filterDetails.Id,
                    x.Key.Id,
                    x.Key.ShortName,
                    x.Key.IsExpired,
                    x.Select(y => y.Solution.CatalogueItem).ToList(),
                    false))
            .ToList();

            var model = new SolutionsFilterResultsModel()
            {
                ResultsCount = filterResults.Count,
                ResultsForFramework = resultsForFrameworks,
                ReviewFilter = new ReviewFilterModel(filter, filterIds),
            };

            return View(model);
        }
    }
}
