using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
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
        public async Task<IActionResult> FilterCapabilities(string selectedIds = null)
        {
            return View(await GetCapabilitiesModel(selectedIds));
        }

        [HttpPost("filter-capabilities")]
        public async Task<IActionResult> FilterCapabilities(FilterCapabilitiesModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(await GetCapabilitiesModel());
            }

            var selectedCapabilityIds = string.Join(
                FilterConstants.Delimiter,
                model.SelectedItems.Where(x => x.Selected).Select(x => x.Id));

            return RedirectToAction(
                nameof(IncludeEpics),
                typeof(FilterController).ControllerName(),
                new { selectedCapabilityIds });
        }

        [HttpGet("include-epics")]
        public async Task<IActionResult> IncludeEpics(string selectedCapabilityIds)
        {
            var capabilityIds = GetIds(selectedCapabilityIds);
            var epics = await epicsService.GetActiveEpicsByCapabilityIds(capabilityIds);

            if (!epics.Any())
            {
                return RedirectToAction(
                    nameof(FilterCapabilities),
                    typeof(FilterController).ControllerName(),
                    new { selectedIds = selectedCapabilityIds });
            }

            return View(new IncludeEpicsModel
            {
                SelectedCapabilityIds = selectedCapabilityIds,
            });
        }

        [HttpPost("include-epics")]
        public IActionResult IncludeEpics(IncludeEpicsModel model)
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
                    new { model.SelectedCapabilityIds });
            }

            return RedirectToAction(
                nameof(FilterCapabilities),
                typeof(FilterController).ControllerName(),
                new { selectedIds = model.SelectedCapabilityIds });
        }

        [HttpGet("filter-epics")]
        public async Task<IActionResult> FilterEpics(string selectedCapabilityIds, string selectedIds = null)
        {
            return View(await GetEpicsModel(selectedCapabilityIds, selectedIds));
        }

        [HttpPost("filter-epics")]
        public async Task<IActionResult> FilterEpics(FilterEpicsModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(await GetEpicsModel(model.CapabilityIds));
            }

            var selectedIds = string.Join(
                FilterConstants.Delimiter,
                model.SelectedItems.Where(x => x.Selected).Select(x => x.Id));

            return RedirectToAction(
                nameof(FilterEpics),
                typeof(FilterController).ControllerName(),
                new { selectedCapabilityIds = model.CapabilityIds, selectedIds });
        }

        private async Task<FilterCapabilitiesModel> GetCapabilitiesModel(string selectedIds = null)
        {
            var capabilities = await capabilitiesService.GetCapabilities();

            return new FilterCapabilitiesModel(capabilities, selectedIds);
        }

        private async Task<FilterEpicsModel> GetEpicsModel(string selectedCapabilityIds, string selectedEpicIds = null)
        {
            var capabilityIds = GetIds(selectedCapabilityIds);
            var capabilities = await capabilitiesService.GetCapabilitiesByIds(capabilityIds);
            var epics = await epicsService.GetActiveEpicsByCapabilityIds(capabilityIds);

            return new FilterEpicsModel(capabilities, epics, selectedEpicIds);
        }

        private List<int> GetIds(string input)
        {
            return input?.Split(FilterConstants.Delimiter)
                .Where(x => int.TryParse(x, out _))
                .Select(int.Parse)
                .ToList() ?? new List<int>();
        }
    }
}
