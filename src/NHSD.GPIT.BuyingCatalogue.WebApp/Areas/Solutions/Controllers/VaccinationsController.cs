using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers
{
    [Area("Solutions")]
    public class VaccinationsController : Controller
    {
        public VaccinationsController()
        {
        }

        [Route("Solutions/Vaccinations")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
