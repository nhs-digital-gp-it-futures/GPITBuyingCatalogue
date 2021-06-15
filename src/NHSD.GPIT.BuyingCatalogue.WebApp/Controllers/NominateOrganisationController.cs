using System;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    public class NominateOrganisationController : Controller
    {
        private readonly ILogWrapper<NominateOrganisationController> logger;

        public NominateOrganisationController(ILogWrapper<NominateOrganisationController> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index()
        {
            logger.LogInformation($"Taking user to {nameof(NominateOrganisationController)}.{nameof(Index)}");

            return View();
        }
    }
}
