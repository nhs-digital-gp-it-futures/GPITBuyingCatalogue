using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;

namespace NHSD.GPIT.BuyingCatalogue.Services.Pdf
{
    public sealed class PdfService : IPdfService
    {
        private const string ChromeArgs = "--no-sandbox --headless --disable-gpu --disable-software-rasterizer --ignore-certificate-errors";
        private const string ChromeWindowsPath = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
        private const string ChromeLinuxPath = "/usr/bin/chromium-browser";

        public byte[] Convert(Uri url)
        {
            string filePath = @$"{Path.GetTempPath()}{Guid.NewGuid()}.pdf";
            string chromePath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ChromeWindowsPath : ChromeLinuxPath;

            var psi = new ProcessStartInfo(chromePath, $"{ChromeArgs} --print-to-pdf={filePath} {url}");
            var process = Process.Start(psi);
            process.WaitForExit(60000);

            if (File.Exists(filePath))
            {
                byte[] fileContent = File.ReadAllBytes(filePath);

                if (File.Exists(filePath))
                    File.Delete(filePath);

                return fileContent;
            }
            else
            {
                var rnd = new Random();
                var b = new byte[10];
#pragma warning disable CA5394 // Do not use insecure randomness
                rnd.NextBytes(b);
#pragma warning restore CA5394 // Do not use insecure randomness

                return b;
            }
        }
    }
}
