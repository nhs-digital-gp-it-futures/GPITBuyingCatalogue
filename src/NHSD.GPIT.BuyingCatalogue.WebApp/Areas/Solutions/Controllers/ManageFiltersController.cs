﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
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
    public class ManageFiltersController : Controller
    {
        private const int MaxNumberOfFilters = 10;

        private readonly IOrganisationsService organisationsService;
        private readonly IManageFiltersService manageFiltersService;
        private readonly ICapabilitiesService capabilitiesService;
        private readonly IEpicsService epicsService;
        private readonly IFrameworkService frameworkService;
        private readonly IPdfService pdfService;

        public ManageFiltersController(
            IOrganisationsService organisationsService,
            ICapabilitiesService capabilitiesService,
            IEpicsService epicsService,
            IFrameworkService frameworkService,
            IManageFiltersService manageFiltersService,
            IPdfService pdfService)
        {
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            this.manageFiltersService = manageFiltersService ?? throw new ArgumentNullException(nameof(manageFiltersService));
            this.capabilitiesService = capabilitiesService ?? throw new ArgumentNullException(nameof(capabilitiesService));
            this.epicsService = epicsService ?? throw new ArgumentNullException(nameof(epicsService));
            this.frameworkService = frameworkService ?? throw new ArgumentNullException(nameof(frameworkService));
            this.pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
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
                    model.Selected,
                    model.SelectedFrameworkId,
                    selectedApplicationTypeIds = model.CombineSelectedOptions(model.ApplicationTypeOptions),
                    selectedHostingTypeIds = model.CombineSelectedOptions(model.HostingTypeOptions),
                    selectedIM1IntegrationsIds = model.CombineSelectedOptions(model.IM1IntegrationsOptions),
                    selectedGPConnectIntegrationsIds = model.CombineSelectedOptions(model.GPConnectIntegrationsOptions),
                    selectedInteroperabilityIds = model.CombineSelectedOptions(model.InteroperabilityOptions),
                });
        }

        [HttpGet("save-filter-confirm")]
        public async Task<IActionResult> ConfirmSaveFilter(
            string selected,
            string selectedFrameworkId,
            string selectedApplicationTypeIds,
            string selectedHostingTypeIds,
            string selectedIM1IntegrationsIds,
            string selectedGPConnectIntegrationsIds,
            string selectedInteroperabilityIds)
        {
            var backLink =
                Url.Action(nameof(SolutionsController.Index), typeof(SolutionsController).ControllerName(), new
                {
                    selected,
                    selectedFrameworkId,
                    selectedApplicationTypeIds,
                    selectedHostingTypeIds,
                    selectedIM1IntegrationsIds,
                    selectedGPConnectIntegrationsIds,
                    selectedInteroperabilityIds,
                });

            var organisationId = (await GetUserOrganisation()).Id;
            var existingFilters = await manageFiltersService.GetFilters(organisationId);
            if (existingFilters.Count >= MaxNumberOfFilters)
                return RedirectToAction(nameof(CannotSaveFilter), typeof(ManageFiltersController).ControllerName(), new { backLink });

            var capabilityAndEpicIds = SolutionsFilterHelper.ParseCapabilityAndEpicIds(selected);
            var capabilitiesAndEpics = await capabilitiesService.GetGroupedCapabilitiesAndEpics(capabilityAndEpicIds);

            var framework = await frameworkService.GetFramework(selectedFrameworkId);

            var applicationTypes = SolutionsFilterHelper.ParseApplicationTypeIds(selectedApplicationTypeIds)?.ToList();
            var hostingTypes = SolutionsFilterHelper.ParseHostingTypeIds(selectedHostingTypeIds)?.ToList();
            var iM1IntegrationsTypes = SolutionsFilterHelper.ParseInteropIm1IntegrationsIds(selectedIM1IntegrationsIds)?.ToList();
            var gPConnectIntegrationsTypes = SolutionsFilterHelper.ParseInteropGpConnectIntegrationsIds(selectedGPConnectIntegrationsIds)?.ToList();
            var interoperabilityIntegrationTypes = SolutionsFilterHelper.ParseInteropIntegrationTypeIds(selectedInteroperabilityIds)?.ToList();

            var model = new SaveFilterModel(capabilitiesAndEpics, framework, applicationTypes, hostingTypes, iM1IntegrationsTypes, gPConnectIntegrationsTypes, interoperabilityIntegrationTypes, organisationId)
            {
                BackLink = backLink,
            };
            return View(model);
        }

        [HttpPost("save-filter-confirm")]
        public async Task<IActionResult> ConfirmSaveFilter(
            SaveFilterModel model,
            string selected)
        {
            var capabilityAndEpicIds = SolutionsFilterHelper.ParseCapabilityAndEpicIds(selected);
            var capabilitiesAndEpics = await capabilitiesService.GetGroupedCapabilitiesAndEpics(capabilityAndEpicIds);
            model.GroupedCapabilities = capabilitiesAndEpics;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await manageFiltersService.AddFilter(
                model.Name,
                model.Description,
                model.OrganisationId,
                capabilityAndEpicIds,
                model.FrameworkId,
                model.ApplicationTypes,
                model.HostingTypes,
                model.IM1IntegrationsTypes,
                model.GPConnectIntegrationsTypes,
                model.InteroperabilityIntegrationTypes);

            return RedirectToAction(
                nameof(Index),
                typeof(ManageFiltersController).ControllerName());
        }

        [HttpGet("filter-details")]
        public async Task<IActionResult> FilterDetails(int filterId)
        {
            var organisation = await GetUserOrganisation();
            var filterDetails = await manageFiltersService.GetFilterDetails(organisation.Id, filterId);
            var filterIds = await manageFiltersService.GetFilterIds(organisation.Id, filterId);

            if (filterDetails == null || filterIds == null)
                return NotFound();

            var model = new ReviewFilterModel(filterDetails, filterIds)
            {
                BackLink = Url.Action(nameof(Index), typeof(ManageFiltersController).ControllerName()),
                Caption = organisation.Name,
                InternalOrgId = organisation.InternalIdentifier,
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
            var oranisation = await GetUserOrganisation();
            var filter = await manageFiltersService.GetFilterDetails(oranisation.Id, filterId);
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

        private async Task<Organisation> GetUserOrganisation()
        {
            var organisationInternalIdentifier = User.GetPrimaryOrganisationInternalIdentifier();
            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(organisationInternalIdentifier);
            return organisation;
        }
    }
}
