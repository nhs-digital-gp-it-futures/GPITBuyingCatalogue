﻿using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    [ExcludeFromCodeCoverage]
    public class AdvancedTelephonyController : Controller
    {
        public IActionResult BuyersGuidePdf()
        {
            const string fileName = "Buyer's Guide for Advanced Cloud-based Telephony-Jan 2023.pdf";

            return GetFileStream(fileName);
        }

        public IActionResult ThinkHealthcarePdf()
        {
            const string fileName = "Think Healthcare.pdf";

            return GetFileStream(fileName);
        }

        public IActionResult DaisyPatientLinePdf()
        {
            const string fileName = "Daisy Patient Line.pdf";

            return GetFileStream(fileName);
        }

        public IActionResult RedcentricPdf()
        {
            const string fileName = "Redcentric.pdf";

            return GetFileStream(fileName);
        }

        public IActionResult SurgeryConnectPdf()
        {
            const string fileName = "Surgery Connect.pdf";

            return GetFileStream(fileName);
        }

        public IActionResult BabbleVoicePdf()
        {
            const string fileName = "Babblevoice.pdf";

            return GetFileStream(fileName);
        }

        private static Stream GetResourceStream(string fileName) =>
            typeof(HomeController).Assembly.GetManifestResourceStream(
                $"NHSD.GPIT.BuyingCatalogue.WebApp.Files.{fileName}");

        private FileStreamResult GetFileStream(string fileName) => File(
            GetResourceStream(fileName),
            "application/pdf",
            fileName);
    }
}
