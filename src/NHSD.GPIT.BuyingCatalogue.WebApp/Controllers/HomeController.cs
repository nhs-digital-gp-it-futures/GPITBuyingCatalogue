using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogWrapper<HomeController> logger;

        public HomeController(ILogWrapper<HomeController> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index()
        {
            logger.LogInformation("Taking user to Home Index page");

            return View();
        }

        public IActionResult TestLinks()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode)
        {
            if (statusCode.HasValue && statusCode == 404)
            {
                var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
                ViewData["BadUrl"] = $"Incorrect url {feature?.OriginalPath}";
                return View("PageNotFound");
            }

            return View();
        }
    }
}
