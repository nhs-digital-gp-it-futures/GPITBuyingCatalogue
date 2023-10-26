using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IContactUsService contactUsService;

        public HomeController(IContactUsService contactUsService)
        {
            this.contactUsService = contactUsService ?? throw new ArgumentNullException(nameof(contactUsService));
        }

        public IActionResult Index()
            => View();

        [HttpGet("privacy-policy")]
        public IActionResult PrivacyPolicy()
            => View();

        [Authorize]
        [HttpGet("contact-us")]
        public IActionResult ContactUs()
            => View(new ContactUsModel());

        [Authorize]
        [HttpPost("contact-us")]
        public async Task<IActionResult> ContactUs(ContactUsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await contactUsService.SubmitQuery(
                model.FullName,
                model.EmailAddress,
                model.Message);

            return RedirectToAction(nameof(ContactUsConfirmation));
        }

        [HttpGet("contact-us/confirmation")]
        public IActionResult ContactUsConfirmation()
        {
            var model = new ContactUsConfirmationModel() { BackLink = Url.Action(nameof(Index)), };

            return View(model);
        }

        [HttpGet("accessibility-statement")]
        public IActionResult AccessibilityStatement() => View();

        [HttpGet("unauthorized")]
        public IActionResult NotAuthorized() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null, string error = null)
        {
            if (statusCode is 404)
            {
                var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
                ViewData["BadUrl"] = $"Incorrect url {feature?.OriginalPath}";
                return View("PageNotFound");
            }

            return View(new ErrorModel(error));
        }
    }
}
