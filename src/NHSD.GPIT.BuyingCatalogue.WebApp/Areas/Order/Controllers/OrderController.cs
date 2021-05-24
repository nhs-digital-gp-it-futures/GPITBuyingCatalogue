using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}")]
    public class OrderController : Controller
    {
        private readonly ILogWrapper<OrderController> logger;

        public OrderController(ILogWrapper<OrderController> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public IActionResult Order(string odsCode, string callOffId)
        {
            return View(new OrderModel());
        }

        [HttpGet("summary")]
        public IActionResult Summary(string odsCode, string callOffId, string print = "false")
        {
            // TODO - obey the print switch
            return View(new SummaryModel());
        }

        [HttpGet("delete-order")]
        public IActionResult DeleteOrder(string odsCode, string callOffId)
        {
            return View(new DeleteOrderModel());
        }

        [HttpPost("delete-order")]
        public IActionResult DeleteOrder(string odsCode, string callOffId, DeleteOrderModel model)
        {
            return Redirect($"/order/organisation/{odsCode}");
        }

        [HttpGet("delete-order/confirmation")]
        public IActionResult DeleteOrderConfirmation(string odsCode, string callOffId)
        {
            return View(new DeleteConfirmationModel());
        }

        [HttpGet("description")]
        public IActionResult OrderDescription(string odsCode, string callOffId)
        {
            return View(new OrderDescriptionModel());
        }

        [HttpPost("description")]
        public IActionResult OrderDescription(string odsCode, string callOffId, OrderDescriptionModel model)
        {
            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}");
        }

        [HttpGet("ordering-party")]
        public IActionResult OrderingParty(string odsCode, string callOffId)
        {
            return View(new OrderingPartyModel());
        }

        [HttpPost("ordering-party")]
        public IActionResult OrderingParty(string odsCode, string callOffId, OrderingPartyModel model)
        {
            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}");
        }

        [HttpGet("commencement-date")]
        public IActionResult CommencementDate(string odsCode, string callOffId)
        {
            return View(new CommencementDateModel());
        }

        [HttpPost("commencement-date")]
        public IActionResult CommencementDate(string odsCode, string callOffId, CommencementDateModel model)
        {
            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}");
        }

        [HttpGet("funding-source")]
        public IActionResult FundingSource(string odsCode, string callOffId)
        {
            return View(new FundingSourceModel());
        }

        [HttpPost("funding-source")]
        public IActionResult FundingSource(string odsCode, string callOffId, FundingSourceModel model)
        {
            return Redirect($"/order/organisation/{odsCode}/order/{callOffId}");
        }
    }
}
