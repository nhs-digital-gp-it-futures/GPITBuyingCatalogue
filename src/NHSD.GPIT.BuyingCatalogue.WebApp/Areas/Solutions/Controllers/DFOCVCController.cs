using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers
{
    [Area("Solutions")]
    public class DFOCVCController : Controller
    {
        private readonly ILogger<DFOCVCController> _logger;
        
        public DFOCVCController(ILogger<DFOCVCController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index()
        {                                    
            return View();
        }        
    }
}
