using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Dashboard;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order")]
    public class DashboardController : Controller
    {
        private readonly ILogWrapper<OrderController> logger;
        private readonly IOrganisationsService organisationsService;
        private readonly IOrderService orderService;

        public DashboardController(
            ILogWrapper<OrderController> logger,
            IOrganisationsService organisationsService,
            IOrderService orderService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        public IActionResult Index()
        {
            if (!User.IsBuyer())
                return View("NotBuyer");

            var odsCode = User.GetPrimaryOdsCode();

            return Redirect($"/order/organisation/{odsCode}");
        }

        [HttpGet("organisation/{odsCode}")]
        public async Task<IActionResult> Organisation(string odsCode)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));

            var organisation = await organisationsService.GetOrganisationByOdsCode(odsCode);

            return View(new OrganisationModel(organisation, User));
        }

        [HttpGet("organisation/{odsCode}/select")]
        public async Task<IActionResult> SelectOrganisation(string odsCode)
        {
            var odsCodes = new List<string>(User.GetSecondaryOdsCodes());
            odsCodes.Add(User.GetPrimaryOdsCode());

            var organisations = await organisationsService.GetOrganisationsByOdsCodes(odsCodes.ToArray());

            return View(new SelectOrganisationModel(odsCode, organisations));
        }

        [HttpPost("organisation/{odsCode}/select")]
        public IActionResult SelectOrganisation(string odsCode, SelectOrganisationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            return Redirect($"/order/organisation/{model.SelectedOrganisation}");
        }

        [HttpGet("organisation/{odsCode}/order/neworder")]
        public IActionResult NewOrder(string odsCode)
        {
            return View(new NewOrderModel(odsCode));
        }

        [HttpGet("organisation/{odsCode}/order/neworder/description")]
        public IActionResult NewOrderDescription(string odsCode)
        {
            return View(new NewOrderDescriptionModel(odsCode));
        }

        [HttpPost("organisation/{odsCode}/order/neworder/description")]
        public async Task<IActionResult> NewOrderDescription(string odsCode, NewOrderDescriptionModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var order = await orderService.CreateOrder(model.Description, model.OdsCode);

            return Redirect($"/order/organisation/{odsCode}/order/{order.CallOffId}");
        }
    }
}
