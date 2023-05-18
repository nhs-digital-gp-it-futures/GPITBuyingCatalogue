using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Services.ServiceHelpers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers
{
    [Area("Solutions")]
    [Route("catalogue-solutions")]
    public class FilterController : Controller
    {
        private readonly ICapabilitiesService capabilitiesService;
        private readonly IEpicsService epicsService;

        public FilterController(
            ICapabilitiesService capabilitiesService,
            IEpicsService epicsService)
        {
            this.capabilitiesService = capabilitiesService ?? throw new ArgumentNullException(nameof(capabilitiesService));
            this.epicsService = epicsService ?? throw new ArgumentNullException(nameof(epicsService));
        }

        [HttpGet("filter-capabilities")]
        public async Task<IActionResult> FilterCapabilities(
            [FromQuery] string selectedCapabilityIds = null,
            [FromQuery] string search = null)
        {
            return View(await GetCapabilitiesModel(selectedCapabilityIds, search));
        }

        [HttpPost("filter-capabilities")]
        public async Task<IActionResult> FilterCapabilities(
            FilterCapabilitiesModel model,
            [FromQuery] string selectedEpicIds = null,
            [FromQuery] string search = null)
        {
            if (!ModelState.IsValid)
                return View(await GetCapabilitiesModel(search: search));

            var selectedCapabilityIds = EncodeIdString(model.SelectedItems);

            var remainingEpicIds = await epicsService.GetEpicsForSelectedCapabilities(
                SolutionsFilterHelper.ParseCapabilityIds(selectedCapabilityIds),
                SolutionsFilterHelper.ParseEpicIds(selectedEpicIds));

            return RedirectToAction(
                nameof(IncludeEpics),
                typeof(FilterController).ControllerName(),
                new { selectedCapabilityIds, selectedEpicIds = remainingEpicIds, search });
        }

        [HttpGet("include-epics")]
        public async Task<IActionResult> IncludeEpics(
            [FromQuery] string selectedCapabilityIds = null,
            [FromQuery] string search = null)
        {
            var capabilityIds = SolutionsFilterHelper.ParseCapabilityIds(selectedCapabilityIds);
            var epics = await epicsService.GetReferencedEpicsByCapabilityIds(capabilityIds);

            if (!epics.Any())
            {
                return RedirectToAction(
                    nameof(SolutionsController.Index),
                    typeof(SolutionsController).ControllerName(),
                    new { selectedCapabilityIds, search });
            }

            return View(new IncludeEpicsModel
            {
                SelectedCapabilityIds = selectedCapabilityIds,
            });
        }

        [HttpPost("include-epics")]
        public IActionResult IncludeEpics(
            IncludeEpicsModel model,
            [FromQuery] string selectedEpicIds = null,
            [FromQuery] string search = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.IncludeEpics.GetValueOrDefault())
            {
                return RedirectToAction(
                    nameof(FilterEpics),
                    typeof(FilterController).ControllerName(),
                    new { model.SelectedCapabilityIds, selectedEpicIds, search });
            }

            return RedirectToAction(
                nameof(SolutionsController.Index),
                typeof(SolutionsController).ControllerName(),
                new { model.SelectedCapabilityIds, selectedEpicIds, search });
        }

        [HttpGet("filter-epics")]
        public async Task<IActionResult> FilterEpics(
            [FromQuery] string selectedCapabilityIds,
            [FromQuery] string selectedEpicIds = null,
            [FromQuery] string search = null)
        {
            return View(await GetEpicsModel(selectedCapabilityIds, selectedEpicIds, search));
        }

        [HttpPost("filter-epics")]
        public async Task<IActionResult> FilterEpics(
            FilterEpicsModel model,
            [FromQuery] string search = null)
        {
            if (!ModelState.IsValid)
            {
                return View(await GetEpicsModel(model.CapabilityIds));
            }

            var selectedEpicIds = EncodeIdString(model.SelectedItems);

            return RedirectToAction(
                nameof(SolutionsController.Index),
                typeof(SolutionsController).ControllerName(),
                new { selectedCapabilityIds = model.CapabilityIds, selectedEpicIds, search });
        }

        private static string EncodeIdString(SelectionModel[] selectedItems) =>
            string.Join(
                FilterConstants.Delimiter,
                selectedItems.Where(x => x.Selected).Select(x => x.Id));

        private async Task<FilterCapabilitiesModel> GetCapabilitiesModel(string selectedIds = null, string search = null)
        {
            var capabilities = await capabilitiesService.GetReferencedCapabilities();

            return new(capabilities, selectedIds, search);
        }

        private async Task<FilterEpicsModel> GetEpicsModel(string selectedCapabilityIds, string selectedEpicIds = null, string search = null)
        {
            var capabilityIds = SolutionsFilterHelper.ParseCapabilityIds(selectedCapabilityIds);
            var capabilities = await capabilitiesService.GetCapabilitiesByIds(capabilityIds);
            var epics = await epicsService.GetReferencedEpicsByCapabilityIds(capabilityIds);

            return new FilterEpicsModel(capabilities, epics, selectedEpicIds, search);
        }
    }
}
