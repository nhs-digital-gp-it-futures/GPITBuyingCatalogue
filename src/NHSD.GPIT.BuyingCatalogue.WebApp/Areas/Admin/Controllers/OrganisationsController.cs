using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrganisationsController : Controller
    {
        private readonly ILogger<OrganisationsController> _logger;
        
        public OrganisationsController(ILogger<OrganisationsController> logger)
        {
            _logger = logger;            
        }

        public IActionResult Index()
        {                                    
            return View();
        }        
    }
}
