using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}/associated-services")]
    public class AssociatedServicesController : Controller
    {
        private readonly ILogWrapper<OrderController> logger;

        public AssociatedServicesController(ILogWrapper<OrderController> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index(string odsCode, string callOffId)
        {
            return View(new AssociatedServiceModel());
        }

        [HttpGet("select/associated-service")]
        public IActionResult SelectAssociatedService(string odsCode, string callOffId)
        {
            return View(new SelectAssociatedServiceModel());
        }

        [HttpPost("select/associated-service")]
        public IActionResult SelectAssociatedService(string odsCode, string callOffId, SelectAssociatedServiceModel model)
        {
            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}/associated-services/neworderitem");
        }

        [HttpGet("neworderitem")]
        public IActionResult NewOrderItem(string odsCode, string callOffId)
        {
            return View(new NewAssociatedServiceOrderItemModel());
        }

        [HttpPost("neworderitem")]
        public IActionResult NewOrderItem(string odsCode, string callOffId, NewAssociatedServiceOrderItemModel model)
        {
            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}/associated-services");
        }

        [HttpGet("{id}")]
        public IActionResult EditAssociatedService(string odsCode, string callOffId, string id)
        {
            return View(new EditAssociatedServiceModel());
        }

        [HttpPost("{id}")]
        public IActionResult EditAssociatedService(string odsCode, string callOffId, string id, EditAssociatedServiceModel model)
        {
            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}/associated-services");
        }

        [HttpGet("delete/{id}/confirmation/{serviceName}")]
        public IActionResult DeleteAssociatedService(string odsCode, string callOffId, string id, string serviceName)
        {
            return View(new DeleteAssociatedServiceModel());
        }

        [HttpPost("delete/{id}/confirmation/{serviceName}")]
        public IActionResult DeleteAssociatedService(string odsCode, string callOffId, string id, string serviceName, DeleteAssociatedServiceModel model)
        {
            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}/associated-services/delete/10001-01/confirmation/AnywereConsult/continue");
        }

        [HttpGet("delete/{id}/confirmation/{serviceName}/continue")]
        public IActionResult DeleteContinue(string odsCode, string callOffId, string id, string serviceName)
        {
            return View(new DeleteContinueModel());
        }
    }
}
