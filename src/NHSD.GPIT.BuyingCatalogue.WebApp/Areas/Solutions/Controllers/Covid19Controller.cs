using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers
{
    [Area("Solutions")]
    public class Covid19Controller : Controller
    {
        private readonly ILogger<Covid19Controller> _logger;
        
        public Covid19Controller(ILogger<Covid19Controller> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index()
        {                                    
            return View();
        }        
    }
}
