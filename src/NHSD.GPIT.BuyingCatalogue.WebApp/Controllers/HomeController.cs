using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("privacy-policy")]
        public IActionResult PrivacyPolicy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode)
        {
            if (statusCode.HasValue && statusCode == 404)
            {
                var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
                ViewData["BadUrl"] = $"Incorrect url {feature?.OriginalPath}";
                return View("PageNotFound");
            }

            return View();
        }
    }
}
