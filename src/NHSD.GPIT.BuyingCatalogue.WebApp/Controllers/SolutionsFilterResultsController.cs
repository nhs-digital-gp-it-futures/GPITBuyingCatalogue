using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    [Route("solutions-filter-results/{internalOrgId}/{filterId}")]
    [RestrictToLocalhostActionFilter]
    public class SolutionsFilterResultsController : Controller
    {
        private readonly ISolutionsFilterService solutionsFilterService;
        private readonly IFrameworkService frameworkService;
        private readonly IOrganisationsService organisationsService;
        private readonly IManageFiltersService manageFiltersService;

        public SolutionsFilterResultsController(
            ISolutionsFilterService solutionsFilterService,
            IFrameworkService frameworkService,
            IOrganisationsService organisationsService,
            IManageFiltersService manageFiltersService)
        {
            this.solutionsFilterService = solutionsFilterService ?? throw new ArgumentNullException(nameof(solutionsFilterService));
            this.frameworkService = frameworkService ?? throw new ArgumentNullException(nameof(frameworkService));
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            this.manageFiltersService = manageFiltersService ?? throw new ArgumentNullException(nameof(manageFiltersService));
        }

        public async Task<IActionResult> Index(
            string internalOrgId,
            int filterId)
        {
            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
            var filter = await manageFiltersService.GetFilterDetails(organisation.Id, filterId);
            var filterIds = await manageFiltersService.GetFilterIds(organisation.Id, filterId);

            if (filter == null || filterIds == null)
                return NotFound();

            var selectedFrameworkId = filterIds.FrameworkId;
            var selectedApplicationTypeIds = filterIds.ApplicationTypeIds.ToFilterString();
            var selectedHostingTypeIds = filterIds.HostingTypeIds.ToFilterString();

            var (catalogueItems, _, capabilitiesAndCount) =
                await solutionsFilterService.GetAllSolutionsFiltered(
                    null,
                    filterIds.CapabilityAndEpicIds,
                    null,
                    selectedFrameworkId,
                    selectedApplicationTypeIds,
                    selectedHostingTypeIds);

            var model = new SolutionsFilterResultsModel()
            {
                Title = new PageTitleModel()
                {
                    Title = "Catalogue Solutions found",
                    Caption = organisation.Name,
                },
                CatalogueItems = catalogueItems,
                ReviewFilter = new ReviewFilterModel(filter, filterIds),
                CapabilitiesAndEpics = capabilitiesAndCount,
            };

            return View(model);
        }
    }
}
