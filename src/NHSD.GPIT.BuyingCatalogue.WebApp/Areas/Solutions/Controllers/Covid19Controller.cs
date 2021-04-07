using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Area("Solutions")]
    public class Covid19Controller : Controller
    {
        private readonly ILogger<Covid19Controller> _logger;
        
        public Covid19Controller(ILogger<Covid19Controller> logger)
        {
            _logger = logger;            
        }

        public IActionResult Index()
        {                                    
            return View();
        }        
    }
}
