using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    public class NominateOrganisationController : Controller
    {
        private readonly ILogger<NominateOrganisationController> _logger;

        public NominateOrganisationController(ILogger<NominateOrganisationController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
