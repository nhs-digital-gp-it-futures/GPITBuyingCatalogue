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

            return RedirectToAction(
                nameof(Organisation),
                typeof(DashboardController).ControllerName(),
                new { odsCode });
        }

        [HttpGet("organisation/{odsCode}")]
        public async Task<IActionResult> Organisation(string odsCode)
        {
            logger.LogInformation($"Taking user to {nameof(DashboardController)}.{nameof(Organisation)} for {nameof(odsCode)} {odsCode}");

            var organisation = await organisationsService.GetOrganisationByOdsCode(odsCode);

            var allOrders = await orderService.GetOrders(organisation.OrganisationId);

            return View(new OrganisationModel(organisation, User, allOrders));
        }

        [HttpGet("organisation/{odsCode}/select")]
        public async Task<IActionResult> SelectOrganisation(string odsCode)
        {
            logger.LogInformation($"Taking user to {nameof(DashboardController)}.{nameof(SelectOrganisation)} for {nameof(odsCode)} {odsCode}");

            var odsCodes = new List<string>(User.GetSecondaryOdsCodes())
            {
                User.GetPrimaryOdsCode(),
            };

            var organisations = await organisationsService.GetOrganisationsByOdsCodes(odsCodes.ToArray());

            return View(new SelectOrganisationModel(odsCode, organisations));
        }

        [HttpPost("organisation/{odsCode}/select")]
        public IActionResult SelectOrganisation(string odsCode, SelectOrganisationModel model)
        {
            logger.LogInformation($"Handling post for {nameof(DashboardController)}.{nameof(SelectOrganisation)} for {nameof(odsCode)} {odsCode}");

            if (!ModelState.IsValid)
                return View(model);

            return RedirectToAction(
                nameof(Organisation),
                typeof(DashboardController).ControllerName(),
                new { odsCode = model.SelectedOrganisation });
        }
    }
}
