using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/catalogue-solutions")]
    public class CatalogueSolutionsController : Controller
    {
        private readonly ILogWrapper<HomeController> logger;

        public CatalogueSolutionsController(ILogWrapper<HomeController> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index()
        {
            logger.LogInformation($"Taking user to {nameof(CatalogueSolutionsController)}.{nameof(Index)}");

            return View();
        }
    }
}
