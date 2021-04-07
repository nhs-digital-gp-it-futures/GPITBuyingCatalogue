using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
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
            return View();
        }

        public IActionResult NewOrder()
        {
            return View();
        }
    }
}
