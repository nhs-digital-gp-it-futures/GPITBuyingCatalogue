using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Dashboard;

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

            var odsCode = User.GetPrimaryOdsCode();

            return Redirect($"/order/organisation/{odsCode}");
        }

        // TODO - allroutes containing {odsCode} should be checked in pipeline to ensue user can use that odsCode
        [HttpGet("organisation/{odsCode}")]
        public IActionResult Organisation(string odsCode)
        {
            // TODO - This check needs applying everywhere, ideally middleware
            if (!User.IsBuyer())
                return View("NotBuyer");

            return View(new OrganisationModel());
        }

        [HttpGet("organisation/{odsCode}/select")]
        public IActionResult SelectOrganisation(string odsCode)
        {
            return View(new SelectOrganisationModel());
        }

        [HttpPost("organisation/{odsCode}/select")]
        public IActionResult SelectOrganisation(string odsCode, SelectOrganisationModel model)
        {
            return Redirect($"/order/organisation/{odsCode}");
        }

        [HttpGet("organisation/{odsCode}/order/neworder")]
        public IActionResult NewOrder(string odsCode)
        {
            return View(new NewOrderModel());
        }

        [HttpGet("organisation/{odsCode}/order/neworder/description")]
        public IActionResult NewOrderDescription(string odsCode)
        {
            return View(new NewOrderDescriptionModel());
        }

        [HttpPost("organisation/{odsCode}/order/neworder/description")]
        public IActionResult NewOrderDescription(string odsCode, NewOrderDescriptionModel model)
        {
            return Redirect($"/order/organisation/{odsCode}/order/C01005-01"); // TODO
        }
    }
}
