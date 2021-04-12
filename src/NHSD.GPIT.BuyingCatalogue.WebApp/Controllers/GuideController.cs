using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    public class GuideController : Controller
    {
        private readonly ILogger<GuideController> _logger;

        public GuideController(ILogger<GuideController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
