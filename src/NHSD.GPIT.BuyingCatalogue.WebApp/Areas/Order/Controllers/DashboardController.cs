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
    public class DashboardController : Controller
    {
        private readonly ILogWrapper<OrderController> logger;

        public DashboardController(ILogWrapper<OrderController> logger)
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

        [HttpGet("organisation/{odsCode}/select")]
        public IActionResult SelectOrganisation(string odsCode)
        {
            if (!User.IsBuyer())
                return View("NotBuyer");

            return View(new SelectOrganisationModel());
        }

        [HttpPost("organisation/{odsCode}/select")]
        public IActionResult SelectOrganisation(string odsCode, SelectOrganisationModel model)
        {
            return Redirect($"/order/organisation/03F");
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



    }
}
