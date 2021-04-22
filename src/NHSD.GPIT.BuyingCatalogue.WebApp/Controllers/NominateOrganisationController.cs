using System;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    public class NominateOrganisationController : Controller
    {
        private readonly ILogWrapper<NominateOrganisationController> _logger;

        public NominateOrganisationController(ILogWrapper<NominateOrganisationController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
