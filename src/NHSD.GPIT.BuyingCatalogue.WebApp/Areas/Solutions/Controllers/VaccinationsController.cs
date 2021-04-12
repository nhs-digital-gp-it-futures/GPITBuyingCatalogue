using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers
{
    [Area("Solutions")]
    public class VaccinationsController : Controller
    {
        private readonly ILogger<VaccinationsController> _logger;
        
        public VaccinationsController(ILogger<VaccinationsController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index()
        {                                    
            return View();
        }        
    }
}
