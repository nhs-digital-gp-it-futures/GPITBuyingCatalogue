using System;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers
{
    [Area("Solutions")]
    public class VaccinationsController : Controller
    {
        private readonly ILogWrapper<VaccinationsController> logger;

        public VaccinationsController(ILogWrapper<VaccinationsController> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Route("Solutions/Vaccinations")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
