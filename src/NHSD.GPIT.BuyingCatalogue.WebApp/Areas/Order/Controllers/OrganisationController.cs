using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    public class OrganisationController : Controller
    {
        private readonly ILogWrapper<OrganisationController> logger;

        public OrganisationController(ILogWrapper<OrganisationController> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index()
        {
            if (!User.IsBuyer())
                return View("NotBuyer");

            return View();
        }

        public IActionResult NewOrder()
        {
            return View();
        }
    }
}
