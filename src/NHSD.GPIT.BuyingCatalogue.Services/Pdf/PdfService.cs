using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;

namespace NHSD.GPIT.BuyingCatalogue.Services.Pdf
{
    [ExcludeFromCodeCoverage(Justification = "Can't be tested as it stands up an instance of Google Chrome")]
    public sealed class PdfService : IPdfService
    {
        private const string ChromeWindows32BitPath = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
        private const string ChromeWindows64BitPath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
        private const string ChromeLinuxPath = "/usr/bin/chromium-browser";
        private const string ChromeMacPath = "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome";

        private static readonly string[] ChromeArgs =
        [
            "--no-sandbox",
            "--headless=new",
            "--disable-dev-shm-usage",
            "--disable-gpu",
            "--disable-software-rasterizer",
            "--ignore-certificate-errors",
            "--no-pdf-header-footer",
        ];

        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly PdfSettings pdfSettings;

        public PdfService(IHttpContextAccessor httpContextAccessor, PdfSettings pdfSettings)
        {
            this.httpContextAccessor =
                httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            this.pdfSettings = pdfSettings ?? throw new ArgumentNullException(nameof(pdfSettings));
        }

        public Uri BaseUri()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                && !RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return pdfSettings.UseSslForPdf
                    ? new Uri($"https://localhost")
                    : new Uri($"http://localhost");
            }

            var httpContext = httpContextAccessor.HttpContext!;
            return new Uri($"{httpContext.Request.Scheme}://{httpContext.Request.Host}");
        }

        public async Task<byte[]> Convert(Uri url)
        {
            ArgumentNullException.ThrowIfNull(url);

            string filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");
            var userDataDir = Path.Combine(Path.GetTempPath(), $"chrome-{Guid.NewGuid()}");

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = GetChromePath(),
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                };

                foreach (var arg in ChromeArgs)
                {
                    psi.ArgumentList.Add(arg);
                }

                psi.ArgumentList.Add($"--user-data-dir={userDataDir}");
                psi.ArgumentList.Add($"--print-to-pdf={filePath}");
                psi.ArgumentList.Add(url.ToString());

                using var process = Process.Start(psi) ?? throw new InvalidOperationException("Failed to start Chrome");

                var stderrTask = process.StandardError.ReadToEndAsync();
                var stdoutTask = process.StandardOutput.ReadToEndAsync();

                await process.WaitForExitAsync();

                if (!File.Exists(filePath))
                {
                    throw new InvalidOperationException("Failed to generate PDF");
                }

                return await File.ReadAllBytesAsync(filePath);
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                if (Directory.Exists(userDataDir))
                {
                    Directory.Delete(userDataDir, recursive: true);
                }
            }
        }

        private static string GetChromePath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return File.Exists(ChromeWindows64BitPath) ? ChromeWindows64BitPath : ChromeWindows32BitPath;
            }

            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                ? ChromeMacPath
                : ChromeLinuxPath;
        }
    }
}
