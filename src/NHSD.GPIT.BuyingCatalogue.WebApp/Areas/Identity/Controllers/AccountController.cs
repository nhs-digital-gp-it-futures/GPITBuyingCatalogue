using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            var loginViewModel = new LoginViewModel { ReturnUrl = returnUrl };

            return View(loginViewModel);
        }

        public IActionResult ResetPasswordExpired()
        {
            return View();
        }
    }
}
