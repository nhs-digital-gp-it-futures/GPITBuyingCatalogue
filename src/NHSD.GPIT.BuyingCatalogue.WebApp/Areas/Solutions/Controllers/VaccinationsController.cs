using System;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers
{
    [Area("Solutions")]
    public class VaccinationsController : Controller
    {
        private readonly ILogWrapper<VaccinationsController> _logger;
        
        public VaccinationsController(ILogWrapper<VaccinationsController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index()
        {                                    
            return View();
        }        
    }
}
