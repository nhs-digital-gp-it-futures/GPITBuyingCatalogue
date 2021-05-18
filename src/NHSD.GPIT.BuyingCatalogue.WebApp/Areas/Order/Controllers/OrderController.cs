using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order")]
    public class OrderController : Controller
    {
        private readonly ILogWrapper<OrderController> logger;

        public OrderController(ILogWrapper<OrderController> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index()
        {
            if (!User.IsBuyer())
                return View("NotBuyer");

            // TODO - determine ODS code. I assume the proxy pages come in here is user can proxy buy
            return Redirect($"/order/organisation/03F");
        }

        [HttpGet("organisation/{odsCode}")]
        public IActionResult Organisation(string odsCode)
        {
            if (!User.IsBuyer())
                return View("NotBuyer");

            return View();
        }

        [HttpGet("organisation/{odsCode}/order/neworder")]
        public IActionResult NewOrder(string odsCode)
        {
            return View();
        }

        [HttpGet("organisation/{odsCode}/order/neworder/description")]
        public IActionResult NewOrderDescription(string odsCode)
        {            
            return View(new OrderDescriptionModel());
        }

        [HttpPost("organisation/{odsCode}/order/neworder/description")]
        public IActionResult NewOrderDescription(string odsCode, OrderDescriptionModel model )
        {
            return Redirect($"/order/organisation/03F/order/C01005-01");
        }


        [HttpGet("organisation/{odsCode}/order/{callOffId}")]
        public IActionResult Order(string odsCode, string callOffId)
        {
            return View();
        }


        [HttpGet("organisation/{odsCode}/order/{callOffId}/summary")]
        public IActionResult Summary(string odsCode, string callOffId, string print = "false")
        {
            // TODO - obey the print switch

            return View();
        }


        [HttpGet("organisation/{odsCode}/order/{callOffId}/delete-order")]
        public IActionResult DeleteOrder(string odsCode, string callOffId)
        {            
            return View(new DeleteOrderModel());
        }

        [HttpPost("organisation/{odsCode}/order/{callOffId}/delete-order")]
        public IActionResult DeleteOrder(string odsCode, string callOffId, DeleteOrderModel model)
        {
            return Redirect($"/order/organisation/03F");
        }

        [HttpGet("organisation/{odsCode}/order/{callOffId}/delete-order/confirmation")]
        public IActionResult DeleteOrderConfirmation(string odsCode, string callOffId)
        {
            return View();
        }

        [HttpGet("organisation/{odsCode}/order/{callOffId}/description")]
        public IActionResult OrderDescription(string odsCode, string callOffId)
        {
            return View(new OrderDescriptionModel());
        }

        [HttpPost("organisation/{odsCode}/order/{callOffId}/description")]
        public IActionResult OrderDescription(string odsCode, string callOffId, OrderDescriptionModel model)
        {
            return Redirect($"/order/organisation/03F/order/C01005-01");
        }


        [HttpGet("organisation/{odsCode}/order/{callOffId}/ordering-party")]
        public IActionResult OrderingParty(string odsCode, string callOffId)
        {
            return View(new OrderingPartyModel());
        }

        [HttpPost("organisation/{odsCode}/order/{callOffId}/ordering-party")]
        public IActionResult OrderingParty(string odsCode, string callOffId, OrderingPartyModel model)
        {
            return Redirect($"/order/organisation/03F/order/C01005-01");
        }


        [HttpGet("organisation/{odsCode}/order/{callOffId}/supplier")]
        public IActionResult Supplier(string odsCode, string callOffId)
        {
            // TODO - display view if supplier already defined otherwise rediect to select

            //return View(new SupplierModel());
            return Redirect($"/order/organisation/03F/order/C01005-01/supplier/search");
        }

        [HttpGet("organisation/{odsCode}/order/{callOffId}/supplier/search")]
        public IActionResult SupplierSearch(string odsCode, string callOffId)
        {
            return View(new SupplierSearchModel());
        }

        [HttpPost("organisation/{odsCode}/order/{callOffId}/supplier/search")]
        public IActionResult SupplierSearch(string odsCode, string callOffId, SupplierSearchModel model)
        {
            // TODO - Display NoSupplierFound if no results

            return Redirect($"/order/organisation/03F/order/C01005-01/supplier/search/select");
        }

        [HttpGet("organisation/{odsCode}/order/{callOffId}/supplier/search/select")]
        public IActionResult SupplierSearchSelect(string odsCode, string callOffId)
        {
            return View(new SupplierSearchSelectModel());
        }

        [HttpPost("organisation/{odsCode}/order/{callOffId}/supplier/search/select")]
        public IActionResult SupplierSearchSelect(string odsCode, string callOffId, SupplierSearchSelectModel model)
        {
            return Redirect($"/order/organisation/03F/order/C01005-01/supplier");
        }
    }
}
