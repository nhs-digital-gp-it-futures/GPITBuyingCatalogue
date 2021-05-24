using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
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

        public DashboardController(
            ILogWrapper<OrderController> logger,
            IOrganisationsService organisationsService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
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

            return View(new OrganisationModel(organisation));
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
