using System.Collections.Generic;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private IEmailService emailService;
        private ContactUsSettings contactUsSettings;

        public HomeController(IEmailService eService, ContactUsSettings contactSettings)
        {
            emailService = eService;
            contactUsSettings = contactSettings;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("privacy-policy")]
        public IActionResult PrivacyPolicy()
        {
            return View();
        }

        [HttpGet("contact-us")]
        public IActionResult ContactUs()
        {
            var model = new ContactUsModel();
            return View(model);
        }

        [HttpPost("contact-us")]
        public IActionResult ContactUs(ContactUsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var recipiant = new EmailAddress(contactUsSettings.TechnicalFaultAddress, "Technical Fault Address");
            var email = new EmailMessage(contactUsSettings.EmailMessage, new List<EmailAddress>() { recipiant });

            emailService.SendEmailAsync(email);

            return View("ContactUsConfirmation");
        }

        [HttpGet("accessibility-statement")]
        public IActionResult AccessibilityStatement() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null, string error = null)
        {
            if (statusCode.HasValue && statusCode == 404)
            {
                var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
                ViewData["BadUrl"] = $"Incorrect url {feature?.OriginalPath}";
                return View("PageNotFound");
            }

            return View(new ErrorModel(error));
        }
    }
}
