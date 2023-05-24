using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics;
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
                                Url = Url.Action(nameof(EditEpic), new { epicId = $"{x.Id}" }),
                            })));
        }

        [HttpGet("add-epic")]
        public async Task<IActionResult> AddEpic()
        {
            var capabilities = await capabilitiesService.GetCapabilities();
            var model = new SupplierDefinedEpicBaseModel()
            {
                BackLink = Url.Action(nameof(Dashboard)),
            };

            return View(model.WithSelectListCapabilities(capabilities));
        }

        [HttpPost("add-epic")]
        public async Task<IActionResult> AddEpic(SupplierDefinedEpicBaseModel model)
        {
            if (!ModelState.IsValid)
            {
                var capabilities = await capabilitiesService.GetCapabilities();
                return View(model.WithSelectListCapabilities(capabilities));
            }

            var createEpicModel = new AddEditSupplierDefinedEpic(
                model.SelectedCapabilityId!.Value,
                model.Name,
                model.Description,
                model.IsActive!.Value);

            await supplierDefinedEpicsService.AddSupplierDefinedEpic(createEpicModel);

            return RedirectToAction(nameof(Dashboard));
        }

        [HttpGet("edit/{epicId}")]
        public async Task<IActionResult> EditEpic(string epicId)
        {
            var epic = await supplierDefinedEpicsService.GetEpic(epicId);
            if (epic is null)
                return BadRequest($"No Supplier defined Epic found for Id: {epicId}");

            var capabilities = await capabilitiesService.GetCapabilities();
            var relatedItems = await supplierDefinedEpicsService.GetItemsReferencingEpic(epicId);

            var model = new EditSupplierDefinedEpicModel(epic, relatedItems)
            {
                BackLink = Url.Action(nameof(Dashboard)),
            };

            return View(model.WithSelectListCapabilities(capabilities) as EditSupplierDefinedEpicModel);
        }

        [HttpPost("edit/{epicId}")]
        public async Task<IActionResult> EditEpic(string epicId, EditSupplierDefinedEpicModel model)
        {
            if (!ModelState.IsValid)
            {
                var capabilities = await capabilitiesService.GetCapabilities();
                var relatedSolutions = await supplierDefinedEpicsService.GetItemsReferencingEpic(epicId);
                model.RelatedItems = relatedSolutions;

                return View(model.WithSelectListCapabilities(capabilities) as EditSupplierDefinedEpicModel);
            }

            var editEpicModel = new AddEditSupplierDefinedEpic(
                epicId,
                model.SelectedCapabilityId!.Value,
                model.Name,
                model.Description,
                model.IsActive!.Value);

            await supplierDefinedEpicsService.EditSupplierDefinedEpic(editEpicModel);

            return RedirectToAction(nameof(Dashboard));
        }

        [HttpGet("delete/{epicId}")]
        public async Task<IActionResult> DeleteEpic(string epicId)
        {
            var epic = await supplierDefinedEpicsService.GetEpic(epicId);
            if (epic is null)
                return RedirectToAction(nameof(Dashboard));

            var model = new DeleteSupplierDefinedEpicConfirmationModel(epicId, epic.Name)
            {
                BackLink = Url.Action(nameof(EditEpic), new { epicId }),
            };

            return View(model);
        }

        [HttpPost("delete/{epicId}")]
        public async Task<IActionResult> DeleteEpic(string epicId, DeleteSupplierDefinedEpicConfirmationModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(EditEpic), new { epicId });

            await supplierDefinedEpicsService.DeleteSupplierDefinedEpic(epicId);

            return RedirectToAction(nameof(Dashboard));
        }

        private async Task<IEnumerable<Epic>> GetFilteredEpics(string search)
        {
            return string.IsNullOrWhiteSpace(search)
                ? await supplierDefinedEpicsService.GetSupplierDefinedEpics()
                : await supplierDefinedEpicsService.GetSupplierDefinedEpicsBySearchTerm(search);
        }
    }
}
