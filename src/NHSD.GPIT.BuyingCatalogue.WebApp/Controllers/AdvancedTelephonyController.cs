using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    [ExcludeFromCodeCoverage]
    public class AdvancedTelephonyController : Controller
    {
        public IActionResult GetAdvancedTelephonyPdf(string file)
        {
            if (string.IsNullOrWhiteSpace(file)) return BadRequest();

            if (!AdvancedTelephonyConstants.TryGetFileMapping(file, out var fileName)) return BadRequest();

            return GetFileStream($"{fileName}.pdf");
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
