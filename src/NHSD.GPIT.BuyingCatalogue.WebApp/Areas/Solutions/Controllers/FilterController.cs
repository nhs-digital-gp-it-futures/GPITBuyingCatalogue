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
            [FromQuery] string selected = null,
            [FromQuery] string search = null)
        {
            return View(FilterCapabilitiesModel.Build(
                await capabilitiesService.GetReferencedCapabilities(),
                selected,
                search));
        }

        [HttpPost("filter-capabilities")]
        public async Task<IActionResult> FilterCapabilities(
            FilterCapabilitiesModel model,
            [FromQuery] string selected = null,
            [FromQuery] string search = null)
        {
            if (!ModelState.IsValid)
            {
                return View(FilterCapabilitiesModel.Build(
                    await capabilitiesService.GetReferencedCapabilities(),
                    search: search));
            }

            var currentCapabilitiesAndEpics = SolutionsFilterHelper.ParseCapabilityAndEpicIds(selected);

            var newCapabilitiesAndEpicsFilterString = new Dictionary<int, string[]>(
                model.SelectedItems
                  .Where(x => x.Selected && int.TryParse(x.Id, out _))
                  .Select(x => int.Parse(x.Id))
                  .Select(x => new KeyValuePair<int, string[]>(x, currentCapabilitiesAndEpics.GetValueOrDefault(x))))
                  .ToFilterString();

            return RedirectToAction(
                nameof(IncludeEpics),
                typeof(FilterController).ControllerName(),
                new
                {
                    selected = newCapabilitiesAndEpicsFilterString,
                    search,
                });
        }

        [HttpGet("include-epics")]
        public async Task<IActionResult> IncludeEpics(
            [FromQuery] string selected = null,
            [FromQuery] string search = null)
        {
            var capabilityAndEpicIds = SolutionsFilterHelper.ParseCapabilityAndEpicIds(selected);
            var epics = await epicsService.GetReferencedEpicsByCapabilityIds(capabilityAndEpicIds.Keys);

            if (!epics.Any())
            {
                return RedirectToAction(
                    nameof(SolutionsController.Index),
                    typeof(SolutionsController).ControllerName(),
                    new
                    {
                        selected,
                        search,
                    });
            }

            return View(new IncludeEpicsModel
            {
                Selected = selected,
            });
        }

        [HttpPost("include-epics")]
        public IActionResult IncludeEpics(
            IncludeEpicsModel model,
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
                    new { selected = model.Selected, search });
            }

            return RedirectToAction(
                nameof(SolutionsController.Index),
                typeof(SolutionsController).ControllerName(),
                new { selected = model.Selected, search });
        }

        [HttpGet("filter-epics")]
        public async Task<IActionResult> FilterEpics(
            [FromQuery] string selected,
            [FromQuery] string search = null)
        {
            return View(await GetEpicsModel(selected, search));
        }

        [HttpPost("filter-epics")]
        public async Task<IActionResult> FilterEpics(
            FilterEpicsModel model,
            [FromQuery] string search = null)
        {
            if (!ModelState.IsValid)
            {
                return View(await GetEpicsModel(model.Selected, search));
            }

            var currentCapabilitiesAndEpics = SolutionsFilterHelper.ParseCapabilityAndEpicIds(model.Selected);

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
                new { selected = newCapabilitiesAndEpics.ToFilterString(), search });
        }

        private async Task<FilterEpicsModel> GetEpicsModel(string selected, string search = null)
        {
            var capabilityAndEpicsIds = SolutionsFilterHelper.ParseCapabilityAndEpicIds(selected);
            var capabilities = await capabilitiesService.GetCapabilitiesByIds(capabilityAndEpicsIds.Keys);
            var epics = await epicsService.GetReferencedEpicsByCapabilityIds(capabilityAndEpicsIds.Keys);

            return new FilterEpicsModel(capabilities, epics, capabilityAndEpicsIds, search);
        }
    }
}
