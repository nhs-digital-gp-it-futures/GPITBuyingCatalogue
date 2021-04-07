using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Area("Solutions")]
    public class DFOCVCController : Controller
    {
        private readonly ILogger<DFOCVCController> _logger;
        
        public DFOCVCController(ILogger<DFOCVCController> logger)
        {
            _logger = logger;            
        }

        public IActionResult Index()
        {                                    
            return View();
        }        
    }
}
