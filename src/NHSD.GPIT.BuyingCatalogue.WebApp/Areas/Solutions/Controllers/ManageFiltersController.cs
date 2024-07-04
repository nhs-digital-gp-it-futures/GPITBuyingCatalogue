using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Integrations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Services.ServiceHelpers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.ManageFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers
{
    [Authorize(Policy = "Buyer")]
    [Area("Solutions")]
    [Route("manage-filters")]
    public class ManageFiltersController(
        IOrganisationsService organisationsService,
        ICapabilitiesService capabilitiesService,
        IFrameworkService frameworkService,
        IManageFiltersService manageFiltersService,
        ISolutionsFilterService solutionsFilterService,
        IPdfService pdfService,
        IIntegrationsService integrationsService)
        : Controller
    {
        private const int MaxNumberOfFilters = 10;

        private readonly IOrganisationsService organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
        private readonly IManageFiltersService manageFiltersService = manageFiltersService ?? throw new ArgumentNullException(nameof(manageFiltersService));
        private readonly ICapabilitiesService capabilitiesService = capabilitiesService ?? throw new ArgumentNullException(nameof(capabilitiesService));
        private readonly ISolutionsFilterService solutionsFilterService = solutionsFilterService ?? throw new ArgumentNullException(nameof(solutionsFilterService));
        private readonly IFrameworkService frameworkService = frameworkService ?? throw new ArgumentNullException(nameof(frameworkService));
        private readonly IPdfService pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
        private readonly IIntegrationsService integrationsService = integrationsService ?? throw new ArgumentNullException(nameof(integrationsService));

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var organisation = await GetUserOrganisation();
            var existingFilters = await manageFiltersService.GetFilters(organisation.Id);
            var model = new ManageFiltersModel(organisation.InternalIdentifier, existingFilters, organisation.Name);
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost("save-filter")]
        public IActionResult SaveFilter(
            AdditionalFiltersModel model)
        {
            return RedirectToAction(
                nameof(ConfirmSaveFilter),
                typeof(ManageFiltersController).ControllerName(),
                new
                {
                    model.Selected,
                    model.SelectedFrameworkId,
                    selectedApplicationTypeIds = model.CombineSelectedOptions(model.ApplicationTypeOptions),
                    selectedHostingTypeIds = model.CombineSelectedOptions(model.HostingTypeOptions),
                    selectedIntegrations = model.GetIntegrationIds(),
                });
        }

        [HttpGet("save-filter-confirm")]
        public async Task<IActionResult> ConfirmSaveFilter(
            string selected,
            string selectedFrameworkId,
            string selectedApplicationTypeIds,
            string selectedHostingTypeIds,
            string selectedIntegrations)
        {
            var backLink =
                Url.Action(nameof(SolutionsController.Index), typeof(SolutionsController).ControllerName(), new
                {
                    selected,
                    selectedFrameworkId,
                    selectedApplicationTypeIds,
                    selectedHostingTypeIds,
                    selectedIntegrations,
                });

            var organisationId = (await GetUserOrganisation()).Id;
            var existingFilters = await manageFiltersService.GetFilters(organisationId);
            if (existingFilters.Count >= MaxNumberOfFilters)
                return RedirectToAction(nameof(CannotSaveFilter), typeof(ManageFiltersController).ControllerName(), new { backLink });

            var capabilityAndEpicIds = SolutionsFilterHelper.ParseCapabilityAndEpicIds(selected);
            var capabilitiesAndEpics = await capabilitiesService.GetGroupedCapabilitiesAndEpics(capabilityAndEpicIds);

            var integrationsAndTypeIds = SolutionsFilterHelper.ParseIntegrationAndTypeIds(selectedIntegrations);
            var integrationsAndTypes = await integrationsService.GetIntegrationAndTypeNames(integrationsAndTypeIds);

            var framework = await frameworkService.GetFramework(selectedFrameworkId);

            var applicationTypes = SolutionsFilterHelper.ParseEnumFilter<ApplicationType>(selectedApplicationTypeIds)?.ToList();
            var hostingTypes = SolutionsFilterHelper.ParseEnumFilter<HostingType>(selectedHostingTypeIds)?.ToList();

            var model = new SaveFilterModel(capabilitiesAndEpics, framework, applicationTypes, hostingTypes, integrationsAndTypes, organisationId)
            {
                BackLink = backLink,
            };

            return View(model);
        }

        [HttpPost("save-filter-confirm")]
        public async Task<IActionResult> ConfirmSaveFilter(
            SaveFilterModel model,
            string selected,
            string selectedIntegrations)
        {
            var capabilityAndEpicIds = SolutionsFilterHelper.ParseCapabilityAndEpicIds(selected);
            var integrationIds = SolutionsFilterHelper.ParseIntegrationAndTypeIds(selectedIntegrations);

            if (!ModelState.IsValid)
            {
                var capabilitiesAndEpics = await capabilitiesService.GetGroupedCapabilitiesAndEpics(capabilityAndEpicIds);
                var integrationAndTypeNames = await integrationsService.GetIntegrationAndTypeNames(integrationIds);

                model.GroupedCapabilities = capabilitiesAndEpics;
                model.Integrations = integrationAndTypeNames;

                return View(model);
            }

            var filterId = await manageFiltersService.AddFilter(
                model.Name,
                model.Description,
                model.OrganisationId,
                capabilityAndEpicIds,
                model.FrameworkId,
                model.ApplicationTypes,
                model.HostingTypes,
                integrationIds);

            return RedirectToAction(
                nameof(FilterDetails),
                new { filterId });
        }

        [HttpGet("filter-details")]
        public async Task<IActionResult> FilterDetails(int filterId)
        {
            var organisation = await GetUserOrganisation();
            var filterDetails = await manageFiltersService.GetFilterDetails(organisation.Id, filterId);
            var filterIds = await manageFiltersService.GetFilterIds(organisation.Id, filterId);
            var solutions = await solutionsFilterService.GetAllSolutionsFilteredFromFilterIds(filterIds);

            if (filterDetails == null || filterIds == null)
                return NotFound();

            var model = new ReviewFilterModel(filterDetails, organisation.InternalIdentifier, solutions.ToList(), false, filterIds)
            {
                BackLink = Url.Action(nameof(Index), typeof(ManageFiltersController).ControllerName()),
                Caption = organisation.Name,
                OrganisationName = organisation.Name,
                InExpander = true,
            };

            return View("Shortlists/FilterDetails", model);
        }

        [HttpGet("save-failed")]
        public IActionResult CannotSaveFilter(string backLink)
        {
            var model = new NavBaseModel
            {
                BackLink = backLink,
            };
            return View(model);
        }

        [HttpGet("delete")]
        public async Task<IActionResult> DeleteFilter(int filterId)
        {
            var oranisation = await GetUserOrganisation();
            var filter = await manageFiltersService.GetFilterDetails(oranisation.Id, filterId);
            if (filter == null)
            {
                return RedirectToAction(
                   nameof(Index),
                   typeof(ManageFiltersController).ControllerName());
            }

            var model = new DeleteFilterModel(filter.Id, filter.Name)
            {
                BackLink = Url.Action(nameof(FilterDetails), new { filterId }),
            };
            return View(model);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteFilter(DeleteFilterModel deleteFilterModel)
        {
            int organisationId = (await GetUserOrganisation()).Id;
            var filter = await manageFiltersService.GetFilterDetails(organisationId, deleteFilterModel.FilterId);
            if (filter != null)
            {
                await manageFiltersService.DeleteFilter(deleteFilterModel.FilterId);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("download-results")]
        public async Task<IActionResult> DownloadResults(int filterId)
        {
            var organisation = await GetUserOrganisation();
            var filter = await manageFiltersService.GetFilterDetails(organisation.Id, filterId);

            if (filter == null)
                return NotFound();

            var uri = Url.Action(
                    nameof(SolutionsFilterResultsController.Index),
                    typeof(SolutionsFilterResultsController).ControllerName(),
                    new
                    {
                        internalOrgId = organisation.InternalIdentifier,
                        filterId = filter.Id,
                    });

            var result = await pdfService.Convert(new(pdfService.BaseUri(), uri));

            var fileName = $"{filter.Name} Catalogue Solutions.pdf";
            return File(result, "application/pdf", fileName);
        }

        [HttpGet("maximum-shortlists")]
        public async Task<IActionResult> MaximumShortlists()
        {
            var organisation = await GetUserOrganisation();
            var model = new MaximumShortlistsModel(organisation.Name)
            {
                BackLink = Url.Action(nameof(Index), typeof(ManageFiltersController).ControllerName()),
            };
            return View(model);
        }

        private async Task<Organisation> GetUserOrganisation()
        {
            var organisationInternalIdentifier = User.GetPrimaryOrganisationInternalIdentifier();
            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(organisationInternalIdentifier);
            return organisation;
        }
    }
}
