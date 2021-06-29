using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    public class GuideController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
