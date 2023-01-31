﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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

        public IActionResult DownloadComissioningSupportPackPDF()
        {
            var resourceStream = typeof(HomeController).Assembly.GetManifestResourceStream("NHSD.GPIT.BuyingCatalogue.WebApp.Files.Advanced GP Telephony Specification Commissioning Support Pack v1.12.pdf");
            return File(resourceStream, "application/pdf", "Advanced GP Telephony Specification Commissioning Support Pack v1.12.pdf");
        }

        public IActionResult Index()
            => View();

        [HttpGet("privacy-policy")]
        public IActionResult PrivacyPolicy()
            => View();

        [HttpGet("contact-us")]
        public IActionResult ContactUs()
            => View(new ContactUsModel());

        [HttpPost("contact-us")]
        public async Task<IActionResult> ContactUs(ContactUsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await contactUsService.SubmitQuery(
                model.ContactMethod == ContactUsModel.ContactMethodTypes.TechnicalFault,
                model.FullName,
                model.EmailAddress,
                model.Message);

            return RedirectToAction(nameof(ContactUsConfirmation), new { contactReason = model.ContactMethod });
        }

        [HttpGet("contact-us/confirmation")]
        public IActionResult ContactUsConfirmation(ContactUsModel.ContactMethodTypes contactReason)
        {
            var model = new ContactUsConfirmationModel(contactReason)
            {
                BackLink = Url.Action(nameof(Index)),
            };

            return View(model);
        }

        [HttpGet("accessibility-statement")]
        public IActionResult AccessibilityStatement() => View();

        [HttpGet("unauthorized")]
        public IActionResult NotAuthorized() => View();

        [HttpGet("tech-innovation")]
        public IActionResult TechInnovationFramework() => View(new NavBaseModel
        {
            BackLink = Url.Action(
                nameof(Index),
                typeof(HomeController).ControllerName()),
        });

        [HttpGet("advanced-telephony-better-purchase")]
        public IActionResult AdvacedTelephonyBetterPurchaseFramework() => View(new NavBaseModel
        {
            BackLink = Url.Action(
                nameof(Index),
                typeof(HomeController).ControllerName()),
        });

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
