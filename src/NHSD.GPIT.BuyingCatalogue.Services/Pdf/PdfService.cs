using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;

namespace NHSD.GPIT.BuyingCatalogue.Services.Pdf
{
    [ExcludeFromCodeCoverage(Justification = "Can't be tested as it stands up an instance of Google Chrome")]
    public sealed class PdfService : IPdfService
    {
        private const string ChromeArgs = "--no-sandbox --headless --disable-dev-shm-usage --disable-gpu --disable-software-rasterizer --ignore-certificate-errors";
        private const string ChromeWindows32BitPath = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
        private const string ChromeWindows64BitPath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
        private const string ChromeLinuxPath = "/usr/bin/chromium-browser";
        private readonly IActionContextAccessor actionContextAccessor;
        private readonly PdfSettings pdfSettings;

        public PdfService(IActionContextAccessor actionContextAccessor, PdfSettings pdfSettings)
        {
            this.actionContextAccessor =
                actionContextAccessor ?? throw new ArgumentNullException(nameof(actionContextAccessor));
            this.pdfSettings = pdfSettings ?? throw new ArgumentNullException(nameof(pdfSettings));
        }

        public Uri BaseUri()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var httpContext = actionContextAccessor.ActionContext!.HttpContext!;
                return new Uri($"{httpContext.Request.Scheme}://{httpContext.Request.Host}");
            }
            else
            {
                return pdfSettings.UseSslForPdf
                    ? new Uri($"https://localhost")
                    : new Uri($"http://localhost");
            }
        }

        [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Process is disposed of via the Exited event handler")]
        public Task<byte[]> Convert(Uri url)
        {
            string filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");

            var tcs = new TaskCompletionSource<byte[]>();
            var psi = new ProcessStartInfo(GetChromePath(), $"{ChromeArgs} --print-to-pdf=\"{filePath}\" {url}");
            var process = new Process { StartInfo = psi, EnableRaisingEvents = true };

            process.Exited += (_, _) =>
            {
                process.Dispose();

                if (!File.Exists(filePath))
                {
                    tcs.SetException(new InvalidOperationException("Failed to generate PDF"));
                    return;
                }

                var fileContents = File.ReadAllBytes(filePath);
                File.Delete(filePath);

                tcs.SetResult(fileContents);
            };

            process.Start();

            return tcs.Task;
        }

        private static string GetChromePath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return File.Exists(ChromeWindows64BitPath) ? ChromeWindows64BitPath : ChromeWindows32BitPath;
            }

            return ChromeLinuxPath;
        }
    }
}
