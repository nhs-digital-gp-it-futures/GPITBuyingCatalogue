using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    [Route("nominate-organisation")]
    public class NominateOrganisationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
