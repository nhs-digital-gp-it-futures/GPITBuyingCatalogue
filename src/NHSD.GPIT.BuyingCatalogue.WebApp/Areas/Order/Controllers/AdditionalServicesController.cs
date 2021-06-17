using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}/additional-services")]
    public class AdditionalServicesController : Controller
    {
        private readonly ILogWrapper<OrderController> logger;

        public AdditionalServicesController(ILogWrapper<OrderController> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index(string odsCode, string callOffId)
        {
            return View(new AdditionalServiceModel());
        }

        [HttpGet("select/additional-service")]
        public IActionResult SelectAdditionalService(string odsCode, string callOffId)
        {
            return View(new SelectAdditionalServiceModel());
        }

        [HttpPost("select/additional-service")]
        public IActionResult SelectAdditionalService(string odsCode, string callOffId, SelectAdditionalServiceModel model)
        {
            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}/additional-services/select/additional-service/price/recipients");
        }

        [HttpGet("select/additional-service/price/recipients")]
        public IActionResult SelectAdditionalServicePriceRecipients(string odsCode, string callOffId)
        {
            return View(new SelectAdditionalServiceRecipientsModel());
        }

        [HttpPost("select/additional-service/price/recipients")]
        public IActionResult SelectAdditionalServicePriceRecipients(string odsCode, string callOffId, SelectAdditionalServiceRecipientsModel model)
        {
            // TODO this isn't the correct redirect if we got here from the Edit service recipients button
            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}/additional-services/select/additional-service/price/recipients/date");
        }

        [HttpGet("select/additional-service/price/recipients/date")]
        public IActionResult SelectAdditionalServicePriceRecipientsDate(string odsCode, string callOffId)
        {
            return View(new SelectAdditionalServiceRecipientsDateModel());
        }

        [HttpPost("select/additional-service/price/recipients/date")]
        public IActionResult SelectAdditionalServicePriceRecipientsDate(string odsCode, string callOffId, SelectAdditionalServiceRecipientsDateModel model)
        {
            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}/additional-services/neworderitem");
        }

        [HttpGet("neworderitem")]
        public IActionResult NewOrderItem(string odsCode, string callOffId)
        {
            return View(new NewAdditionalServiceOrderItemModel());
        }

        [HttpPost("neworderitem")]
        public IActionResult NewOrderItem(string odsCode, string callOffId, NewAdditionalServiceOrderItemModel model)
        {
            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}/additional-services");
        }

        [HttpGet("{id}")]
        public IActionResult EditAdditionalService(string odsCode, string callOffId, string id)
        {
            return View(new EditAdditionalServiceModel());
        }

        [HttpPost("{id}")]
        public IActionResult EditAdditionalService(string odsCode, string callOffId, string id, EditAdditionalServiceModel model)
        {
            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}/additional-services");
        }

        [HttpGet("delete/{id}/confirmation/{serviceName}")]
        public IActionResult DeleteAdditionalService(string odsCode, string callOffId, string id, string serviceName)
        {
            return View(new DeleteAdditionalServiceModel());
        }

        [HttpPost("delete/{id}/confirmation/{serviceName}")]
        public IActionResult DeleteAdditionalService(string odsCode, string callOffId, string id, string serviceName, DeleteAdditionalServiceModel model)
        {
            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}/additional-services/delete/{id}/confirmation/{serviceName}/continue");
        }

        [HttpGet("delete/{id}/confirmation/{serviceName}/continue")]
        public IActionResult DeleteContinue(string odsCode, string callOffId, string id, string serviceName)
        {
            return View(new DeleteContinueModel());
        }
    }
}
