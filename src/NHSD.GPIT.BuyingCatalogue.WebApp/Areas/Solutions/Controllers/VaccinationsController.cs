using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Area("Solutions")]
    public class VaccinationsController : Controller
    {
        private readonly ILogger<VaccinationsController> _logger;
        
        public VaccinationsController(ILogger<VaccinationsController> logger)
        {
            _logger = logger;            
        }

        public IActionResult Index()
        {                                    
            return View();
        }        
    }
}
