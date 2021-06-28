using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers
{
    [Area("Solutions")]
    public sealed class VaccinationsController : Controller
    {
        [Route("Solutions/Vaccinations")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
