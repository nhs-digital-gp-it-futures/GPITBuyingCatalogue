using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.Services.ServiceHelpers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

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
            RequestedFilters filters = null)
        {
            ArgumentNullException.ThrowIfNull(filters);

            var capabilityAndEpicsIds = filters.GetCapabilityAndEpicIds();
            var model = new FilterCapabilitiesModel(await capabilitiesService.GetReferencedCapabilities(), capabilityAndEpicsIds.Keys)
            {
                IsFilter = true,
                BackLink = Url.Action(
                    nameof(SolutionsController.Index),
                    typeof(SolutionsController).ControllerName(),
                    filters.ToRouteValues()),
            };

            return View(model);
        }

        [HttpPost("filter-capabilities")]
        public IActionResult FilterCapabilities(
            RequestedFilters filters,
            FilterCapabilitiesModel model)
        {
            ArgumentNullException.ThrowIfNull(filters);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentCapabilitiesAndEpics = filters.GetCapabilityAndEpicIds();

            var newCapabilitiesAndEpicsFilterString = new Dictionary<int, string[]>(
                model.SelectedItems
                  .Where(x => x.Selected && int.TryParse(x.Id, out _))
                  .Select(x => int.Parse(x.Id))
                  .Select(x => new KeyValuePair<int, string[]>(x, currentCapabilitiesAndEpics.GetValueOrDefault(x))))
                  .ToFilterString();

            return RedirectToAction(
                nameof(SolutionsController.Index),
                typeof(SolutionsController).ControllerName(),
                (filters with { Selected = newCapabilitiesAndEpicsFilterString }).ToRouteValues());
        }

        [HttpGet("filter-epics")]
        public async Task<IActionResult> FilterEpics(
            RequestedFilters filters = null)
        {
            ArgumentNullException.ThrowIfNull(filters);

            var capabilityAndEpicsIds = filters.GetCapabilityAndEpicIds();
            var capabilities = await capabilitiesService.GetCapabilitiesByIds(capabilityAndEpicsIds.Keys);
            var epics = await epicsService.GetReferencedEpicsByCapabilityIds(capabilityAndEpicsIds.Keys);

            var model = new FilterEpicsModel(capabilities, epics, capabilityAndEpicsIds)
            {
                BackLink = Url.Action(
                    nameof(SolutionsController.Index),
                    typeof(SolutionsController).ControllerName(),
                    filters.ToRouteValues()),
            };

            return View(model);
        }

        [HttpPost("filter-epics")]
        public IActionResult FilterEpics(
            RequestedFilters filters,
            FilterEpicsModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentCapabilitiesAndEpics = filters.GetCapabilityAndEpicIds();

            var changes = model.SelectedItems
                .Select(v => new
                {
                    CapabilityId = v.Id.Split(",")[0],
                    EpicId = v.Id.Split(",")[1],
                    SelectionModel = v,
                })
                .Where(v => int.TryParse(v.CapabilityId, out _))
                .GroupBy(v => int.Parse(v.CapabilityId), v => v)
                .ToDictionary(
                    v => v.Key,
                    v => v.Where(s => s.SelectionModel.Selected).Select(s => s.EpicId).ToArray());

            var newCapabilitiesAndEpics = new Dictionary<int, string[]>(currentCapabilitiesAndEpics
                .Select(kv => new KeyValuePair<int, string[]>(kv.Key, changes.GetValueOrDefault(kv.Key) ?? kv.Value)));

            return RedirectToAction(
                nameof(SolutionsController.Index),
                typeof(SolutionsController).ControllerName(),
                (filters with { Selected = newCapabilitiesAndEpics.ToFilterString() }).ToRouteValues());
        }
    }
}
