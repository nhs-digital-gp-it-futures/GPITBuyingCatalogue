using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/supplier-defined-epics")]
    public class SupplierDefinedEpicsController : Controller
    {
        private readonly ISupplierDefinedEpicsService supplierDefinedEpicsService;

        public SupplierDefinedEpicsController(ISupplierDefinedEpicsService supplierDefinedEpicsService)
        {
            this.supplierDefinedEpicsService = supplierDefinedEpicsService ?? throw new ArgumentNullException(nameof(supplierDefinedEpicsService));
        }

        public async Task<IActionResult> Dashboard()
        {
            var supplierDefinedEpics = await supplierDefinedEpicsService.GetSupplierDefinedEpics();

            var model = new SupplierDefinedEpicsDashboardModel(supplierDefinedEpics);

            return View(model);
        }

        [HttpGet("add-epic")]
        public IActionResult AddEpic()
        {
            return View();
        }

        [HttpGet("edit/{epicId}")]
        public IActionResult EditEpic(string epicId)
        {
            return View();
        }
    }
}
