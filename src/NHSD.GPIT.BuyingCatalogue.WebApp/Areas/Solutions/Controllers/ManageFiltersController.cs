using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Services.ServiceHelpers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.ManageFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers
{
    [Authorize(Policy = "Buyer")]
    [Area("Solutions")]
    [Route("manage-filters")]
    public class ManageFiltersController : Controller
    {
        private const int MaxNumberOfFilters = 10;

        private readonly IOrganisationsService organisationsService;
        private readonly IManageFiltersService manageFiltersService;
        private readonly ICapabilitiesService capabilitiesService;
        private readonly IEpicsService epicsService;
        private readonly IFrameworkService frameworkService;

        public ManageFiltersController(
            IOrganisationsService organisationsService,
            ICapabilitiesService capabilitiesService,
            IEpicsService epicsService,
            IFrameworkService frameworkService,
            IManageFiltersService manageFiltersService)
        {
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            this.manageFiltersService = manageFiltersService ?? throw new ArgumentNullException(nameof(manageFiltersService));
            this.capabilitiesService = capabilitiesService ?? throw new ArgumentNullException(nameof(capabilitiesService));
            this.epicsService = epicsService ?? throw new ArgumentNullException(nameof(epicsService));
            this.frameworkService = frameworkService ?? throw new ArgumentNullException(nameof(frameworkService));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var organisation = await GetUserOrganisation();
            var existingFilters = await manageFiltersService.GetFilters(organisation.Id);
            var model = new ManageFiltersModel(existingFilters, organisation.Name);
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
                    model.SelectedCapabilityIds,
                    model.SelectedEpicIds,
                    model.SelectedFrameworkId,
                    selectedClientApplicationTypeIds = model.CombineSelectedOptions(model.ClientApplicationTypeOptions),
                    selectedHostingTypeIds = model.CombineSelectedOptions(model.HostingTypeOptions),
                });
        }

        [HttpGet("save-filter-confirm")]
        public async Task<IActionResult> ConfirmSaveFilter(
            string selectedCapabilityIds,
            string selectedEpicIds,
            string selectedFrameworkId,
            string selectedClientApplicationTypeIds,
            string selectedHostingTypeIds)
        {
            var backLink =
                Url.Action(nameof(SolutionsController.Index), typeof(SolutionsController).ControllerName(), new
                {
                    selectedCapabilityIds,
                    selectedEpicIds,
                    selectedFrameworkId,
                    selectedClientApplicationTypeIds,
                    selectedHostingTypeIds,
                });

            var organisationId = (await GetUserOrganisation()).Id;
            var existingFilters = await manageFiltersService.GetFilters(organisationId);
            if (existingFilters.Count >= MaxNumberOfFilters)
                return RedirectToAction(nameof(CannotSaveFilter), typeof(ManageFiltersController).ControllerName(), new { backLink });

            var capabilities = await capabilitiesService.GetCapabilitiesByIds(SolutionsFilterHelper.ParseCapabilityIds(selectedCapabilityIds));

            var epics = await epicsService.GetEpicsByIds(
                    SolutionsFilterHelper.ParseEpicIds(selectedEpicIds));

            var framework = await frameworkService.GetFramework(selectedFrameworkId);

            var clientApplicationTypes = SolutionsFilterHelper.ParseClientApplicationTypeIds(selectedClientApplicationTypeIds)?.ToList();
            var hostingTypes = SolutionsFilterHelper.ParseHostingTypeIds(selectedHostingTypeIds)?.ToList();

            var model = new SaveFilterModel(capabilities, epics, framework, clientApplicationTypes, hostingTypes, organisationId)
                {
                    BackLink = backLink,
                };
            return View(model);
        }

        [HttpPost("save-filter-confirm")]
        public async Task<IActionResult> ConfirmSaveFilter(
            SaveFilterModel model)
        {
            if (!ModelState.IsValid)
            {
                var capabilities = await capabilitiesService.GetCapabilitiesByIds(model.CapabilityIds ?? new List<int>());
                var epics = await epicsService.GetEpicsByIds(model.EpicIds ?? new List<string>());
                model.SetGroupedCapabilities(capabilities, epics);
                return View(model);
            }

            await manageFiltersService.AddFilter(
                model.Name,
                model.Description,
                model.OrganisationId,
                model.CapabilityIds,
                model.EpicIds,
                model.FrameworkId,
                model.ClientApplicationTypes,
                model.HostingTypes);

            return RedirectToAction(
                nameof(Index),
                typeof(ManageFiltersController).ControllerName());
        }

        [HttpGet("filter-details")]
        public async Task<IActionResult> FilterDetails(int filterId)
        {
            var organisation = await GetUserOrganisation();
            var filter = await manageFiltersService.GetFilterDetails(organisation.Id, filterId);

            if (filter == null)
                return NotFound();

            var model = new ReviewFilterModel(filter)
            {
                BackLink = Url.Action(nameof(Index), typeof(ManageFiltersController).ControllerName()),
                Caption = organisation.Name,
            };

            return View(model);
        }

        [HttpGet("save-failed")]
        public IActionResult CannotSaveFilter(string backLink)
        {
            var model = new NavBaseModel()
            {
                BackLink = backLink,
            };
            return View(model);
        }

        [HttpGet("delete")]
        public async Task<IActionResult> DeleteFilter(int filterId)
        {
            var filter = await manageFiltersService.GetFilter(filterId);
            var model = new DeleteFilterModel()
            {
                BackLink = Url.Action(nameof(FilterDetails), typeof(ManageFiltersController).ControllerName(), new { filterId }),
                FilterId = filter.Id,
                FilterName = filter.Name,
            };
            return View(model);
        }

        [HttpPost("delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFilterConfirmed(int filterId)
        {
            var organisationId = (await GetUserOrganisation()).Id;
            var filter = await manageFiltersService.GetFilter(filterId);
            if (filter.OrganisationId == organisationId)
            {
                await manageFiltersService.DeleteFilter(filterId);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<Organisation> GetUserOrganisation()
        {
            var organisationInternalIdentifier = User.GetPrimaryOrganisationInternalIdentifier();
            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(organisationInternalIdentifier);
            return organisation;
        }
    }
}
