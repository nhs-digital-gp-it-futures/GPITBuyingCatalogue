using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.Services.ServiceHelpers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/supplier-defined-epics")]
    public class SupplierDefinedEpicsController : Controller
    {
        private readonly ISupplierDefinedEpicsService supplierDefinedEpicsService;
        private readonly ICapabilitiesService capabilitiesService;

        public SupplierDefinedEpicsController(
            ISupplierDefinedEpicsService supplierDefinedEpicsService,
            ICapabilitiesService capabilitiesService)
        {
            this.supplierDefinedEpicsService = supplierDefinedEpicsService ?? throw new ArgumentNullException(nameof(supplierDefinedEpicsService));
            this.capabilitiesService = capabilitiesService ?? throw new ArgumentNullException(nameof(capabilitiesService));
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard([FromQuery] string search = null)
        {
            var supplierDefinedEpics = await GetFilteredEpics(search);

            var model = new SupplierDefinedEpicsDashboardModel(supplierDefinedEpics, search);

            return View(model);
        }

        [HttpGet("search-results")]
        public async Task<IActionResult> SearchResults([FromQuery] string search)
        {
            var results = await GetFilteredEpics(search);

            return Json(
                results.Take(15)
                    .SelectMany(
                        x => x.Capabilities.Select(
                            y => new SuggestionSearchResult
                            {
                                Title = x.Name,
                                Category = y.Name,
                                Url = Url.Action(nameof(EditSupplierDefinedEpic), new { epicId = $"{x.Id}" }),
                            })));
        }

        [HttpGet("select-capabilities")]
        public async Task<IActionResult> SelectCapabilities(string selectedCapabilityIds = null)
        {
            var capabilities = await capabilitiesService.GetCapabilities();
            var selected = SolutionsFilterHelper.ParseCapabilityIds(selectedCapabilityIds);
            var model = new FilterCapabilitiesModel(capabilities, false, selected)
            {
                BackLink = Url.Action(nameof(Dashboard)),
            };

            return View("FilterCapabilities", model);
        }

        [HttpPost("select-capabilities")]
        public async Task<IActionResult> SelectCapabilities(
            FilterCapabilitiesModel model)
        {
            if (!ModelState.IsValid)
            {
                var capabilities = await capabilitiesService.GetCapabilities();
                var newModel = new FilterCapabilitiesModel(capabilities, false, null)
                {
                    BackLink = Url.Action(nameof(Dashboard)),
                };
                return View("FilterCapabilities", newModel);
            }

            var selectedCapabilityIds = EncodeIdString(model.SelectedItems);

            return RedirectToAction(
                nameof(AddSupplierDefinedEpicDetails),
                typeof(SupplierDefinedEpicsController).ControllerName(),
                new { selectedCapabilityIds });
        }

        [HttpGet("add-epic")]
        public IActionResult AddSupplierDefinedEpicDetails(string selectedCapabilityIds = null)
        {
            var model = new AddSupplierDefinedEpicDetailsModel()
            {
                BackLink = Url.Action(
                    nameof(SelectCapabilities),
                    typeof(SupplierDefinedEpicsController).ControllerName(),
                    new { selectedCapabilityIds }),
            };
            return View(model);
        }

        [HttpPost("add-epic")]
        public async Task<IActionResult> AddSupplierDefinedEpicDetails(AddSupplierDefinedEpicDetailsModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var createEpicModel = new AddEditSupplierDefinedEpic(
                (List<int>)SolutionsFilterHelper.ParseCapabilityIds(model.SelectedCapabilityIds),
                model.Name,
                model.Description,
                model.IsActive!.Value);

            await supplierDefinedEpicsService.AddSupplierDefinedEpic(createEpicModel);

            return RedirectToAction(nameof(Dashboard));
        }

        [HttpGet("edit/{epicId}")]
        public async Task<IActionResult> EditSupplierDefinedEpic(string epicId)
        {
            var epic = await supplierDefinedEpicsService.GetEpic(epicId);
            if (epic is null)
                return BadRequest($"No Supplier defined Epic found for Id: {epicId}");

            var relatedItems = await supplierDefinedEpicsService.GetItemsReferencingEpic(epicId);

            var model = new EditSupplierDefinedEpicModel(epic, relatedItems)
            {
                BackLink = Url.Action(nameof(Dashboard)),
            };

            return View(model);
        }

        [HttpPost("edit/{epicId}")]
        public IActionResult EditSupplierDefinedEpic(string epicId, EditSupplierDefinedEpicModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return RedirectToAction(nameof(Dashboard));
        }

        [HttpGet("edit/{epicId}/select-capabilities")]
        public async Task<IActionResult> EditCapabilities(string epicId)
        {
            var epic = await supplierDefinedEpicsService.GetEpic(epicId);
            if (epic is null)
                return BadRequest($"No Supplier defined Epic found for Id: {epicId}");

            var capabilities = await capabilitiesService.GetCapabilities();
            var relatedItems = await supplierDefinedEpicsService.GetItemsReferencingEpic(epicId);
            var selectedCapabilityIds = epic.Capabilities.Select(x => x.Id).ToList();

            var model = new FilterCapabilitiesModel(capabilities, false, selectedCapabilityIds)
            {
                BackLink = Url.Action(nameof(EditSupplierDefinedEpic), new { epicId }),
            };

            return View("FilterCapabilities", model);
        }

        [HttpPost("edit/{epicId}/select-capabilities")]
        public async Task<IActionResult> EditCapabilities(string epicId, FilterCapabilitiesModel model)
        {
            if (!ModelState.IsValid)
            {
                var newEpic = await supplierDefinedEpicsService.GetEpic(epicId);
                if (newEpic is null)
                    return BadRequest($"No Supplier defined Epic found for Id: {epicId}");

                var capabilities = await capabilitiesService.GetCapabilities();
                var relatedItems = await supplierDefinedEpicsService.GetItemsReferencingEpic(epicId);
                var selectedCapabilityIds = newEpic.Capabilities.Select(x => x.Id).ToList();

                var newModel = new FilterCapabilitiesModel(capabilities, false, selectedCapabilityIds)
                {
                    BackLink = Url.Action(nameof(EditSupplierDefinedEpic), new { epicId }),
                };

                return View("FilterCapabilities", newModel);
            }

            var epic = await supplierDefinedEpicsService.GetEpic(epicId);
            if (epic is null)
                return BadRequest($"No Supplier defined Epic found for Id: {epicId}");

            List<int> selectedIds = model.SelectedItems.Where(x => x.Selected).Select(x => int.Parse(x.Id)).ToList();

            var editEpicModel = new AddEditSupplierDefinedEpic(
                epicId,
                selectedIds,
                epic.Name,
                epic.Description,
                epic.IsActive);

            await supplierDefinedEpicsService.EditSupplierDefinedEpic(editEpicModel);

            return RedirectToAction(
                nameof(EditSupplierDefinedEpic),
                new { epicId });
        }

        [HttpGet("edit/{epicId}/epic-details")]
        public async Task<IActionResult> EditSupplierDefinedEpicDetails(string epicId)
        {
            var epic = await supplierDefinedEpicsService.GetEpic(epicId);
            if (epic is null)
                return BadRequest($"No Supplier defined Epic found for Id: {epicId}");
            var relatedItems = await supplierDefinedEpicsService.GetItemsReferencingEpic(epicId);

            var model = new EditSupplierDefinedEpicDetailsModel(epic, relatedItems)
            {
            };

            return View(model);
        }

        [HttpPost("edit/{epicId}/epic-details")]
        public async Task<IActionResult> EditSupplierDefinedEpicDetails(string epicId, EditSupplierDefinedEpicDetailsModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var epic = await supplierDefinedEpicsService.GetEpic(epicId);
            if (epic is null)
                return BadRequest($"No Supplier defined Epic found for Id: {epicId}");

            var editEpicModel = new AddEditSupplierDefinedEpic(
                epicId,
                epic.Capabilities.Select(x => x.Id).ToList(),
                model.Name,
                model.Description,
                model.IsActive!.Value);

            await supplierDefinedEpicsService.EditSupplierDefinedEpic(editEpicModel);

            return RedirectToAction(
                nameof(EditSupplierDefinedEpic),
                new { epicId });
        }

        private static string EncodeIdString(SelectionModel[] selectedItems) =>
                selectedItems.Where(x => x.Selected).Select(x => x.Id).ToFilterString();

        private async Task<IEnumerable<Epic>> GetFilteredEpics(string search)
        {
            return string.IsNullOrWhiteSpace(search)
                ? await supplierDefinedEpicsService.GetSupplierDefinedEpics()
                : await supplierDefinedEpicsService.GetSupplierDefinedEpicsBySearchTerm(search);
        }
    }
}
