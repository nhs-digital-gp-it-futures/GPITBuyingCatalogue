using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ILogWrapper<HomeController> logger;

        public HomeController(ILogWrapper<HomeController> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Route("admin")]
        public IActionResult Index()
        {
            logger.LogInformation($"Taking user to {nameof(HomeController)}.{nameof(Index)}");

            return View();
        }
    }
}
