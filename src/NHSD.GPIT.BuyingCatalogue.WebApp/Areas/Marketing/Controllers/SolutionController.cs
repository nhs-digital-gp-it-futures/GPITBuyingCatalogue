using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    public class SolutionController : Controller
    {
        private readonly ILogger<SolutionController> _logger;
        
        public SolutionController(ILogger<SolutionController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Route("Marketing/Supplier/Solution/{id}")]
        public IActionResult Index(string id)
        {                                    
            return View();
        }

        [HttpGet("Marketing/Supplier/Solution/{id}/Preview")]
        public IActionResult Preview(string id)
        {
            return View();
        }
    }
}
