using System;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    public class GuideController : Controller
    {
        private readonly ILogWrapper<GuideController> logger;

        public GuideController(ILogWrapper<GuideController> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index()
        {
            logger.LogInformation($"Taking user to {nameof(GuideController)}.{nameof(Index)}");
            return View();
        }
    }
}
