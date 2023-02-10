using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    public class AdvancedTelephonyController : Controller
    {
        [ExcludeFromCodeCoverage]
        public IActionResult ThinkHealthcarePdf()
        {
            const string fileName = "Think Healthcare.pdf";
            var resourceStream =
                typeof(HomeController).Assembly.GetManifestResourceStream(
                    $"NHSD.GPIT.BuyingCatalogue.WebApp.Files.{fileName}");

            return File(resourceStream!, "application/pdf", fileName);
        }

        [ExcludeFromCodeCoverage]
        public IActionResult DaisyPatientLinePdf()
        {
            const string fileName = "Daisy Patient Line.pdf";
            var resourceStream =
                typeof(HomeController).Assembly.GetManifestResourceStream(
                    $"NHSD.GPIT.BuyingCatalogue.WebApp.Files.{fileName}");

            return File(resourceStream!, "application/pdf", fileName);
        }

        [ExcludeFromCodeCoverage]
        public IActionResult RedcentricPdf()
        {
            const string fileName = "Redcentric.pdf";
            var resourceStream =
                typeof(HomeController).Assembly.GetManifestResourceStream(
                    $"NHSD.GPIT.BuyingCatalogue.WebApp.Files.{fileName}");

            return File(resourceStream!, "application/pdf", fileName);
        }

        [ExcludeFromCodeCoverage]
        public IActionResult SurgeryConnectPdf()
        {
            const string fileName = "Surgery Connect.pdf";
            var resourceStream =
                typeof(HomeController).Assembly.GetManifestResourceStream(
                    $"NHSD.GPIT.BuyingCatalogue.WebApp.Files.{fileName}");

            return File(resourceStream!, "application/pdf", fileName);
        }
    }
}
