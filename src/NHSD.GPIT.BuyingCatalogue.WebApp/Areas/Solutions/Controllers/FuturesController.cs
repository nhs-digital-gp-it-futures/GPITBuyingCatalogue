using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Area("Solutions")]
    public class FuturesController : Controller
    {
        private readonly ILogger<FuturesController> _logger;
        
        public FuturesController(ILogger<FuturesController> logger)
        {
            _logger = logger;            
        }

        public IActionResult Index()
        {                                    
            return View();
        }        
    }
}
