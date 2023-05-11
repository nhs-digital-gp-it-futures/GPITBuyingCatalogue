using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.Services.Framework;
using NHSD.GPIT.BuyingCatalogue.Services.ServiceHelpers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.ManageFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers
{
    [Authorize(Policy = "Buyer")]
    [Area("Solutions")]
    [Route("manage-filters")]
    public class ManageFiltersController : Controller
    {
        private readonly IOrganisationsService organisationsService;
        private readonly IManageFiltersService manageFiltersService;
        private readonly ICapabilitiesService capabilitiesService;
        private readonly IEpicsService epicsService;
        private readonly IFrameworkService frameworkService;

        private const int MaxNumberOfFilters = 10;

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
            //TODO Currently using to Display filters. Will become manage filters page
            var organisationId = await GetUserOrganisationId();
            var existingFilters = await manageFiltersService.GetFilters(organisationId);
            return View(existingFilters);
        }

        [AllowAnonymous]
        [HttpPost("save-filter")]
        public IActionResult SaveFilter(
            AdditionalFiltersModel model)
        {
            return RedirectToAction(
                nameof(ManageFiltersController.ConfirmSaveFilter),
                typeof(ManageFiltersController).ControllerName(),
                new
                {
                    model.SelectedCapabilityIds,
                    model.SelectedEpicIds,
                    model.SelectedFrameworkId,
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
            var organisationId = await GetUserOrganisationId();
            var existingFilters = await manageFiltersService.GetFilters(organisationId);

            if (existingFilters.Count >= MaxNumberOfFilters)
                return RedirectToAction(
                    nameof(CannotSaveFilter),
                    typeof(ManageFiltersController).ControllerName());

            var capabilities = await
                capabilitiesService.GetCapabilitiesByIds(
                    SolutionsFilterHelper.ParseCapabilityIds(selectedCapabilityIds));

            var epics = await epicsService.GetEpicsByIds(
                    SolutionsFilterHelper.ParseEpicIds(selectedEpicIds));

            var framework = await frameworkService.GetFramework(selectedFrameworkId);

            var clientApplicationTypes = SolutionsFilterHelper.ParseClientApplicationTypeIds(selectedClientApplicationTypeIds)?.ToList();
            var hostingTypes = SolutionsFilterHelper.ParseHostingTypeIds(selectedHostingTypeIds)?.ToList();

            var model = new SaveFilterModel(capabilities, epics, framework, clientApplicationTypes, hostingTypes, organisationId) 
                { 
                    BackLink = "/", 
                    BackLinkText = "Go back",
                };
            return View(model);
        }

        [HttpPost("save-filter-confirm")]
        public async Task<IActionResult> ConfirmSaveFilter(
            SaveFilterModel model)
        {
            if (!ModelState.IsValid)
            {
                var capabilities = await capabilitiesService.GetCapabilitiesByIds(model.CapabilityIds);
                var epics = await epicsService.GetEpicsByIds(model.EpicIds);
                model.SetGroupedCapabilities(capabilities, epics);
                return View(model);
            }

            await manageFiltersService.SaveFilter(
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

        [HttpGet("save-failed")]
        public IActionResult CannotSaveFilter()
        {
            var model = new NavBaseModel()
            {
                BackLink = "/",
                BackLinkText = "Go back",
            };
            return View(model);
        }

        private async Task<int> GetUserOrganisationId()
        {
            var organisationInternalIdentifier = User.GetPrimaryOrganisationInternalIdentifier();
            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(organisationInternalIdentifier);
            return organisation.Id;
        }
    }
}
