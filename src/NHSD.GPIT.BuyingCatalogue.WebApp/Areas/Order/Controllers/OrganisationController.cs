using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    public class OrganisationController : Controller
    {
        private readonly ILogger<OrganisationController> _logger;
        
        public OrganisationController(ILogger<OrganisationController> logger)
        {
            _logger = logger;            
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
