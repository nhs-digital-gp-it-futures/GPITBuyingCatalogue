using System;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    public class GuideController : Controller
    {
        private readonly ILogWrapper<GuideController> _logger;

        public GuideController(ILogWrapper<GuideController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
